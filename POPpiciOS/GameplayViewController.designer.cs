// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;

namespace POPpiciOS
{
	[Register ("GameplayViewController")]
	partial class GameplayViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		POPpiciOS.GameplayControlsOverlayView ControlsOverlay { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UIButton FinishRoundButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UILabel HintLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		POPpiciOS.GameplayControlsOverlayView OverlayView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UILabel PlayerOnePlayerNameLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UIImageView PlayerOneProfilePictureImage { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UILabel PlayerOneTimeElapsed { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UIView PlayerOneViewContainer { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UILabel PlayerTwoPlayerNameLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UIImageView PlayerTwoProfilePictureImage { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UILabel PlayerTwoTimeElapsed { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UIView PlayerTwoViewContainer { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (ControlsOverlay != null) {
				ControlsOverlay.Dispose ();
				ControlsOverlay = null;
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
			if (PlayerOneViewContainer != null) {
				PlayerOneViewContainer.Dispose ();
				PlayerOneViewContainer = null;
			}
			if (PlayerTwoPlayerNameLabel != null) {
				PlayerTwoPlayerNameLabel.Dispose ();
				PlayerTwoPlayerNameLabel = null;
			}
			if (PlayerTwoProfilePictureImage != null) {
				PlayerTwoProfilePictureImage.Dispose ();
				PlayerTwoProfilePictureImage = null;
			}
			if (PlayerTwoTimeElapsed != null) {
				PlayerTwoTimeElapsed.Dispose ();
				PlayerTwoTimeElapsed = null;
			}
			if (PlayerTwoViewContainer != null) {
				PlayerTwoViewContainer.Dispose ();
				PlayerTwoViewContainer = null;
			}
		}
	}
}
