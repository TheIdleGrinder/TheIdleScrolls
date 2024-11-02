using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using TheIdleScrolls_Core.DataAccess;
using TheIdleScrolls_Core.Items;

namespace TheIdleScrolls_Storage
{
    public class EntityJsonConverter : IEntityConverter
    {
        private static JsonSerializerOptions _options = new() { 
            WriteIndented = true, 
            TypeInfoResolver = new DefaultJsonTypeInfoResolver() 
        };

        public EntityJsonConverter() { }

        public Entity? DeserializeEntity(string serialized)
        {
            JsonObject? json = JsonNode.Parse(serialized)?.AsObject();
            if (json == null)
                return null;
            return EntityJsonConversion.EntityFromJson(json);
        }

        public bool DeserializeEntity(string serialized, out Entity entity)
        {
            throw new NotImplementedException();
        }

        public string SerializeEntity(Entity entity)
        {
            return entity.ToJson().ToJsonString(_options);
        }
    }
}
