using Microsoft.JSInterop;
using TheIdleScrolls_Core.Storage;

namespace TheIdleScrolls_Web.CoreWrapper
{
	public class LocalBrowserStorageHandler : IStorageHandler<string>
	{
		const string CharNameKey = "_chars";
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
			string chars = await m_localStorage.GetValueAsync<string>(CharNameKey);
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

			// Put name at the front of the character list => characters are ordered by recency of play
			if (storedChars.Count > 0 && storedChars.First() != key)
			{
				storedChars.Remove(key);
			}
			if (!storedChars.Contains(key))
			{
				storedChars.Insert(0, key);
				await m_localStorage.SetValueAsync(CharNameKey, String.Join(" ", storedChars));
			}

			await m_localStorage.SetValueAsync(key, data);
		}

		public async Task ExportData(string data)
		{
            await m_localStorage.DownloadText(data);
        }
	}
}
