using EventSourcing.Entities;
using EventSourcing.Exceptions;
using EventSourcing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mocks
{
    public class EventStoreMock : IEventStore
    {
        private readonly Dictionary<string, List<EncryptedEvent>> _eventStreams;

        public EventStoreMock(Dictionary<string, List<EncryptedEvent>> streams = default)
        {
            _eventStreams = streams ?? new Dictionary<string, List<EncryptedEvent>>();
        }

        public async Task Add(string key, IEnumerable<EncryptedEvent> events)
        {
            foreach (var @event in events)
                await Add(key, @event);
        }

        public async Task Add(string key, EncryptedEvent @event)
        {
            if (!string.IsNullOrEmpty(key))
            {
                if (!_eventStreams.ContainsKey(key))
                {
                    _eventStreams.Add(key, new List<EncryptedEvent>());
                }

                // Check the event is at a valid location of the stream
                if (_eventStreams[key].Count + 1 != @event.Location)
                    throw new EventConcurrencyException("Location of provided event is invalid");

                _eventStreams[key].Add(@event);

                await Task.Yield();
            }
            else
            {
                throw new ArgumentNullException("Key cannot be null or empty");
            }
        }

        public async Task<IEnumerable<EncryptedEvent>> Get(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                IEnumerable<EncryptedEvent> result;
                if (_eventStreams.ContainsKey(key))
                {
                    result = _eventStreams[key];
                }
                else
                {
                    result = Enumerable.Empty<EncryptedEvent>();
                }

                return await Task.FromResult(result);
            }
            else
            {
                throw new ArgumentNullException("Key cannot be null or empty");
            }
        }

        public void Reset()
        {
            _eventStreams.Clear();
        }
    }
}
