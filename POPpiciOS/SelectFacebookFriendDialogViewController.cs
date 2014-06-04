
using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using POPpicLibrary;
using System.Diagnostics;

namespace POPpiciOS
{
	public partial class SelectFacebookFriendDialogViewController : DialogViewController
	{
		SelectFriendViewModel viewModel;
		public SelectFacebookFriendDialogViewController (SelectFriendViewModel viewModel) : base (UITableViewStyle.Plain, null, true)
		{
			this.viewModel = viewModel;
			this.EnableSearch = true;
			this.SearchPlaceholder = "Search";
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Debug.Assert (this.viewModel.IsInitialized);

			// Add all of the child items
			var localRoot = new RootElement (this.viewModel.PageTitle);
			localRoot.UnevenRows = true;
			var topSection = new Section ();
			localRoot.Add (topSection);

			foreach (var user in this.viewModel.Friends) {
				UIImage image = UIImage.FromBundle ("unknownUserImage_50.png");
				var element = new CreateGamePlayerCell (image, user.Name, "", () => {
					IOSUtilities.ShowSetupGameScreen(user, this.NavigationController, this);
				});
				element.ImageDownloadTask = PopPicImageCache.GetUserProfileImage (user.UserId, user.ProfilePictureUri.ToString());

				topSection.Add (element); 
			}

			this.Root = localRoot;
		}
	}
}
