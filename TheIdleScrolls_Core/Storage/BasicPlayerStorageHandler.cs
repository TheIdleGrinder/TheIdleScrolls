﻿using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Storage
{
    public class BasicFileStorageHandler : IStorageHandler<string>
    {
        readonly string StorageDirectory;

        public BasicFileStorageHandler()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            StorageDirectory = Path.Combine(appDataPath, "TheIdleGrind", "saves");
        }

        public string LoadData(string key)
        {
            string path = BuildPath(key);
            if (!File.Exists(path))
                return "";
            return File.ReadAllText(path);
        }

        public void StoreData(string key, string data)
        {
            string path = BuildPath(key);
            if (!CreateStorageDirectoryIfNecessary())
                throw new DirectoryNotFoundException("Could not create save game folder");
            Task.Factory.StartNew(() => File.WriteAllText(path, data)); // TODO: Check if this really saves any time
        }

        string BuildPath(string fileName)
        {
            return Path.Combine(StorageDirectory, fileName + ".json");
        }

        bool CreateStorageDirectoryIfNecessary()
        {
            var info = Directory.CreateDirectory(StorageDirectory);
            return info.Exists;
        }
    }
}
