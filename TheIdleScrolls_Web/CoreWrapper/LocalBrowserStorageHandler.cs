using Microsoft.JSInterop;
using TheIdleScrolls_Core.Storage;

namespace TheIdleScrolls_Web.CoreWrapper
{
	public class LocalBrowserStorageHandler : IStorageHandler<string>
	{ 
		readonly LocalStorageAccessor m_localStorage;

		public LocalBrowserStorageHandler(IJSRuntime jSRuntime)
		{
			m_localStorage = new LocalStorageAccessor(jSRuntime);
		}

		public async void DeleteData(string key)
		{
			await m_localStorage.RemoveAsync(key);
		}

		public List<string> GetKeys()
		{
			return m_localStorage.GetValueAsync<List<string>>("_chars").Result;
		}

		public string LoadData(string key)
		{
			if (!GetKeys().Contains(key))
			{
				return "";
			}
			return m_localStorage.GetValueAsync<string>(key).Result;
		}

		public async void StoreData(string key, string data)
		{
			var storedChars = GetKeys();
			if (!storedChars.Contains(key))
			{
				storedChars.Add(key);
				await m_localStorage.SetValueAsync("_chars", storedChars);
			}
			await m_localStorage.SetValueAsync<string>(key, data);
		}
	}
}
