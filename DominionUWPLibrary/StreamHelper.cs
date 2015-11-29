using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace DominionUWP
{
	public static class StorageFolderExtensions
	{
		public static async Task<IStorageItem> TryGetItemAsync(this StorageFolder folder,
															   string name)
		{
			var files = await folder.GetItemsAsync().AsTask().ConfigureAwait(false);
			return files.FirstOrDefault(p => p.Name == name);
		}
	}
}
