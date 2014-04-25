using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using Newtonsoft.Json;
using WebRole2.Models;

namespace POPpicLibrary
{
	public class GameImageryRepository
	{
		public GameImageryRepository ()
		{
		}

		static readonly string balloonImageUrlFormat = "http://poppicwebapp.cloudapp.net:8081/api/imageproduct?type={0}";
		public async Task<IList<GameImageryItemViewModel>> GetBalloonImagesAsync() 
		{
			var results = await GetImagesAsync (ImageryType.BALLOON);
			return results;
		}

		public async Task<IList<GameImageryItemViewModel>> GetBackgroundImagesAsync() 
		{
			var results = await GetImagesAsync (ImageryType.BALLOON_BACKGROUND);
			return results;
		}

		private async Task<IList<GameImageryItemViewModel>> GetImagesAsync(ImageryType imageType)
		{
			WebClient client = new WebClient ();
			string downloadUrl = string.Format (balloonImageUrlFormat, Enum.GetName (typeof(ImageryType), imageType));
			var dataString = await client.DownloadStringTaskAsync (downloadUrl);
			var images = JsonConvert.DeserializeObject<IEnumerable<ImageProduct>> (dataString);

			return images.Select (image => {
				return new GameImageryItemViewModel(image);
			}).ToList();
		}

		public async Task<Stream> GetImageAsync(string url) {
			WebClient client = new WebClient ();
			var result = await client.DownloadDataTaskAsync (url);
			Stream stream = new MemoryStream (result);
			return stream;
		}
	}
}

