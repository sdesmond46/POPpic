using System;
using System.Threading.Tasks;
using System.IO;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;
using POPpicLibrary;
using BigTed;

namespace POPpiciOS
{
	public static class IOSUtilities
	{
		public static void SetImageFromStream (Task<Stream> taskToAwait, UIImageView imageView, NSObject context)
		{
			taskToAwait.ContinueWith (t => {
				if (!t.IsFaulted && t.Result != null) {
					var data = NSData.FromStream (t.Result);
					context.InvokeOnMainThread (() => {
						var image = UIImage.LoadFromData (data);
						imageView.Image = image;
					});
				} else {
					Console.WriteLine (t.Exception.ToString ());
				}
			});
		}

		public static void SetImageFromStream (Task<Stream> taskToAwait, UIImageView imageView, NSObject context, int newWidth, int newHeight)
		{
			taskToAwait.ContinueWith (t => {
				if (!t.IsFaulted && t.Result != null) {
					var data = NSData.FromStream (t.Result);
					context.InvokeOnMainThread (() => {
						var image = UIImage.LoadFromData (data);
						if (image != null && imageView != null) {
							UIGraphics.BeginImageContext (new SizeF (newWidth, newHeight));
							image.Draw (new RectangleF (0, 0, newWidth, newHeight));
							image = UIGraphics.GetImageFromCurrentImageContext();
							UIGraphics.EndImageContext ();

							imageView.Image = image;
						}
					});
				} else {
					Console.WriteLine (t.Exception.ToString ());
				}
			});
		}

		public static void ShowSetupGameScreen(FriendViewModel friendModel, UINavigationController nav, NSObject context) {
			var viewModel = new GameSetupViewModel (AppDelegate.Repository);
			viewModel.Opponent = friendModel;
			BTProgressHUD.Show ("Loading", -1, ProgressHUD.MaskType.Black);
			viewModel.InitializeAsync ().ContinueWith (t => {
				context.InvokeOnMainThread(() => {
					BTProgressHUD.Dismiss();
					if (!t.IsFaulted && t.Result) {
						var controller = new GameSetupDialogViewController(viewModel);
						nav.PushViewController(controller, true);
					} else {
						// TODO error
					}
				});
			});
		}


	}
}

