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
	[Register ("TestTVC")]
	partial class TestTVC
	{
		[Outlet]
		MonoTouch.UIKit.UILabel LastMoveLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel TimeElapsedLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel UserNameLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView UserProfileImage { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (LastMoveLabel != null) {
				LastMoveLabel.Dispose ();
				LastMoveLabel = null;
			}

			if (TimeElapsedLabel != null) {
				TimeElapsedLabel.Dispose ();
				TimeElapsedLabel = null;
			}

			if (UserNameLabel != null) {
				UserNameLabel.Dispose ();
				UserNameLabel = null;
			}

			if (UserProfileImage != null) {
				UserProfileImage.Dispose ();
				UserProfileImage = null;
			}
		}
	}
}
