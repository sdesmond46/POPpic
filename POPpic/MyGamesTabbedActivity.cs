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

namespace POPpic
{
	[Activity (Label = "MyGamesTabbedActivity"
		//, MainLauncher = true
	)]			
	public class MyGamesTabbedActivity : Activity, ActionBar.ITabListener
	{
		const int SELECT_FRIEND_ACTIVITY = 10;
		const int GAMEPLAY_ACTIVITY = 11;

		GameListFragment myTurnFragment, theirTurnFragment, completedFragment;
		AndroidGameListAdapter myTurnAdapter, theirTurnAdapter, completedAdapter;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			this.RequestWindowFeature (WindowFeatures.Progress);
			this.RequestWindowFeature (WindowFeatures.IndeterminateProgress);
			SetProgressBarIndeterminate (true);
			SetProgressBarVisibility (true);

			this.ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
			this.ActionBar.Title = "My Games";
			this.ActionBar.SetDisplayHomeAsUpEnabled (false);
			SetContentView (Resource.Layout.MyTrophiesLayout);

			myTurnFragment = new GameListFragment ();
			theirTurnFragment = new GameListFragment ();
			completedFragment = new GameListFragment ();

			ReloadList ();

			SetupTab ("My Turn", this.myTurnAdapter, this.myTurnFragment);
			SetupTab ("Their Turn", this.theirTurnAdapter, this.theirTurnFragment);
			SetupTab ("Completed", this.completedAdapter, this.completedFragment);
		}

		void SetupTab(string title, IListAdapter adapter, GameListFragment fragment) {
			var tab = this.ActionBar.NewTab ();
			tab.SetTabListener (this);
			tab.SetText (title);
			this.ActionBar.AddTab (tab);
		}

		public void OnTabReselected (ActionBar.Tab tab, FragmentTransaction ft)
		{
			// Nothing
		}

		Fragment GetFragmentForPosition(int position) {
			Fragment fragment = null;
			switch (position) {
			case 0:
				fragment = this.myTurnFragment;
				break;
			case 1:
				fragment = this.theirTurnFragment;
				break;
			case 2:
				fragment = this.completedFragment;
				break;
			}

			return fragment;
		}

		public void OnTabSelected (ActionBar.Tab tab, FragmentTransaction ft)
		{
			var fragment = GetFragmentForPosition (tab.Position);
			if (fragment.IsDetached) {
				ft.Attach (fragment);
			} else {
				ft.Add (Resource.Id.fragmentcontainer, fragment, "");
			}
		}

		public void OnTabUnselected (ActionBar.Tab tab, FragmentTransaction ft)
		{
			var fragment = GetFragmentForPosition (tab.Position);
			ft.Detach (fragment);
		}

		MyGamesViewModel viewModel;
		protected void ReloadList (string gameGuidToStart = "")
		{
			((POPpicApplication)Application).GetGameRepository (this).ContinueWith (r => {
				this.viewModel = new MyGamesViewModel (r.Result);
				viewModel.InitializeAsync ().ContinueWith (t => {
					RunOnUiThread(() => {
						if (!t.IsFaulted && t.Result) {
							this.myTurnAdapter = new AndroidGameListAdapter(this, this.viewModel, MyGamesViewModel.ListType.MY_TURN);
							this.theirTurnAdapter = new AndroidGameListAdapter(this, this.viewModel, MyGamesViewModel.ListType.THEIR_TURN);
							this.completedAdapter = new AndroidGameListAdapter(this, this.viewModel, MyGamesViewModel.ListType.COMPLETED);

							this.myTurnFragment.ListAdapter = this.myTurnAdapter;
							this.theirTurnFragment.ListAdapter = this.theirTurnAdapter;
							this.completedFragment.ListAdapter = this.completedAdapter;

							this.myTurnAdapter.GameSelected += this.OnGameSelected;
							this.theirTurnAdapter.GameSelected += this.OnGameSelected;
							this.completedAdapter.GameSelected += this.OnGameSelected;
						} else {
							// TODO Show error
						}

						SetProgressBarVisibility (false);
					});
				});
			});
		}

		protected void StartCreateGameActivity() {
			StartActivityForResult (typeof(CreateGameActivity), SELECT_FRIEND_ACTIVITY);
		}

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
				return false;
			case Resource.Id.create_game:
				StartCreateGameActivity ();
				return false;
			case Resource.Id.view_trophies:
				StartActivity (typeof(MyTrophiesActivity));
				return false;
			}
			return base.OnOptionsItemSelected (item);
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


	class GameListFragment : ListFragment
	{           
		public event EventHandler<GameViewModel> GameSelected;

		public override View OnCreateView (LayoutInflater inflater,
			ViewGroup container, Bundle savedInstanceState)
		{
			var myView = base.OnCreateView (inflater, container, savedInstanceState);
			return myView;
		}

		public override void OnListItemClick (ListView l, View v, int position, long id)
		{
			var adapter = this.ListAdapter as AndroidGameListAdapter;
			if (adapter != null) {
				var item = adapter [position];
				if (item != null && this.GameSelected != null) {
					this.GameSelected (this, item);
				}
			}
		}
	}
}

