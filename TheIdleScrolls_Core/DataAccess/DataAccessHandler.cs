using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
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

        public Task StoreEntity(Entity entity)
        {
            string serialized = m_converter.SerializeEntity(entity);
            return m_storage.StoreData(entity.GetName(), serialized);
        }

        public async Task<bool> LoadEntity(string accessKey, Entity outputEntity)
        {
            string serialized = await m_storage.LoadData(accessKey);
            if (serialized != String.Empty)
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

        public async Task<Entity?> LoadEntity(string accessKey)
        {
            Entity entity = new();
            var success = await LoadEntity(accessKey, entity);
            return success ? entity : null;
        }

        public Task<List<string>> ListStoredEntities()
        {
            return m_storage.GetKeys();
        }

        public async Task DeleteStoredEntity(string accessKey)
        {
            await m_storage.DeleteData(accessKey);
        }

        public async Task DeleteAllStoredEntities()
        {
            var keys = await ListStoredEntities();
            foreach (string key in keys)
            {
                await DeleteStoredEntity(key);
            }
        }

        public async Task<string> GetExportText()
        {
			var chars = await ListStoredEntities();
			List<string> data = new();
			foreach (string charName in chars)
			{
				string charData = await m_storage.LoadData(charName);
				data.Add(charData);
			}
			return String.Join("|", data); // CornerCut: assumes that the data is not going to contain the pipe character
		}

        public async Task ExportAllCharacters()
        {
            string export = await GetExportText();
            await m_storage.ExportData(export);
        }

        public async Task ImportCharacters(string data)
        {
            string[] charData = data.Split('|');
            List<Entity> entities = new();
            foreach (string charString in charData)
            {
                try
                {
                    Entity? entity = m_converter.DeserializeEntity(charString);
                    if (entity != null)
                    {
                        entities.Add(entity);
                    }
                }
				catch (Exception e)
                {
					throw new Exception("Invalid import data: " + e.Message);
				}
            }
            // Only store entities if all of them were successfully deserialized
            foreach (Entity entity in entities)
            {
				await StoreEntity(entity);
			}
        }
    }
}
