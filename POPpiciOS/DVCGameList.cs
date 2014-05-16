using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using System.Drawing;
using POPpicLibrary;

namespace POPpiciOS
{
	[Register ("DVCGameList")]
	public partial class DVCGameList : DialogViewController
	{
		private RootElement rootElement;

		public DVCGameList(IntPtr input) : base(input) {
			Root = rootElement = new RootElement ("WOOOOO") {
				new Section ("First Section") {
					new StringElement ("Hello", () => {
						new UIAlertView ("Hola", "Thanks for tapping!", null, "Continue").Show (); 
					}),
					new EntryElement ("Name", "Enter your name", String.Empty)
				},
				new Section ("Second Section") {
				},
				new Section () {
					new ActivityElement()
				}
			};
		}

		POPpicLibrary.MyGamesViewModel.ListType listType;
		public void InitializeList(MyGamesViewModel.ListType gameType) 
		{
			this.listType = gameType;

			rootElement.Caption = "Game type is " + this.listType.ToString ();
			rootElement.Caption = "WOOOOOOO";
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var addButton = new UIBarButtonItem (UIBarButtonSystemItem.Add);
			addButton.Clicked += (s, ev) =>  Console.WriteLine("Button Clicked!");
			var trophyButton = new UIBarButtonItem (UIBarButtonSystemItem.Action);
			trophyButton.Clicked += (s, ev) => {

			};

			this.NavigationItem.RightBarButtonItems = new UIBarButtonItem[] {
				addButton,
				trophyButton
			};


		}
	}
}
