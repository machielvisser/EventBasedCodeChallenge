using EventSourcing.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mocks
{
    public class KeyStoreMock : IKeyStore
    {
        private readonly Dictionary<string, byte[]> _keys;

        public KeyStoreMock()
        {
            _keys = new Dictionary<string, byte[]>();
        }

        public async Task Add(string id, byte[] key)
        {
            _keys.Add(id, key);

            await Task.Yield();
        }

        public async Task<bool> Contains(string id)
        {
            return await Task.FromResult(_keys.ContainsKey(id));
        }

        public async Task<byte[]> Get(string id)
        {
            _keys.TryGetValue(id, out byte[] key);

            return await Task.FromResult(key);
        }
    }
}
