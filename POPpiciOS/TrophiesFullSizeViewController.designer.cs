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
	[Register ("TrophiesFullSizeViewController")]
	partial class TrophiesFullSizeViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIPageControl pageControl { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIScrollView scrollView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (pageControl != null) {
				pageControl.Dispose ();
				pageControl = null;
			}
			if (scrollView != null) {
				scrollView.Dispose ();
				scrollView = null;
			}
		}
	}
}
