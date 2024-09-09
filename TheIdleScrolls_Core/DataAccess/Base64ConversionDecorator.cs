using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Storage;

namespace TheIdleScrolls_Core.DataAccess
{
    public class Base64ConversionDecorator<T> : IDataEncryptor<T, string>
    {
        public IDataEncryptor<T, byte[]> Inner { get; set; }

        public Base64ConversionDecorator(IDataEncryptor<T, byte[]> inner)
        {
            Inner = inner;
        }

        public string EncryptData(T data)
        {
            var array = Inner.EncryptData(data);
            return Convert.ToBase64String(array);
        }

        public T DecryptData(string data)
        {
            var array = Convert.FromBase64String(data);
            return Inner.DecryptData(array);
        }
    }
}
