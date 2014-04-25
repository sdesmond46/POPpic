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
using POPpicLibrary;
using Buddy;

namespace POPpic
{
	public class MyTrophiesGalleryFragment : Fragment
	{
		public event EventHandler<AdapterView.ItemClickEventArgs> ImageSelected;

		IList<PicturePublic> viewModel;
		public MyTrophiesGalleryFragment(IList<Buddy.PicturePublic> viewModel) {
			this.viewModel = viewModel;
		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

		}

		View myView;
		GridView gridView;
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			if (myView == null) {

				myView = inflater.Inflate (Resource.Layout.MyTrophiesFragmentLayout, container, false);
				gridView = myView.FindViewById<GridView> (Resource.Id.gridView1);
				gridView.ItemClick += ImageSelected;

				var numColumns = 4;
				var columnWidth = gridView.Width / numColumns;

				var thumbnailAdapter = new AndroidMyTrophiesThumbnailAdapter (this.Activity, this.viewModel, columnWidth);
				gridView.Adapter = thumbnailAdapter;
				gridView.SetNumColumns (numColumns);

				this.Activity.SetProgressBarIndeterminateVisibility (false);
				//  gridView.SetColumnWidth (columnWidth);
			}

			return myView;
		}
	}
}

