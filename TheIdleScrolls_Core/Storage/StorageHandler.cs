using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniECS;

namespace TheIdleScrolls_Core.Storage
{
    public interface IStorageHandler<T>
    {
        public Task StoreData(string key, T data);

        public Task<T> LoadData(string key);

        public Task DeleteData(string key);

        public Task<List<string>> GetKeys();
    }
}
