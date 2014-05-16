// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace POPpiciOS
{
	[Register ("GameplayViewController")]
	partial class GameplayViewController
	{
		[Outlet]
		POPpiciOS.GameplayControlsOverlayView ControlsOverlay { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton FinishRoundButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel HintLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView OverlayView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel PlayerOnePlayerNameLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView PlayerOneProfilePictureImage { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel PlayerOneTimeElapsed { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView PlayerOneViewContainer { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel PlayerTwoPlayerNameLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView PlayerTwoProfilePictureImage { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel PlayerTwoTimeElapsed { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView PlayerTwoViewContainer { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ControlsOverlay != null) {
				ControlsOverlay.Dispose ();
				ControlsOverlay = null;
			}

			if (PlayerTwoTimeElapsed != null) {
				PlayerTwoTimeElapsed.Dispose ();
				PlayerTwoTimeElapsed = null;
			}

			if (PlayerTwoPlayerNameLabel != null) {
				PlayerTwoPlayerNameLabel.Dispose ();
				PlayerTwoPlayerNameLabel = null;
			}

			if (FinishRoundButton != null) {
				FinishRoundButton.Dispose ();
				FinishRoundButton = null;
			}

			if (HintLabel != null) {
				HintLabel.Dispose ();
				HintLabel = null;
			}

			if (OverlayView != null) {
				OverlayView.Dispose ();
				OverlayView = null;
			}

			if (PlayerTwoProfilePictureImage != null) {
				PlayerTwoProfilePictureImage.Dispose ();
				PlayerTwoProfilePictureImage = null;
			}

			if (PlayerOnePlayerNameLabel != null) {
				PlayerOnePlayerNameLabel.Dispose ();
				PlayerOnePlayerNameLabel = null;
			}

			if (PlayerOneProfilePictureImage != null) {
				PlayerOneProfilePictureImage.Dispose ();
				PlayerOneProfilePictureImage = null;
			}

			if (PlayerOneTimeElapsed != null) {
				PlayerOneTimeElapsed.Dispose ();
				PlayerOneTimeElapsed = null;
			}

			if (PlayerTwoViewContainer != null) {
				PlayerTwoViewContainer.Dispose ();
				PlayerTwoViewContainer = null;
			}

			if (PlayerOneViewContainer != null) {
				PlayerOneViewContainer.Dispose ();
				PlayerOneViewContainer = null;
			}
		}
	}
}
