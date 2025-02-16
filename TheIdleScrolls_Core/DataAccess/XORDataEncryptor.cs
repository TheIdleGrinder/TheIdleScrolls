using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.DataAccess
{
    public class XORDataEncryptor(byte[] Key) : IDataEncryptor<byte[], byte[]>
    {
        public byte[] DecryptData(byte[] data)
        {
            if (data.Length == 0 || data[0] == '{')
            {
                return data;
            }
            ApplyXOR(data);
            return data;
        }

        public byte[] EncryptData(byte[] data)
        {
            ApplyXOR(data);
            return data;
        }

        private void ApplyXOR(byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= Key[i % Key.Length];
            }
        }
    }
}
