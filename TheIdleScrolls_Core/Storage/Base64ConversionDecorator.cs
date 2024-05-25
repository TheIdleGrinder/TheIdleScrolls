using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Storage
{
    public class Base64ConversionDecorator : IStorageHandler<byte[]>
    {
        public IStorageHandler<string> Inner { get; set; }

        public Base64ConversionDecorator(IStorageHandler<string> inner)
        {
            Inner = inner;
        }

        public Task DeleteData(string key)
        {
            return Inner.DeleteData(key);
        }

        public Task ExportData(byte[] data)
        {
            var encoded = Convert.ToBase64String(data);
            return Inner.ExportData(encoded);
        }

        public Task<List<string>> GetKeys()
        {
            return Inner.GetKeys();
        }

        public Task<byte[]> LoadData(string key)
        {
            return Inner.LoadData(key).ContinueWith(t => Convert.FromBase64String(t.Result));
        }

        public Task StoreData(string key, byte[] data)
        {
            var encoded = Convert.ToBase64String(data);
            return Inner.StoreData(key, encoded);
        }
    }
}
