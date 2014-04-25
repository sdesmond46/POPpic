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

namespace POPpic
{
	public class SelectFriendSourceFragment : Fragment
	{
		public event EventHandler<FriendSourceViewModel> FriendSourceOptionSelected;

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
		}

		View myView = null;
		SelectFriendSourceViewModel viewModel;
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			myView = inflater.Inflate (Resource.Layout.SelectFriendSourceLayout, container, false);


			this.viewModel = new SelectFriendSourceViewModel ();
			this.viewModel.FriendSources.Add(FriendSourceViewModel.CreateModel("Facebook Friends", "play against your Facebook friends", Resource.Drawable.facebook_icon, FriendSourceId.FACEBOOK));

			var rootView = myView.FindViewById<LinearLayout> (Resource.Id.linearLayout1);
			var topList = Utility.CreateListView (this.viewModel.FriendSourceHeader, rootView, Resources, inflater);
			var bottomList = Utility.CreateListView (this.viewModel.RecentFriendsHeader, rootView, Resources, inflater);
			var friendSourceAdapter = new AndroidSelectFriendSourceListAdapter (this.Activity, this.viewModel.FriendSources);
			friendSourceAdapter.OptionSelected += this.FriendSourceOptionSelected;
			topList.Adapter = friendSourceAdapter;

			Utility.SetListViewHeightBasedOnChildren2 (topList);
			Utility.SetListViewHeightBasedOnChildren2 (bottomList);

			return myView;
		}


	}
}

