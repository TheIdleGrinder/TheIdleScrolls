using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.DataAccess
{
    public interface IDataEncryptor<T, U>
    {
        public U EncryptData(T data);

        public T DecryptData(U data);
    }

    public class NopDataEncryptor<T> : IDataEncryptor<T, T>
    {
        public T EncryptData(T data)
        {
            return data;
        }

        public T DecryptData(T data)
        {
            return data;
        }
    }
}
