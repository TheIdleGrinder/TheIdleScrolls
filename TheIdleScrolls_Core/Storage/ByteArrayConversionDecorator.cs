using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Storage
{
    public class ByteArrayConversionDecorator : IStorageHandler<string>
    {
        public IStorageHandler<byte[]> Inner { get; set; }

        public ByteArrayConversionDecorator(IStorageHandler<byte[]> inner)
        {
            Inner = inner;
        }

        public Task DeleteData(string key)
        {
            return Inner.DeleteData(key);
        }

        public Task ExportData(string data)
        {
            var array = Encoding.UTF8.GetBytes(data);
            return Inner.ExportData(array);
        }

        public Task<List<string>> GetKeys()
        {
            return Inner.GetKeys();
        }

        public Task<string> LoadData(string key)
        {
            return Inner.LoadData(key).ContinueWith(t => Encoding.UTF8.GetString(t.Result));
        }

        public Task StoreData(string key, string data)
        {
            var array = Encoding.UTF8.GetBytes(data);
            return Inner.StoreData(key, array);
        }
    }
}
