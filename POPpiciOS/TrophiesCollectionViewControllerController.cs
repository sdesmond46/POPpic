using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using POPpicLibrary;
using FormsLibrary;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace POPpiciOS
{
	public class TrophiesCollectionViewControllerController : UICollectionViewController
	{
		public TrophiesCollectionViewControllerController (UICollectionViewLayout layout) : base (layout)
		{

		}

		bool navigatingToFullsize = false;
		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{

//			var controller = new TrophiesGalleryViewController ();
//
//			// var controller = App.GetMainPage().CreateViewController();
//			this.NavigationController.PushViewController (controller, true);
//
//
//			var viewController = AppDelegate.TrophiesFullSizeStoryboard.InstantiateViewController ("TrophiesFullSizeViewController")
//				as TrophiesFullSizeViewController;
//
			var viewController = new TrophiesGalleryViewController ();
			viewController.SetViewModel (this.viewModel, indexPath.Row);

			navigatingToFullsize = true;
			this.NavigationController.PushViewController (viewController, true);
		}



		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
		}

		private MyTrophiesViewModel viewModel;
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.TabBarController.TabBar.Hidden = true;

			// Register any custom UICollectionViewCell classes
			CollectionView.RegisterClassForCell (typeof(TrophiesCollectionViewControllerCell), TrophiesCollectionViewControllerCell.Key);

			this.viewModel = new MyTrophiesViewModel (AppDelegate.Repository);
			this.viewModel.InitializeAsync ().ContinueWith (t => {
				if (!t.IsFaulted && t.Result) {
					BeginInvokeOnMainThread(() => {
						CollectionView.ReloadData();
					});
				}
			});

		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			// this.TabBarController.TabBar.Hidden = false;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			this.TabBarController.TabBar.Hidden = true;
		}

		public override int NumberOfSections (UICollectionView collectionView)
		{
			return 1;

		}

		public override int GetItemsCount (UICollectionView collectionView, int section)
		{
			if (this.viewModel != null && this.viewModel.IsInitialized) {
				return this.viewModel.MyBuddyPictures.Count;
			} else {
				return 0;
			}
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = collectionView.DequeueReusableCell (TrophiesCollectionViewControllerCell.Key, indexPath) as TrophiesCollectionViewControllerCell;
			var frame = Layout.LayoutAttributesForItem (indexPath).Frame;
			var cellData = this.viewModel.MyBuddyPictures [indexPath.Row];
			cell.SetCellData (cellData);
			
			return cell;
		}
	}
}

