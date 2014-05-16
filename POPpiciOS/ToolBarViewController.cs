using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace POPpiciOS
{
	public partial class ToolBarViewController : UIViewController
	{
		public ToolBarViewController () : base ("ToolBarViewController", null)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		UIToolbar toolbar;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.Title = "Programmatic Toolbar";

			// set the background color of the view to white
			this.View.BackgroundColor = UIColor.White;

			// new up the toolbar
			float toolbarHeight = 44;
			toolbar = new UIToolbar (new RectangleF (0
				, this.View.Frame.Height - 60
				, this.View.Frame.Width, toolbarHeight));
			toolbar.AutoresizingMask = UIViewAutoresizing.FlexibleTopMargin | UIViewAutoresizing.FlexibleWidth;


			// button one
			string buttonTitle = "One";
			UIBarButtonItem btnOne = new UIBarButtonItem (buttonTitle, UIBarButtonItemStyle.Bordered, null);
			btnOne.Clicked += (s, e) => { new UIAlertView ("click!", "btnOne clicked", null, "OK", null).Show (); };

			// fixed width
			UIBarButtonItem fixedWidth = new UIBarButtonItem (UIBarButtonSystemItem.FixedSpace);
			fixedWidth.Width = 25;

			// button two 
			UIBarButtonItem btnTwo = new UIBarButtonItem ("second", UIBarButtonItemStyle.Bordered, null);

			// flexible width space
			UIBarButtonItem flexibleWidth = new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace);

			// button three
			UIBarButtonItem btnThree = new UIBarButtonItem ("3", UIBarButtonItemStyle.Bordered, null);

			// button four
			UIBarButtonItem btnFour = new UIBarButtonItem ("another!", UIBarButtonItemStyle.Bordered, null);

			// create the items array
			UIBarButtonItem[] items = new UIBarButtonItem[] { 
				btnOne, fixedWidth, btnTwo, flexibleWidth, btnThree, btnFour };

			// add the items to the toolbar
			toolbar.SetItems (items, false);			

			// add the toolbar to the page
			this.View = toolbar;
			// this.View.AddSubview (toolbar);
			// Perform any additional setup after loading the view, typically from a nib.
		}
	}
}

