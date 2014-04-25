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
using Newtonsoft.Json;

namespace POPpic
{
	[Activity (Label = "MyTrophiesActivity")]			
	public class MyTrophiesActivity : Activity
	{
		MyTrophiesViewModel viewModel;
		// IList<PicturePublic> items;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			RequestWindowFeature (WindowFeatures.IndeterminateProgress);
			SetContentView (Resource.Layout.MyTrophiesLayout);
			this.ActionBar.Title = "Loading Trophies";
			SetProgressBarIndeterminate (true);
			SetProgressBarIndeterminateVisibility (true);

			((POPpicApplication)Application).GetGameRepository (this).ContinueWith (r => {
				viewModel = new MyTrophiesViewModel(r.Result);
				viewModel.InitializeAsync().ContinueWith(t => {
					if (!t.IsFaulted) {
						var thumbnailsFragment = new MyTrophiesGalleryFragment(this.viewModel.MyBuddyPictures);
						thumbnailsFragment.ImageSelected += HandleImageSelected;
						this.DoTransaction(thumbnailsFragment, FragmentTransit.FragmentOpen, false, "My Trophies");
					} else {
						AndroidUtilities.ShowAlert(this, "Loading Failed", "Unable to load your trophies");
					}
				});
			});
		}

		void HandleImageSelected (object sender, AdapterView.ItemClickEventArgs e)
		{
			Intent intent = new Intent (this, typeof(MyTrophiesFullSizeGalleryActivity));
			intent.PutExtra (MyTrophiesFullSizeGalleryActivity.IMAGE_LIST_KEY, JsonConvert.SerializeObject(this.viewModel.MyPictures));
			intent.PutExtra (MyTrophiesFullSizeGalleryActivity.IMAGE_INDEX_KEY, e.Position);
			StartActivity (intent);
//			var fullsizeFragment = new MyTrophyFullSizeGalleryFragment (this.items, e.Position);
//			DoTransaction (fullsizeFragment, FragmentTransit.FragmentOpen, true, "HaHa what a loser");
		}

		void DoTransaction(Fragment fragment, FragmentTransit transition, bool addToBackstack, string title) {
			RunOnUiThread (() => {
				FragmentTransaction tx = this.FragmentManager.BeginTransaction ();
				tx.SetTransition (transition);
				tx.Replace (Resource.Id.fragmentcontainer, fragment);
				if (addToBackstack) {
					tx.AddToBackStack ("");
				}

				this.ActionBar.Title = title;
				tx.Commit ();
			});

		}
	}
}

