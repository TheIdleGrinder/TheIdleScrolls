using TheIdleScrolls_Core.Storage;

namespace Test_TheIdleScrolls_Core.Utility
{
	public class DictionaryStorageHandler : IStorageHandler<string>
	{
		readonly Dictionary<string, string> m_localStorage;

		public DictionaryStorageHandler()
		{
			m_localStorage = new Dictionary<string, string>();
		}

		public Task DeleteData(string key)
		{
			m_localStorage.Remove(key);
			return Task.CompletedTask;
		}

		public Task<List<string>> GetKeys()
		{
			return Task.FromResult(m_localStorage.Keys.ToList());
		}

		public Task<string> LoadData(string key)
		{
			return Task.FromResult(m_localStorage.ContainsKey(key) ? m_localStorage[key] : "");
		}

		public Task StoreData(string key, string data)
		{
			m_localStorage[key] = data;
			return Task.CompletedTask;
		}

		public Task ExportData(string data)
		{
			throw new NotImplementedException();
        }
	}
}
