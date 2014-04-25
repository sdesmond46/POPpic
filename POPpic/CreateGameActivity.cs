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
using POPpic;

namespace POPpic
{
	[Activity (Label = "CreateGameActivity")]			
	public class CreateGameActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.CreateGameLayout);
			this.ActionBar.Title = "Create Game";

			var friendSourceFragment = new SelectFriendSourceFragment ();
			friendSourceFragment.FriendSourceOptionSelected -= HandleFriendSourceOptionSelected;
			friendSourceFragment.FriendSourceOptionSelected += HandleFriendSourceOptionSelected;
			DoTransaction (friendSourceFragment, FragmentTransit.FragmentOpen, false, "Select Opponent");
		}

		void HandleFriendSourceOptionSelected (object sender, FriendSourceViewModel e)
		{
			switch (e.SourceId) {
			case FriendSourceId.FACEBOOK:
				var selectFacebookFriendFragment = new SelectFacebookFriendFragment ();
				selectFacebookFriendFragment.FriendSelected -= HandleFriendSelected;
				selectFacebookFriendFragment.FriendSelected += HandleFriendSelected;
				DoTransaction (selectFacebookFriendFragment, FragmentTransit.FragmentOpen, true, "Choose Facebook Friend");
				break;
			case FriendSourceId.RANDOM:
				break;
			case FriendSourceId.RANDOM_NEARBY:
				break;
			}
		}

		private FriendViewModel currentOpponent = null;
		void HandleFriendSelected (object sender, FriendViewModel e)
		{
			currentOpponent = e;
			var setupFragment = new GameSetupFragment (currentOpponent);
			DoTransaction (setupFragment, FragmentTransit.EnterMask, true, "Create Game");
		}

		void DoTransaction(Fragment fragment, FragmentTransit transition, bool addToBackstack, string title) {
			FragmentTransaction tx = this.FragmentManager.BeginTransaction ();
			tx.SetTransition (transition);
			tx.Replace (Resource.Id.fragmentcontainer, fragment);
			if (addToBackstack) {
				tx.AddToBackStack (typeof(Fragment).ToString ());
			}

			this.ActionBar.Title = title;
			tx.Commit ();
		}
	}
}

