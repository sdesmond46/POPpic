using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using POPpicLibrary;

namespace POPpiciOS
{
	public class TrophiesCollectionViewControllerCell : UICollectionViewCell
	{
		public static readonly NSString Key = new NSString ("TrophiesCollectionViewControllerCell");

		[Export ("initWithFrame:")]
		public TrophiesCollectionViewControllerCell (RectangleF frame) : base (frame)
		{
			SelectedBackgroundView = new UIView{BackgroundColor = UIColor.Green};

			this.imageView = new UIImageView (ContentView.Frame);
			this.imageView.ContentMode = UIViewContentMode.ScaleAspectFill;
			this.imageView.ClipsToBounds = true;
			ContentView.Add (imageView); 
		}


		UIImageView imageView;
		public override void PrepareForReuse ()
		{
			base.PrepareForReuse ();

			taskCancelled = true;
		}

		bool taskCancelled = false;
		public void SetCellData(Buddy.PicturePublic imageData)
		{
			taskCancelled = false;

			var downloadTask = PopPicImageCache.GetPhotoAlbumImage (0, imageData.PhotoID, imageData.FullUrl).ContinueWith (t => {
				if (!t.IsFaulted && t.Result != null && !taskCancelled) {
					return t.Result;
				} else {
					return null;
				}
			});

			IOSUtilities.SetImageFromStream (downloadTask,
				this.imageView,
				this);
		}
	}
}

