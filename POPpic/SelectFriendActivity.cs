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

namespace POPpic
{
	[Activity (Label = "SelectFriendActivity")]			
	public class SelectFriendActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.SelectFriendLayout);

			this.ActionBar.NavigationMode = ActionBarNavigationMode.Standard;
			this.ActionBar.Title = "Select Opponent";
			this.ActionBar.SetDisplayHomeAsUpEnabled (true);

			var listView = FindViewById<ListView> (Resource.Id.listView1);
			AndHUD.Shared.Show(this, "Loading Friends", -1, MaskType.Black, null);

			((POPpicApplication)Application).GetGameRepository (this).ContinueWith (r => {
				this.viewModel = new SelectFriendViewModel (r.Result);
				viewModel.InitializeAsync ().ContinueWith (t => {
					if (!t.IsFaulted && t.Result) {
						var listAdapter = new AndroidFriendListAdapter(this, viewModel);
						listAdapter.FriendSelected += OnFriendSelected;
						listView.Adapter = listAdapter;
					}

					AndHUD.Shared.Dismiss();
				}, TaskScheduler.FromCurrentSynchronizationContext ());
			}, TaskScheduler.FromCurrentSynchronizationContext ());
		}

		public void OnFriendSelected(object sender, FriendViewModel friend) {
			Intent returnIntent = new Intent ();
			returnIntent.PutExtra (SelectFriendViewModel.SelectedFriendKey, friend.UserId);
			SetResult (Result.Ok, returnIntent);
			Finish ();
		}

		private SelectFriendViewModel viewModel;
	}
}

