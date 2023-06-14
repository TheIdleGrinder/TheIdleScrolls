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

		public async Task DeleteData(string key)
		{
			await m_localStorage.RemoveAsync(key);
		}

		public async Task<List<string>> GetKeys()
		{
			string chars = await m_localStorage.GetValueAsync<string>("_chars");
			return chars.Split().ToList();
		}

		public async Task<string> LoadData(string key)
		{
			var keys = await GetKeys();
			if (!keys.Contains(key))
			{
				return "";
			}
			return await m_localStorage.GetValueAsync<string>(key);
		}

		public async Task StoreData(string key, string data)
		{
			var storedChars = await GetKeys();
			if (!storedChars.Contains(key))
			{
				storedChars.Add(key);
				await m_localStorage.SetValueAsync("_chars", storedChars);
			}
			await m_localStorage.SetValueAsync<string>(key, data);
		}
	}
}
