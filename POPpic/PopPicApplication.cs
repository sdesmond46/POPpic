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
using Xamarin.FacebookBinding;
using Xamarin.Auth;
using System.Threading.Tasks;
using Facebook;
using Buddy;

namespace POPpic
{
	#if DEBUG
	[Application(Debuggable=true)]
	#else
	[Application(Debuggable=false)] // TODO Change this
	#endif
	class POPpicApplication : Android.App.Application
	{

		public POPpicApplication(IntPtr handle, JniHandleOwnership transfer)
			: base(handle,transfer)
		{
			repositoryLock = new object ();
			newRepositoryEvent = new System.Threading.ManualResetEvent (false);
			// do any initialisation you want here (for example initialising properties)
		}

		public GameRepository RawGameRepository { get { return this.repository; } }

		public Task<GameRepository> GetGameRepository(Activity context) {
			lock (this.repositoryLock) {
				// TEMP test
				return GetRepositoryHelper (context);

//				if (this.repository == null) {
//					return GetRepositoryHelper (context);
//				} else {
//					var completion = new TaskCompletionSource<GameRepository> ();
//					completion.SetResult (this.repository);
//					return completion.Task;
//				}
			}
		}

		public void SetGameRepository(GameRepository repository) {
			lock (this.repositoryLock) {
				this.repository = repository;
				this.newRepositoryEvent.Set ();
			}
		}

		private object repositoryLock;
		private GameRepository repository;

		private Task<GameRepository> GetRepositoryHelper(Activity context) {
			// First try to get the game repository from saved settings
			var fbSession = Session.OpenActiveSessionFromCache (this);
			var accountStore = AccountStore.Create (this);
			var savedBuddyAccount = accountStore.FindAccountsForService (PopPicConstants.BuddyAccountKey).LastOrDefault ();

			if (fbSession != null && savedBuddyAccount != null && fbSession.IsOpened) {
				// This is the case where we have a cached 
				FacebookClient fb = new FacebookClient (fbSession.AccessToken);
				fb.AppId = PopPicConstants.AppId;
				fb.AppSecret = PopPicConstants.AppSecret;

				var buddyToken = savedBuddyAccount.Properties [PopPicConstants.BuddyAccessTokenKey];
				BuddyClient buddyClient = new BuddyClient (PopPicConstants.BuddyAppName, PopPicConstants.BuddyAppKey);
				return buddyClient.LoginAsync (buddyToken).ContinueWith (result => {
					if (!result.IsFaulted) {
						var authenticatedUser = result.Result;
						var repository = new GameRepository (authenticatedUser, buddyClient, fb);
						return repository;
					} else {
						throw result.Exception;
					}
				});
			} else {
				this.newRepositoryEvent.Reset ();
				Intent startIntent = new Intent (context, typeof(HelloFacebookSampleActivity));
			    startIntent.AddFlags(ActivityFlags.NewTask);
				// startIntent.AddFlags (ActivityFlags.NoHistory);
				context.StartActivityForResult (startIntent, 1234);

				return Task.Factory.StartNew(() => { 
					this.newRepositoryEvent.WaitOne ();
					return this.repository;
				});
			}
		}

		System.Threading.ManualResetEvent newRepositoryEvent; 

	}
}

