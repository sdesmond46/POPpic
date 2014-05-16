using System;
using System.IO;
using System.Threading.Tasks;
using System.Net;

namespace POPpicLibrary
{
	public static class PopPicImageCache
	{
		public static readonly string PhotoAlbumsPath = "PhotoAlbums";
#if __ANDROID__
		public static readonly string RootImagePath = Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData);
#endif

#if __IOS__
		// public static readonly string RootImagePath = "./Library/Caches";
		public static readonly string RootImagePath = Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData);
#endif

		public static Task<Stream> GetPhotoAlbumImage (long albumId, long photoId, string imageUrl)
		{
			return GetPhotoAlbumImage (albumId, photoId, imageUrl, TimeSpan.MaxValue);
		}

		public static Task<Stream> GetPhotoAlbumImage (long albumId, long photoId, string imageUrl, TimeSpan cacheLength)
		{
			string fileName = new Uri (imageUrl).LocalPath;
			var path = RootImagePath + "/" + albumId.ToString () + "/" + photoId.ToString ();
			Directory.CreateDirectory (path);
			var fullPath = path + fileName;
			return RetrieveImageOrCache (imageUrl, fullPath, cacheLength);
		}

		public static Task<Stream> GetUserProfileImage(int userId, string imageUrl) {
			return GetUserProfileImage (userId, imageUrl, TimeSpan.FromMinutes (5));
		}

		public static Task<Stream> GetUserProfileImage(int userId, string imageUrl, TimeSpan cacheLength) {
			var path = RootImagePath + "/" + userId.ToString ();
			return GetImageAsync (imageUrl, path, cacheLength);
		}

		private static Task<Stream> GetImageAsync(string imageUrl, string path, TimeSpan cacheLength) {
			string fileName = new Uri (imageUrl).LocalPath;
			Directory.CreateDirectory (path);
			var fullPath = path + fileName;
			return RetrieveImageOrCache (imageUrl, fullPath, cacheLength);
		}

		private static Task<Stream> RetrieveImageOrCache(string imageUrl, string fullPath, TimeSpan cacheLength) {
			if (File.Exists (fullPath) && (DateTime.Now - File.GetCreationTime (fullPath)) < cacheLength) {
				// Get the file from the cache
				var result = new TaskCompletionSource<Stream> ();
				FileStream fs = new FileStream (fullPath, FileMode.Open, FileAccess.Read);
				result.SetResult (fs);
				return result.Task;
			} else {
				WebClient downloader = new WebClient ();
				return downloader.DownloadDataTaskAsync(imageUrl).ContinueWith<Stream> (t => {
					if (t.IsFaulted)
						throw t.Exception;

					var result = new MemoryStream(t.Result);

					using (var file = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
					{
						result.CopyTo(file);
					}

					result.Seek(0, SeekOrigin.Begin);
					return result;
				});
			}
		}

		private static Task<Stream> DownloadStream (string url, string path)
		{
			WebClient downloader = new WebClient ();

			return downloader.DownloadFileTaskAsync (url, path).ContinueWith<Stream> (t => {
				if (!t.IsFaulted) {
					FileStream fs = new FileStream (path, FileMode.Open, FileAccess.Read);
					return fs;
				} else {
					return null;
				}
			});
		}
	}
}

