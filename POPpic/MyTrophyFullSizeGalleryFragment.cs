using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Buddy;
using POPpicLibrary;
using Android.Graphics;

namespace POPpic
//
{
//	public class MyTrophyFullSizeGalleryFragment : Fragment
//	{
//		IList<Buddy.PicturePublic> viewModel;
//		int startIndex;
//		AndroidMyTrophyFullSizePagerAdapter pagerAdapter;
//		public MyTrophyFullSizeGalleryFragment(IList<Buddy.PicturePublic> viewModel, int index) {
//			this.viewModel = viewModel;
//
//			pagerAdapter = new AndroidMyTrophyFullSizePagerAdapter (viewModel, this.Activity.FragmentManager);
//			pagerAdapter.SetItems (viewModel);
//			this.startIndex = index;
//		}
//
//		ViewPager myView;
//
//		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
//		{
//			if (myView == null) {
//				myView = new ViewPager(this.Activity);
//				myView.Id = Resource.Id.linearLayout1; // not really a linear layout
//				myView.Adapter = pagerAdapter;
//				myView.SetCurrentItem (this.startIndex, false);
//			}
//
//			return myView;
//		}
//	}
//
//	public class AndroidMyTrophyFullSizePagerAdapter : FragmentStatePagerAdapter   {
//
//		IList<Buddy.PicturePublic> items;
//		Dictionary<int, FullSizeImageFragment> fragments = new Dictionary<int, FullSizeImageFragment>();
//		public AndroidMyTrophyFullSizePagerAdapter(IList<Buddy.PicturePublic> viewModel, Android.Support.V4.App.FragmentManager fm) : base(fm) {
//			this.items = viewModel;
//		}
//
//		public void SetItems(IList<Buddy.PicturePublic> viewModel) {
//			this.items = viewModel;
//		}
//
//		public override int Count {
//			get {
//				return items == null ? 0 : this.items.Count;
//			}
//		}
//
//		public override Android.Support.V4.App.Fragment GetItem (int position)
//		{
//			if (!fragments.ContainsKey (position)) {
//				fragments [position] = new FullSizeImageFragment (this.items [position]);
//			}
//
//			var fragment = fragments [position];
//			return fragment;
//		}
//
//	}
//
//	public class FullSizeImageFragment : Android.Support.V4.App.Fragment {
//		PicturePublic picture;
//		public FullSizeImageFragment(PicturePublic picture) {
//			this.picture = picture;
//		}
//
//		View myView;
//		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
//		{
//			if (myView == null) {
//				myView = inflater.Inflate (Resource.Layout.MyTrophiesFullSizeImageLayout, container, false);
//				var imageView = myView.FindViewById<ImageView> (Resource.Id.imageView1);
//				POPpicImageCache.GetPhotoAlbumImage (0, picture.PhotoID, picture.FullUrl).ContinueWith ((t) => {
//					if (!t.IsFaulted) {
//						var bmp = BitmapFactory.DecodeStream(t.Result);
//						imageView.SetImageBitmap(bmp);
//					}
//				});
//			}
//
//			return myView;
//		}
//	}
}

