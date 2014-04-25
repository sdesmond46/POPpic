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
using System.Threading.Tasks;
using AndroidHUD;
using Android.Util;

namespace POPpic
{
	[Activity (Label = "MyGamesActivity")]			
	public class MyGamesActivity : Activity //, ActionMode.ICallback
	{
		//		public bool OnActionItemClicked (ActionMode mode, IMenuItem item)
		//		{
		//			switch (item.ItemId) {
		//			case Resource.Id.create_game:
		//				StartActivityForResult (typeof(SelectFriendActivity), 0);
		//				break;
		//			}
		//
		//			return true;
		//
		//		}
		//
		//		public bool OnCreateActionMode (ActionMode mode, IMenu menu)
		//		{
		//			MenuInflater inflater = mode.MenuInflater;
		//			inflater.Inflate(Resource.Menu.MyGamesMenu, menu);
		//			return true;
		//		}
		//
		//		public void OnDestroyActionMode (ActionMode mode)
		//		{
		//
		//		}
		//
		//		public bool OnPrepareActionMode (ActionMode mode, IMenu menu)
		//		{
		//			return false;
		//		}

		RelativeLayout loadingRelativeLayout;
		ListView myMoveList, theirMoveList, completedList;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			if (!RequestWindowFeature (WindowFeatures.ActionBar))
				throw new ArgumentException ();

			SetContentView (Resource.Layout.MyGamesLayout);

			loadingRelativeLayout = FindViewById<RelativeLayout> (Resource.Id.loadingRelativeLayout);
			loadingRelativeLayout.Visibility = ViewStates.Visible;

			this.ActionBar.NavigationMode = ActionBarNavigationMode.Standard;
			this.ActionBar.Title = "My Games";
			this.ActionBar.SetDisplayHomeAsUpEnabled (false);

			ReloadList ();

		}

//		protected ListView CreateListView (string header, LinearLayout rootView)
//		{
//			var listView = LayoutInflater.Inflate (Resource.Layout.MyGamesListView, null) as ListView;
//			var myGamesHeader = LayoutInflater.Inflate (Resource.Layout.GamesListDividerItem, null);
//			myGamesHeader.FindViewById<TextView> (Resource.Id.textView1).Text = header;
//			listView.AddHeaderView (myGamesHeader);
//
//			var rootView = FindViewById<LinearLayout> (Resource.Id.rootLinearLayout);
//			rootView.AddView (listView);
//			var margin = 10.0 * Resources.DisplayMetrics.Density;
//			((LinearLayout.LayoutParams)listView.LayoutParameters).SetMargins (0, 0, 0, (int)margin);
//
//			return listView;
//		}

		AndroidGameListAdapter myGamesAdapter, theirGamesAdapter, completedAdapter;

		protected void ReloadList (string gameGuidToStart = "")
		{
			loadingRelativeLayout.Visibility = ViewStates.Visible;
			var rootView = FindViewById<LinearLayout> (Resource.Id.rootLinearLayout);
			rootView.Visibility = ViewStates.Invisible;

			if (myMoveList == null) {


				// we need to create the lists
				myMoveList = Utility.CreateListView ("My Turn", rootView, Resources, LayoutInflater);
				theirMoveList = Utility.CreateListView ("Their Turn", rootView, Resources, LayoutInflater);
				completedList = Utility.CreateListView ("Completed", rootView, Resources, LayoutInflater);
			}

			RunOnUiThread (() => {
				// AndHUD.Shared.Show (this, "Loading Your Games", -1, MaskType.Black, null);
				((POPpicApplication)Application).GetGameRepository (this).ContinueWith (r => {

					this.viewModel = new MyGamesViewModel (r.Result);
					viewModel.InitializeAsync ().ContinueWith (t => {
						if (!t.IsFaulted && t.Result) {
							myGamesAdapter = new AndroidGameListAdapter (this, this.viewModel, MyGamesViewModel.ListType.MY_TURN); //, this);
							theirGamesAdapter = new AndroidGameListAdapter (this, this.viewModel, MyGamesViewModel.ListType.THEIR_TURN); //, this);
							completedAdapter = new AndroidGameListAdapter (this, this.viewModel, MyGamesViewModel.ListType.COMPLETED); //, this);
							myGamesAdapter.GameSelected += OnGameSelected;
							theirGamesAdapter.GameSelected += OnGameSelected;
							theirGamesAdapter.SeeAllClicked += OnSeeAllSelected;
							completedAdapter.GameSelected += OnGameSelected;
							completedAdapter.SeeAllClicked += OnSeeAllSelected;

							myGamesAdapter.CreateNewGameSelected += (object sender, bool e) => {
								StartCreateGameActivity();
							};

							myMoveList.Adapter = myGamesAdapter;
							theirMoveList.Adapter = theirGamesAdapter;
							completedList.Adapter = completedAdapter;

							Utility.SetListViewHeightBasedOnChildren2 (myMoveList);
							Utility.SetListViewHeightBasedOnChildren2 (theirMoveList);
							Utility.SetListViewHeightBasedOnChildren2 (completedList);
						} else {
							AndroidUtilities.ShowAlert(this, "Loading Games Failed", "Sorry, loading your games failed. Please try again later.");
						}

						if (!string.IsNullOrEmpty (gameGuidToStart)) {
							var gameToStart = this.viewModel.GetGameById (gameGuidToStart);
							if (gameToStart != null) {
								OnGameSelected (this, gameToStart);
							}
						}

						loadingRelativeLayout.Visibility = ViewStates.Gone;
						rootView.Visibility = ViewStates.Visible;

					}, TaskScheduler.FromCurrentSynchronizationContext());
				}, TaskScheduler.FromCurrentSynchronizationContext());
			});

		}

		private void OnSeeAllSelected (object sender, MyGamesViewModel.ListType e)
		{
			var adapter = (e == MyGamesViewModel.ListType.COMPLETED) ? this.completedAdapter : this.theirGamesAdapter;
			adapter.SetMaxItems (int.MaxValue);

			var listView = (e == MyGamesViewModel.ListType.COMPLETED) ? this.completedList : this.theirMoveList;
			Utility.SetListViewHeightBasedOnChildren2 (listView);
		}

		private MyGamesViewModel viewModel;

		public override bool OnPrepareOptionsMenu (IMenu menu)
		{
			if (menu.Size () == 0) {
				MenuInflater.Inflate (Resource.Menu.MyGamesMenu, menu);
			}
			return base.OnPrepareOptionsMenu (menu);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case Resource.Id.refresh_games:
				ReloadList ();
				return true;
			case Resource.Id.create_game:
				StartCreateGameActivity ();
				return true;
			case Resource.Id.view_trophies:
				StartActivity (typeof(MyTrophiesActivity));
				return true;
			}
			return base.OnOptionsItemSelected (item);
		}

		const int SELECT_FRIEND_ACTIVITY = 10;
		const int GAMEPLAY_ACTIVITY = 11;

		protected void StartCreateGameActivity() {
			StartActivityForResult (typeof(CreateGameActivity), SELECT_FRIEND_ACTIVITY);
			// StartActivityForResult (typeof(SelectFriendActivity), SELECT_FRIEND_ACTIVITY);
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			switch (requestCode) {
			case SELECT_FRIEND_ACTIVITY:
				if (resultCode == Result.Ok) {
					string newGameGuid = data.GetStringExtra (SelectFriendViewModel.NewGameGuidKey);
					ReloadList (newGameGuid);
				}
				break;
			case GAMEPLAY_ACTIVITY:
				if (resultCode == Result.Ok) {
					ReloadList ();
				}
				break;
			}
		}

		protected void OnGameSelected (object sender, GameViewModel gameModel)
		{
			Intent intent = new Intent (ApplicationContext, typeof(GameplayActivity));
			intent.PutExtra (GameModel.GetKey (), gameModel.Model.SerializeToJson ());
			StartActivityForResult (intent, GAMEPLAY_ACTIVITY);
		}
	}

	public class Utility
	{

		public static void SetListViewHeightBasedOnChildren2 (ListView listView)
		{
			try {
				var listAdapter = listView.Adapter;
				if (listAdapter == null) {
					return;
				}
				int desiredWidth = Android.Views.View.MeasureSpec.MakeMeasureSpec (listView.Width, MeasureSpecMode.AtMost);
				int totalHeight = 0;
				View view = null;
				for (int i = 0; i < listAdapter.Count; i++) {
					view = listAdapter.GetView (i, view, listView);
					if (i == 0) {
						view.LayoutParameters = new AbsListView.LayoutParams (desiredWidth, ViewGroup.LayoutParams.WrapContent);
					} else {
						view.LayoutParameters = new ViewGroup.LayoutParams (ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
					}

					view.Measure (desiredWidth, (int)MeasureSpecMode.Unspecified);
					totalHeight += view.MeasuredHeight;
				}
				ViewGroup.LayoutParams p = listView.LayoutParameters;
				p.Height = totalHeight + (listView.DividerHeight * (listAdapter.Count - 1));
				listView.LayoutParameters = p;
				listView.RequestLayout ();
			} catch (Exception e) {

			}
		}


		public static ListView CreateListView (string header, LinearLayout rootView, Android.Content.Res.Resources res, LayoutInflater inflater)
		{
			var listView = inflater.Inflate (Resource.Layout.MyGamesListView, null) as ListView;
			var myGamesHeader = inflater.Inflate (Resource.Layout.GamesListDividerItem, null);
			myGamesHeader.FindViewById<TextView> (Resource.Id.textView1).Text = header;
			listView.AddHeaderView (myGamesHeader);

			rootView.AddView (listView);
			var margin = 10.0 * res.DisplayMetrics.Density;
			((LinearLayout.LayoutParams)listView.LayoutParameters).SetMargins (0, 0, 0, (int)margin);

			return listView;
		}
	}
}

