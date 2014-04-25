using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Text;
using Android.Views;
using Android.Widget;
using Xamarin.FacebookBinding;
using Xamarin.FacebookBinding.Model;
using Xamarin.FacebookBinding.Widget;
using Java.Security;
using Android.Util;
using Buddy;
using Facebook;
using POPpicLibrary;
using Newtonsoft.Json;
using Xamarin.Auth;
using System.Linq;
using System.Threading.Tasks;
using Gcm.Client;

[assembly:Permission (Name = Android.Manifest.Permission.Internet)]
[assembly:Permission (Name = Android.Manifest.Permission.WriteExternalStorage)]
[assembly:MetaData ("com.facebook.sdk.ApplicationId", Value ="@string/app_id")]

namespace POPpic
{
	[Activity (Label = "@string/app_name", 
		// MainLauncher = true, 
		WindowSoftInputMode = SoftInput.AdjustResize)]
	public class HelloFacebookSampleActivity : FragmentActivity
	{
		public HelloFacebookSampleActivity ()
		{
			callback = new MyStatusCallback (this);
		}

		private LoginButton loginButton;
		private TextView greeting;
		private IGraphUser user;

		private UiLifecycleHelper uiHelper;

		class MyStatusCallback : Java.Lang.Object, Session.IStatusCallback
		{
			HelloFacebookSampleActivity owner;

			public MyStatusCallback (HelloFacebookSampleActivity owner)
			{
				this.owner = owner;
			}

			public void Call (Session session, SessionState state, Java.Lang.Exception exception)
			{
				owner.OnSessionStateChange (session, state, exception);
			}
		}

		private Session.IStatusCallback callback;

		class MyUserInfoChangedCallback : Java.Lang.Object, LoginButton.IUserInfoChangedCallback
		{
			HelloFacebookSampleActivity owner;

			public MyUserInfoChangedCallback (HelloFacebookSampleActivity owner)
			{
				this.owner = owner;
			}

			public void OnUserInfoFetched (IGraphUser user)
			{
				var oldUser = owner.user;
				owner.user = user;
				if (user != null) {
					if (oldUser == null || oldUser.Id != user.Id) {
						// Id's are different so we log in
						owner.AuthThroughBuddy ();
					}
				} else {
					// Statics.Repository = null;
				}
				owner.UpdateUI ();
			}
		}

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			uiHelper = new UiLifecycleHelper (this, callback);
			uiHelper.OnCreate (savedInstanceState);


			var info = PackageManager.GetPackageInfo("com.sammdesmond.popPIC", Android.Content.PM.PackageInfoFlags.Signatures);
			foreach (var signature in info.Signatures) {
				MessageDigest me = MessageDigest.GetInstance ("SHA");
				me.Update (signature.ToByteArray ());
				var s = Base64.EncodeToString (me.Digest(), Base64Flags.Default);
				Log.Error ("ssl key", s);
				// ShowAlert ("Danny ignore this", s);
			}

			SetContentView (Resource.Layout.loginPage);

			loginButton = (LoginButton)FindViewById (Resource.Id.login_button);
			loginButton.UserInfoChangedCallback = new MyUserInfoChangedCallback (this);
			loginButton.LoginBehavior = SessionLoginBehavior.SsoWithFallback;
			greeting = FindViewById<TextView> (Resource.Id.greeting);

			var activeSession = Session.OpenActiveSessionFromCache (this);
			if (activeSession != null && activeSession.IsOpened) {
				// Do somethign interesting!
			}
		}

		protected override void OnResume ()
		{
			base.OnResume ();
			uiHelper.OnResume ();

			UpdateUI ();
		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
			base.OnSaveInstanceState (outState);
			uiHelper.OnSaveInstanceState (outState);
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);
			uiHelper.OnActivityResult (requestCode, (int)resultCode, data);
		}

		protected override void OnPause ()
		{
			base.OnPause ();
			uiHelper.OnPause ();
		}

		protected override void OnDestroy ()
		{
			base.OnDestroy ();
			uiHelper.OnDestroy ();
		}

		Session session;

		private void OnSessionStateChange (Session session, SessionState state, Exception exception)
		{
			this.session = session;
			if ((exception is FacebookOperationCanceledException ||
				exception is FacebookAuthorizationException)) {
				new AlertDialog.Builder (this)
					.SetTitle (Resource.String.cancelled)
						.SetMessage (Resource.String.permission_not_granted)
						.SetPositiveButton (Resource.String.ok, (object sender, DialogClickEventArgs e) => {})
						.Show ();
			} else if (state == SessionState.OpenedTokenUpdated || state == SessionState.Opened) {
				if (session != null) {
					RunOnUiThread (() => {
						progressDialog = ProgressDialog.Show(this, "Loading Account", "Loading or creating your account", true);
					});
				}
				this.loginButton.SetSession (session);
			}

			UpdateUI ();
		}

		ProgressDialog progressDialog;

		private void UpdateUI ()
		{
			Session session = Session.ActiveSession;
			bool enableButtons = (session != null && session.IsOpened);
			greeting.Text = enableButtons ? "Welcome User" : "Not Logged In";
		}

		// My code starts here

		private const string AppId = "356773981130068";
		private const string AppSecret = "17207773a74d98f198e38ab0193d9a56";
		private const string BuddyAppName = "BalloonPopper";
		private const string BuddyAppKey = "19901ACB-EAB5-47E4-89A7-0C6A9B3A6E27";

		private void AuthThroughBuddy() {
			var authToken = Session.ActiveSession.AccessToken;
			var userId = this.user.Id;
			AuthThroughBuddy (userId, authToken);
		}

		private void AuthThroughBuddy(string fbUserId, string fbUserToken) {

			//Check to ensure everything's setup right
			GcmClient.CheckDevice(this);
			GcmClient.CheckManifest(this);
			GcmClient.Register(this, GcmBroadcastReceiver.SENDER_IDS);
			var registrationId = GcmClient.GetRegistrationId(this);

			FacebookClient fb = new FacebookClient (fbUserToken);
			fb.AppId = AppId;
			fb.AppSecret = AppSecret;

			BuddyClient client = new BuddyClient(BuddyAppName, BuddyAppKey);
			const string BuddyAccountKey = "BuddyAccount";
			const string BuddyAccessTokenKey = "AccessToken";
			const string FacebookIDTokenKey = "FacebookIdTokenKey";
			var accountStore = AccountStore.Create (this);
			var savedAccount = accountStore.FindAccountsForService (BuddyAccountKey).LastOrDefault ();
			Task<AuthenticatedUser> getUserTask;
			bool saveAccount = false;
			if (savedAccount != null && savedAccount.Properties.ContainsKey(BuddyAccessTokenKey)) {
				saveAccount = false;
				var token = savedAccount.Properties [BuddyAccessTokenKey];
				getUserTask = client.LoginAsync (token);
			} else {
				saveAccount = true;
				getUserTask = client.SocialLoginAsync("Facebook", fbUserId, fbUserToken).ContinueWith((u) => {
					if (u.IsFaulted) {
						// try again for kicks
					}

					return (AuthenticatedUser) u.Result;
				});
			}

			getUserTask.ContinueWith(r => {
				Console.WriteLine ("Get User Task has happened result is faulted = " + r.IsFaulted.ToString());

				if (!r.IsFaulted)
				{
					AuthenticatedUser user = r.Result;
					var successActivity = new Action(() => {


						var repository = new POPpicLibrary.GameRepository(user, client, fb);
						Console.WriteLine ("Success task is running!");

						if (saveAccount) {
							var properties = new Dictionary<string, string>();
							properties[BuddyAccessTokenKey] = user.Token;
							properties[FacebookIDTokenKey] = fbUserId;
							Account buddyAccount = new Account(user.ID.ToString(), properties);
							accountStore.Save(buddyAccount, BuddyAccountKey);
						}

						// Finish();

						if (progressDialog != null)
							progressDialog.Dismiss();

						((POPpicApplication)Application).SetGameRepository(repository);

					});

					if (string.IsNullOrEmpty(user.ApplicationTag)) {
						user.PhotoAlbums.CreateAsync(user.ID.ToString(), true, null).ContinueWith(pa => {
							if (!pa.IsFaulted) {
								var album = pa.Result;
								user.VirtualAlbums.CreateAsync(user.ID.ToString(), null).ContinueWith(va => {
									if (!va.IsFaulted) {
										var virtualAlbum = va.Result;
										var extraData = new UserExtraData();
										extraData.UploadAlbumId = album.AlbumId;
										extraData.WinnerAblumVirtualId = virtualAlbum.ID;
										user.UpdateAsync(null, null, UserGender.Any, 0, null, UserStatus.Any, false, false, JsonConvert.SerializeObject(extraData)).ContinueWith(updateResult => {
											if (!updateResult.IsFaulted && updateResult.Result) {
												RunOnUiThread(successActivity);
											}
										});
									}
								});
							}
						});
					} else {
						RunOnUiThread(successActivity);
					}

				} else {
					if (progressDialog != null)
						progressDialog.Dismiss();
					RunOnUiThread(() => {
						AndroidUtilities.ShowAlert(this, "Error Getting User Account", r.Exception.Message, "Try Again", (object sender, DialogClickEventArgs e) => {
							AuthThroughBuddy();
						});
					});

					Console.WriteLine(r.Exception.Message);
				}
			});
		}
	}
}
