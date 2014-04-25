using System;
using Buddy;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace POPpicLibrary
{
	public class BuddyPictureModel
	{
		public BuddyPictureModel ()
		{
		}

		public BuddyPictureModel (PicturePublic picture) 
		{
			this.ImageId = picture.PhotoID;
			this.FullsizeUrl = picture.FullUrl;
			this.ThumbnailUrl = picture.ThumbnailUrl;
			this.AppTag = picture.AppTag;
		}

		public long ImageId { get; set;}
		public string ThumbnailUrl { get; set;}
		public string FullsizeUrl { get; set;}
		public string AppTag { get; set;}
	}

	public class MyTrophiesViewModel
	{
		public IList<PicturePublic> MyBuddyPictures { get; private set;}
		public IList<BuddyPictureModel> MyPictures { get; private set;}

		GameRepository repository;
		public MyTrophiesViewModel(GameRepository repository)
		{
			this.repository = repository;
		}

		public async Task<bool> InitializeAsync() {
			this.MyBuddyPictures = await this.repository.GetMyWinnerPicturesAsync ();
			this.MyPictures = this.MyBuddyPictures.Select (item => new BuddyPictureModel (item)).ToList();
			return true;
		}
	}
}

