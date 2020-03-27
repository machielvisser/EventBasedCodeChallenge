using EventSourcing.Entities;
using EventSourcing.Exceptions;
using EventSourcing.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace EventSourcing
{
    public class EventRepository
    {
        private static readonly int DeleteAfter = 30;

        private readonly IEventStore _eventStore;
        private readonly IKeyStore _keyStore;

        private static JsonSerializerSettings _serializerSettings;

        private readonly Dictionary<string, byte[]> _keyCache;

        /// <summary>
        /// Wraps the event store and adds the type of the stream to the stream identifier
        /// </summary>
        /// <param name="eventStore"></param>
        public EventRepository(IEventStore eventStore, IKeyStore keyStore)
        {
            _eventStore = eventStore;
            _keyStore = keyStore;

            _serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            _keyCache = new Dictionary<string, byte[]>();
        }

        /// <summary>
        /// Get all the events of a stream
        /// </summary>
        /// <typeparam name="T">The type of the aggregate</typeparam>
        /// <param name="id">The id of the aggregate</param>
        /// <returns>All the events of the stream</returns>
        public async Task<IEnumerable<Event>> Get<T>(Guid id)
        {
            var streamId = StreamId<T>(id);
            using var aes = await GetAes(streamId);
            var encryptedEvents = await _eventStore.Get(streamId);

            return await Task.WhenAll(encryptedEvents.Select(async @event => await Decrypt(aes, @event)).ToList());
        }

        /// <summary>
        /// Add one event to the stream
        /// </summary>
        /// <typeparam name="T">Type of the aggregate</typeparam>
        /// <param name="id">The id of the aggregate</param>
        /// <param name="event">The event to add</param>
        /// <returns></returns>
        public async Task Add<T>(Guid id, Event @event)
        {
            await Add<T>(id, new[] { @event });
        }

        /// <summary>
        /// Add multiple events to the stream
        /// </summary>
        /// <typeparam name="T">Type of the aggregate</typeparam>
        /// <param name="id">The id of the aggregate</param>
        /// <param name="events">The events to add</param>
        public async Task Add<T>(Guid id, IEnumerable<Event> events)
        {
            var streamId = StreamId<T>(id);
            using var aes = await GetAes(streamId, true);

            await _eventStore.Add(streamId, await Task.WhenAll(events.Select(async @event => await Encrypt(aes, @event))));

            if (events.Any(@event => @event is IProtectedDataDeletionEvent))
                await _keyStore.Delete(streamId, DeleteAfter);
        }

        /// <summary>
        /// Get a stream id from the aggregate id
        /// </summary>
        /// <typeparam name="T">Type of the aggregate</typeparam>
        /// <param name="id">Id of the aggregate</param>
        /// <returns>The stream id</returns>
        private string StreamId<T>(Guid id)
        {
            return $"{typeof(T).FullName}.{id}";
        }

        /// <summary>
        /// Configure the AES instance
        /// </summary>
        /// <param name="streamId">Id of the stream to obtain the encryption key for</param>
        /// <param name="create">Whether to create a new key in case there is none in the key store</param>
        /// <returns>The configured AES instance</returns>
        private async Task<Aes> GetAes(string streamId, bool create = false)
        {
            var aes = new AesManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            };

            if (!_keyCache.ContainsKey(streamId))
            {
                if (!await _keyStore.Contains(streamId))
                {
                    if (create)
                    {
                        aes.GenerateKey();
                        await _keyStore.Add(streamId, aes.Key);
                        _keyCache.Add(streamId, aes.Key);
                    }
                    else
                    {
                        throw new KeyDeletedException("The encryption key has been deleted");
                    }
                }
                else
                {
                    var value = await _keyStore.Get(streamId);
                    _keyCache.Add(streamId, value);
                }
            }

            aes.Key = _keyCache[streamId];

            return aes;
        }

        /// <summary>
        /// Serializes and encrypts the object using AES
        /// </summary>
        /// <param name="aes">The configured AES instance</param>
        /// <param name="input">The object to encrypt</param>
        /// <returns>The encrypted object</returns>
        private static async Task<EncryptedEvent> Encrypt(Aes aes, Event input)
        {
            var serializedInput = JsonConvert.SerializeObject(input, Formatting.Indented, _serializerSettings);

            aes.GenerateIV();
            var encryptor = aes.CreateEncryptor();

            byte[] output;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
                using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                {
                    await streamWriter.WriteAsync(serializedInput);
                }

                output = memoryStream.ToArray();
            }

            return new EncryptedEvent(input.Location, output, aes.IV);
        }

        /// <summary>
        /// Decrypts and deserializes the input object using AES
        /// </summary>
        /// <param name="aes">The configured AES instance</param>
        /// <param name="input">The encrypted event object</param>
        /// <returns>The decrypted event object</returns>
        private static async Task<Event> Decrypt(Aes aes, EncryptedEvent input)
        {
            aes.IV = input.InitializationVector;
            var decryptor = aes.CreateDecryptor();

            string output;
            using (MemoryStream memoryStream = new MemoryStream(input.Content))
            {
                using CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                using StreamReader streamReader = new StreamReader(cryptoStream);
                output = await streamReader.ReadToEndAsync();
            }

            return JsonConvert.DeserializeObject<Event>(output, _serializerSettings);
        }
    }
}
