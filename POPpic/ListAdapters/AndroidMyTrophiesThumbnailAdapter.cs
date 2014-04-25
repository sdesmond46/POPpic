using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using POPpicLibrary;
using Buddy;
using System.Net;
using Android.Graphics;

namespace POPpic
{
	public class AndroidMyTrophiesThumbnailAdapter : BaseAdapter<Buddy.PicturePublic>
	{
		IList<Buddy.PicturePublic> myPictures;
		Activity context;
		public AndroidMyTrophiesThumbnailAdapter(Activity context, IList<Buddy.PicturePublic> pictures, int viewDimension) {
			this.context = context;
			this.myPictures = pictures;
			this.ViewDimension = viewDimension;
		}

		public int ViewDimension { get; set; }

		#region implemented abstract members of BaseAdapter
		public override long GetItemId (int position)
		{
			return position;
		}
		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			View view = this.context.LayoutInflater.Inflate(Resource.Layout.MyTrophiesGalleryThumbnailLayout, null);
			// view.LayoutParameters = new AbsListView.LayoutParams (this.ViewDimension, this.ViewDimension);
			var item = this.myPictures [position];
			var textView = view.FindViewById<TextView> (Resource.Id.textView1);
			var imageView = view.FindViewById<ImageView> (Resource.Id.imageView1);
			textView.Text = "";

			PopPicImageCache.GetPhotoAlbumImage(0, item.PhotoID, item.FullUrl).ContinueWith (t => { // TODO: Use the thumbnails instead
				if (!t.IsFaulted) {
					var bmp = AndroidUtilities.DecodeBitmapToSize(t.Result, 300);
					// var bmp = BitmapFactory.DecodeStream(t.Result);
					this.context.RunOnUiThread( () => imageView.SetImageBitmap(bmp));
				}
			});

			return view;
		}
		public override int Count {
			get {
				return this.myPictures.Count;
			}
		}
		#endregion
		#region implemented abstract members of BaseAdapter
		public override Buddy.PicturePublic this [int index] {
			get {
				return this.myPictures [index];
			}
		}
		#endregion
	}
}

