using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using MonoTouch.FacebookConnect;
using System.Drawing;
using POPpicLibrary;
using Xamarin.Auth;
using Buddy;
using Facebook;
using System.Threading;
using Xamarin.Forms;

namespace POPpiciOS
{
	public interface IRepositoryChangedListener {
		void RepositoryChanged (GameRepository repository);
	}

	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// Get your own App ID at developers.facebook.com/apps
		public const string FacebookAppId = "356773981130068";
		public const string DisplayName = "POPpic";

		public override bool OpenUrl (UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
		{
			// We need to handle URLs by passing them to FBSession in order for SSO authentication
			// to work.
			return FBSession.ActiveSession.HandleOpenURL(url);

		}

		public override void OnActivated (UIApplication application)
		{
			// We need to properly handle activation of the application with regards to SSO
			// (e.g., returning from iOS 6.0 authorization dialog or from fast app switching).
			FBSession.ActiveSession.HandleDidBecomeActive();
		}

		private static object repositoryLock = new object();
		private static GameRepository repository = null;
		public static GameRepository Repository { 
			get { 
				lock (repositoryLock) {
					return repository;
				}
			}
			set {
				lock (repositoryLock) {
					repository = value;
				}
			}
		}

		public static UIStoryboard GameplayStoryboard = UIStoryboard.FromName ("GameplayStoryboard", null);
		public static UIStoryboard Storyboard = UIStoryboard.FromName ("MyGamesStoryboard", null);
		public static UIStoryboard SplashScreenStoryboard = UIStoryboard.FromName ("SplashScreenStoryboard", null);
		public static UIStoryboard TrophiesFullSizeStoryboard = UIStoryboard.FromName ("TrophiesFullSizeStoryboard", null);

		// class-level declarations
		UIWindow window;
		//
		// This method is invoked when the application has loaded and is ready to run. In this
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// window = new UIWindow (UIScreen.MainScreen.Bounds);

			Forms.Init ();

			FBSettings.DefaultAppID = FacebookAppId;
			FBSettings.DefaultDisplayName = DisplayName;

			window = new UIWindow (UIScreen.MainScreen.Bounds);
			UIViewController initialController;

			// Try to open up the cached fb access token
			var accountStore = AccountStore.Create ();
			var savedBuddyAccount = accountStore.FindAccountsForService (PopPicConstants.BuddyAccountKey).LastOrDefault ();

			if (savedBuddyAccount != null &&
			    FBSession.OpenActiveSession (false) &&
			    FBSession.ActiveSession.IsOpen) {
				var buddyToken = savedBuddyAccount.Properties [PopPicConstants.BuddyAccessTokenKey];
				BuddyClient buddyClient = new BuddyClient (PopPicConstants.BuddyAppName, PopPicConstants.BuddyAppKey);
				FacebookClient fb = new FacebookClient (FBSession.ActiveSession.AccessTokenData.AccessToken);
				fb.AppId = PopPicConstants.AppId;
				fb.AppSecret = PopPicConstants.AppSecret;

				initialController = SplashScreenStoryboard.InstantiateViewController ("SplashScreenViewController") as UIViewController;

				buddyClient.LoginAsync (buddyToken).ContinueWith (t => {
					if (!t.IsFaulted) {
						var authenticatedUser = t.Result;
						var repository = new GameRepository (authenticatedUser, buddyClient, fb);
						AppDelegate.Repository = repository;

						InvokeOnMainThread(() => {
							var newController = Storyboard.InstantiateViewController("MyGamesTabBarController") as UIViewController;
							newController.ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal;
							initialController.PresentViewController(newController, true, null);
						});
					} else {
						// TODO: error
					}
				});


			} else {
				initialController = Storyboard.InstantiateViewController("MyGamesTabBarController") as UIViewController;
				// initialController = GameplayStoryboard.InstantiateViewController("GameplayViewController") as GameplayViewController;

			}


			window.RootViewController = initialController;
			window.MakeKeyAndVisible ();

			return true;
		}

	}
}

