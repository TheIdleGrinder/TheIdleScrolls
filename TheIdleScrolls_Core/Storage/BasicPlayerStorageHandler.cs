using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Storage
{
    public class BasicFileStorageHandler : IStorageHandler<string>
    {
        const string FileExtension = ".json";
        readonly string StorageDirectory;

        public BasicFileStorageHandler()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            StorageDirectory = Path.Combine(appDataPath, "TheIdleGrind", "saves");
        }

        public Task DeleteData(string key)
        {
            string path = BuildPath(key);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            return Task.CompletedTask;
        }

        public Task<List<string>> GetKeys()
        {
            return Task.FromResult(Directory.GetFiles(StorageDirectory)
                .OrderByDescending(f => File.GetLastWriteTime(f))
                .Where(f => f.EndsWith(FileExtension))
                .Select(f => Path.GetFileNameWithoutExtension(f))
                .Where(f => !f.StartsWith("_"))
                .ToList());
        }

        public Task<string> LoadData(string key)
        {
            string path = BuildPath(key);
            return Task.FromResult(File.Exists(path) ? File.ReadAllText(path) : "");
        }

        public Task StoreData(string key, string data)
        {
            string path = BuildPath(key);
            if (!CreateStorageDirectoryIfNecessary())
                throw new DirectoryNotFoundException("Could not create save game folder");
            return Task.Run(() => File.WriteAllText(path, data));
        }

        string BuildPath(string fileName)
        {
            return Path.Combine(StorageDirectory, fileName + FileExtension);
        }

        bool CreateStorageDirectoryIfNecessary()
        {
            var info = Directory.CreateDirectory(StorageDirectory);
            return info.Exists;
        }
    }
}
