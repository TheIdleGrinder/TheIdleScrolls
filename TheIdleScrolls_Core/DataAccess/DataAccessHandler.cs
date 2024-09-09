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
        IEntityConverter Converter { get; set; }
        
        IDataEncryptor<string, string>? DataEncryptor { get; set; }

        IStorageHandler<string> Storage { get; set; }

        public DataAccessHandler(IEntityConverter entityConverter, 
            IStorageHandler<string> storageHandler, 
            IDataEncryptor<string, string>? dataEncryptor = null)
        {
            Converter = entityConverter;
            Storage = storageHandler;
            DataEncryptor = dataEncryptor;
        }

        public IStorageHandler<string> StorageHandler
        {
            get { return Storage; }
        }

        public string Encrypt(string data)
        {
            if (DataEncryptor != null)
            {
                return DataEncryptor.EncryptData(data);
            }
            return data;
        }

        public string Decrypt(string data)
        {
            if (DataEncryptor != null)
            {
                return DataEncryptor.DecryptData(data);
            }
            return data;
        }

        public Task<string> LoadData(string key)
        {
            return Storage.LoadData(key);
        }

        public Task StoreData(string key, string data)
        {
            return Storage.StoreData(key, data);
        }

        public Task StoreEntity(Entity entity)
        {
            string serialized = Converter.SerializeEntity(entity);
            serialized = Encrypt(serialized);
            return Storage.StoreData(entity.GetName(), serialized);
        }

        public async Task<bool> LoadEntity(string accessKey, Entity outputEntity)
        {
            string serialized = await Storage.LoadData(accessKey);
            serialized = Decrypt(serialized);
            if (serialized != String.Empty)
            {
                var loaded = Converter.DeserializeEntity(serialized);
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
            return Storage.GetKeys();
        }

        public async Task DeleteStoredEntity(string accessKey)
        {
            await Storage.DeleteData(accessKey);
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
				string charData = await Storage.LoadData(charName);
				data.Add(charData);
			}
            // Currently, the most recently saved character is the first one in the list. We reverse the list to make it the last one that gets imported.
            data.Reverse();
			return String.Join("|", data); // CornerCut: assumes that the data is not going to contain the pipe character
		}

        public async Task ExportAllCharacters()
        {
            string export = await GetExportText();
            await Storage.ExportData(export);
        }

        public async Task ImportCharacters(string data)
        {
            string[] charData = data.Split('|');
            List<Entity> entities = new();
            foreach (string charString in charData)
            {
                try
                {
                    string serialized = Decrypt(charString);
                    Entity? entity = Converter.DeserializeEntity(serialized);
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
