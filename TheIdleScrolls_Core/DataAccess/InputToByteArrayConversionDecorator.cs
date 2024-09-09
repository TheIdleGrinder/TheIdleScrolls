using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Storage;

namespace TheIdleScrolls_Core.DataAccess
{
    public class InputToByteArrayConversionDecorator<T> : IDataEncryptor<string, T>
    {
        public IDataEncryptor<byte[], T> Inner { get; set; }

        public InputToByteArrayConversionDecorator(IDataEncryptor<byte[], T> inner)
        {
            Inner = inner;
        }

        public T EncryptData(string data)
        {
            return Inner.EncryptData(Encoding.UTF8.GetBytes(data));
        }

        public string DecryptData(T data)
        {
            return Encoding.UTF8.GetString(Inner.DecryptData(data));
        }
    }
}
