using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Support.V4.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Buddy;
using Android.App;
using POPpicLibrary;
using Android.Support.V4.View;
using Android.Graphics;
using Newtonsoft.Json;

namespace POPpic
{
	[Activity (Label = "MyTrophiesFullSizeGalleryActivity")]			
	public class MyTrophiesFullSizeGalleryActivity : Android.Support.V4.App.FragmentActivity
	{
		public static readonly string IMAGE_INDEX_KEY = "ImageIndexKey";
		public static readonly string IMAGE_LIST_KEY = "ImageListKey";

		IList<BuddyPictureModel> items;
		ViewPager pager;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// parse out the images and current index
			var imagesString = Intent.GetStringExtra (IMAGE_LIST_KEY);
			this.items = JsonConvert.DeserializeObject<IList<BuddyPictureModel>> (imagesString);
			var currentIndex = Intent.GetIntExtra(IMAGE_INDEX_KEY, 0);

			pager = new ViewPager(this);
			pager.Id = Resource.Id.linearLayout1; // not really a linear layout
			SetContentView (pager);

			AndroidMyTrophyFullSizePagerAdapter pagerAdapter = new AndroidMyTrophyFullSizePagerAdapter(this.items, this.SupportFragmentManager);
			pager.Adapter = pagerAdapter;
			pager.SetCurrentItem (currentIndex, false);
//
//			((POPpicApplication)Application).GetGameRepository (this).ContinueWith (r => {
//				r.Result.GetMyWinnerPicturesAsync().ContinueWith(t => {
//					if (!t.IsFaulted) {
//						this.items = t.Result;
//						AndroidMyTrophyFullSizePagerAdapter pagerAdapter = new AndroidMyTrophyFullSizePagerAdapter(this.items, this.SupportFragmentManager);
//						RunOnUiThread(() => {
//							pager.Adapter = pagerAdapter;
//							pager.SetCurrentItem(currentIndex, false);
//						});
//					} else {
//						AndroidUtilities.ShowAlert(this, "Loading Failed", "Unable to load your trophies");
//					}
//				});
//			});
		}
	}


	public class AndroidMyTrophyFullSizePagerAdapter : FragmentStatePagerAdapter   {

		IList<BuddyPictureModel> items;
		Dictionary<int, FullSizeImageFragment> fragments = new Dictionary<int, FullSizeImageFragment>();
		public AndroidMyTrophyFullSizePagerAdapter(IList<BuddyPictureModel> viewModel, Android.Support.V4.App.FragmentManager fm) : base(fm) {
			this.items = viewModel;
		}

		public override int Count {
			get {
				return items == null ? 0 : this.items.Count;
			}
		}

		public override Android.Support.V4.App.Fragment GetItem (int position)
		{
			return new FullSizeImageFragment (this.items [position]);

//			if (!fragments.ContainsKey (position)) {
//				fragments [position] = new FullSizeImageFragment (this.items [position]);
//			}
//
//			var fragment = fragments [position];
//			return fragment;
		}

	}

	public class FullSizeImageFragment : Android.Support.V4.App.Fragment {
		BuddyPictureModel picture;
		public FullSizeImageFragment(BuddyPictureModel picture) {
			this.picture = picture;
		}

		View myView;
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			if (myView == null) {
				myView = inflater.Inflate (Resource.Layout.MyTrophiesFullSizeImageLayout, container, false);
				var imageView = myView.FindViewById<ImageView> (Resource.Id.imageView1);



				POPpicLibrary.PopPicImageCache.GetPhotoAlbumImage (0, picture.ImageId, picture.FullsizeUrl).ContinueWith ((t) => {
					if (!t.IsFaulted) {
						var bmp = BitmapFactory.DecodeStream(t.Result);
						this.Activity.RunOnUiThread(() => imageView.SetImageBitmap(bmp));
					}
				});
			}

			return myView;
		}
	}
}

