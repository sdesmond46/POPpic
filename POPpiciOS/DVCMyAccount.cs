using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using MonoTouch.FacebookConnect;
using System.Drawing;
using Buddy;
using POPpicLibrary;
using Facebook;

namespace POPpiciOS
{
	[Register ("DVCMyAccount")]
	public partial class DVCMyAccount : DialogViewController
	{
		public event EventHandler<GameRepository> RepositoryLoaded;

		public DVCMyAccount () : base (UITableViewStyle.Grouped, null)
		{
			InitializeView ();
		}

		public DVCMyAccount (IntPtr p) : base (p)
		{
			InitializeView ();
		}

		public bool ReturnAfterLoading { get; set; }

		private void InitializeView ()
		{

			// Create the Facebook LogIn View with the needed Permissions
			// https://developers.facebook.com/ios/login-ui-control/
			var loginView = new FBLoginView () {
				Frame = new RectangleF (85, 0, 151, 43)
			};
			
			// Hook up to FetchedUserInfo event, so you know when
			// you have the user information available
			
			bool didFetchUserInfo = false;
			loginView.FetchedUserInfo += (sender, e) => {
				if (didFetchUserInfo)
					return;
				didFetchUserInfo = true;

				Console.WriteLine ("User is logged in ID=" + e.User.GetId () + ", User name is " + e.User.GetName ());
				FacebookClient fb = new FacebookClient (FBSession.ActiveSession.AccessTokenData.AccessToken);
				fb.AppId = PopPicConstants.AppId;
				fb.AppSecret = PopPicConstants.AppSecret;

				BuddyClient buddyClient = new BuddyClient (PopPicConstants.BuddyAppName, PopPicConstants.BuddyAppKey);
				buddyClient.SocialLoginAsync("Facebook", e.User.GetId(), FBSession.ActiveSession.AccessTokenData.AccessToken).ContinueWith(result => {
					var authenticatedUser = result.Result;
					var repository = new GameRepository(authenticatedUser, buddyClient, fb);
					AppDelegate.Repository = repository;
					InvokeOnMainThread(() => {
						if (this.ReturnAfterLoading) {
							this.NavigationController.PopViewControllerAnimated(true);
						}

						if (this.RepositoryLoaded != null) {
							this.RepositoryLoaded(this, repository);
						}
					});

				});

			};
			
			// Clean user Picture and label when Logged Out
			loginView.ShowingLoggedOutUser += (sender, e) => {
			};
			
			Root = new RootElement ("Log In") {
				new Section ("Please Sign In") {
					new UIViewElement ("", loginView, false) {
						Flags = UIViewElement.CellFlags.DisableSelection | UIViewElement.CellFlags.Transparent,
					}
				}
			};
		}
	}
}
