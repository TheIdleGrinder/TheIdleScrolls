using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TheIdleScrolls_Core.Storage;

namespace TheIdleScrolls_Core.DataAccess
{
    public class DataAccessHandler
    {
        IEntityConverter m_converter { get; set; }

        IStorageHandler<string> m_storage { get; set; }

        public DataAccessHandler(IEntityConverter entityConverter, IStorageHandler<string> storageHandler)
        {
            m_converter = entityConverter;
            m_storage = storageHandler;
        }

        public void StoreEntity(Entity entity)
        {
            string serialized = m_converter.SerializeEntity(entity);
            m_storage.StoreData(entity.GetName(), serialized);
        }

        public bool LoadEntity(string accessKey, Entity outputEntity)
        {
            string serialized = m_storage.LoadData(accessKey);
            if (serialized != "")
            {
                var loaded = m_converter.DeserializeEntity(serialized);
                if (loaded != null)
                {
                    foreach (var component in loaded.Components)
                    {
                        outputEntity.AddComponent(component as dynamic);
                    }
                    return true;
                }
            }
            return false;
        }

        public Entity? LoadEntity(string accessKey)
        {
            Entity entity = new();
            var success = LoadEntity(accessKey, entity);
            return success ? entity : null;
        }

        public List<string> ListStoredEntities()
        {
            return m_storage.GetKeys();
        }


    }
}
