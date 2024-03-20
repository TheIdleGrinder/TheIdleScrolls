using MiniECS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using TheIdleScrolls_Core.DataAccess;
using TheIdleScrolls_Core.Items;

namespace TheIdleScrolls_Storage
{
    public class Base64ConverterDecorator : IEntityConverter
    {
        readonly IEntityConverter InnerConverter;

        public Base64ConverterDecorator(IEntityConverter innerConverter)
        {
            InnerConverter = innerConverter;
        }

        public Entity? DeserializeEntity(string serialized)
        {
            string decodedString = serialized;
            // If the string contains { or }, it's not base64 encoded, so probably from an older savegame
            if (!serialized.Contains('{') && !serialized.Contains('}'))
            {
                // Assume it's base64 encoded
                var decoded = Convert.FromBase64String(serialized);
                decodedString = Encoding.UTF8.GetString(decoded);
            }
            return InnerConverter.DeserializeEntity(decodedString);
        }

        public bool DeserializeEntity(string serialized, out Entity entity)
        {
            var result = DeserializeEntity(serialized);
            if (result == null)
            {
                entity = new Entity();
                return false;
            }
            entity = result;
            return true;
        }

        public string SerializeEntity(Entity entity)
        {
            string converted = InnerConverter.SerializeEntity(entity);
            byte[] encoded = Encoding.UTF8.GetBytes(converted);
            return Convert.ToBase64String(encoded);
        }
    }
}
