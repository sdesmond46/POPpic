
using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using POPpicLibrary;
using System.Diagnostics;
using BigTed;

namespace POPpiciOS
{
	public partial class GameSetupDialogViewController : DialogViewController
	{
		GameSetupViewModel viewModel;
		public GameSetupDialogViewController (GameSetupViewModel viewModel) : base (UITableViewStyle.Grouped, null, true)
		{
			this.viewModel = viewModel;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Debug.Assert (this.viewModel.IsInitialized);

			UIBarButtonItem startGameButton = new UIBarButtonItem ("Start", UIBarButtonItemStyle.Done, null);
			// startGameButton.Style = UIBarButtonItemStyle.Done;
			startGameButton.Clicked += (object sender, EventArgs e) => {
				BTProgressHUD.Show ("Creating Game", -1, ProgressHUD.MaskType.Black);
				this.viewModel.CreateNewGame(this.View.Frame.Height, this.View.Frame.Width).ContinueWith(t => {
					this.InvokeOnMainThread(() => {
						BTProgressHUD.Dismiss();
						if (!t.IsFaulted && t.Result != null) {
							var gameplayViewController = AppDelegate.GameplayStoryboard.InstantiateViewController("GameplayViewController") as GameplayViewController;
							gameplayViewController.SetGameplayViewModel(t.Result);

							// Need to pop off the set up stack
							var navController = this.NavigationController;
							var targetController = this.NavigationController.ViewControllers.Where(vc => vc.GetType() == typeof(GameListTableViewController)).FirstOrDefault();
							if (targetController != null)
								this.NavigationController.PopToViewController(targetController, false);

							navController.PushViewController(gameplayViewController, true);
						} else {
							// TODO Error
						}
					});
				});
			};

			this.NavigationItem.RightBarButtonItems = new UIBarButtonItem[] {
				startGameButton
			};

			var localRoot = new RootElement ("New Game");
			var opponentSection = new Section ("Opponent");
			localRoot.Add (opponentSection);

			UIImage image = UIImage.FromBundle ("unknownUserImage_50.png");
			var opponentElement = new CreateGamePlayerCell (image, this.viewModel.Opponent.Name, "", () => {
				// TODO - allow the user to change who the opponent is
			});
			opponentElement.ImageDownloadTask = PopPicImageCache.GetUserProfileImage (this.viewModel.Opponent.UserId, this.viewModel.Opponent.ProfilePictureUri.ToString());
			opponentSection.Add (opponentElement);

			this.Root = localRoot;
		}
	}
}
