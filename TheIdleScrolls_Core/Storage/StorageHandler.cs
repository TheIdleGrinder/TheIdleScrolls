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
        public void StoreData(string key, T data);

        public T LoadData(string key);

        public void DeleteData(string key);

        public List<string> GetKeys();
    }
}
