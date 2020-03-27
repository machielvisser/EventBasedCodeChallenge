using System.Threading.Tasks;

namespace EventSourcing.Interfaces
{
    public interface IKeyStore
    {
        /// <summary>
        /// Add an encryption key to the key store
        /// </summary>
        /// <param name="id">Id of the key (id of the aggregate it belongs to)</param>
        /// <param name="key">The encryption key</param>
        public Task Add(string id, byte[] key);

        /// <summary>
        /// Get an encryption key
        /// </summary>
        /// <param name="id">The id of the key</param>
        /// <returns>The encryption key</returns>
        public Task<byte[]> Get(string id);

        /// <summary>
        /// Checks whether a key exists
        /// </summary>
        /// <param name="id">Id of the key</param>
        /// <returns>Whether the key exists</returns>
        public Task<bool> Contains(string id);

        /// <summary>
        /// Delete a key from the store after daysFromNow days
        /// </summary>
        /// <param name="id">The id of the key</param>
        /// <param name="daysFromNow">Days to wait before actually deleting the key from the store</param>
        public Task Delete(string id, int daysFromNow);
    }
}
