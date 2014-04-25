using System;
using System.IO;
using System.Threading.Tasks;
using System.Net;

namespace POPpicLibrary
{
	public static class PopPicImageCache
	{
		public static readonly string PhotoAlbumsPath = "PhotoAlbums";
		public static readonly string RootImagePath = Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData);

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
			if (File.Exists (fullPath) && (DateTime.Now - File.GetCreationTime (fullPath)) < cacheLength) {
				// Get the file from the cache
				var result = new TaskCompletionSource<Stream> ();
				FileStream fs = new FileStream (fullPath, FileMode.Open, FileAccess.Read);
				result.SetResult (fs);
				return result.Task;
			} else {
				return PopPicImageCache.DownloadStream (imageUrl, fullPath);
//						.ContinueWith<byte[]> (t => {
//					if (!t.IsFaulted) {
//						using (FileStream fileStream = new FileStream (fullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
//							fileStream.Write (t.Result, 0, t.Result.Length);
//						}
//						return t.Result;
//					} else {
//						return null;
//					}
//				});
			}
		}

		public static Task<Stream> DownloadStream (string url, string path)
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

//		public static Task<Stream> DownloadBytes (string url, string path)
//		{
//			WebClient downloader = new WebClient ();
//			downloader.DownloadFileTaskAsync (url, path).ContinueWith<Stream> (t => {
//				if (!t.IsFaulted) {
//					FileStream fs = new FileStream (path, FileMode.Open, FileAccess.Read);
//					return fs;
//				} else {
//					return null;
//				}
//			});
//
//			// return downloader.DownloadDataTaskAsync (url);
//		}
	}
}

