using System;
using MonoTouch.UIKit;
using POPpicLibrary;
using System.Collections.Generic;
using System.Drawing;

namespace POPpiciOS
{
	public class LockedScrollView : UIScrollView
	{
		public LockedScrollView(RectangleF frame) : base(frame)
		{
		}

	}

	public class TrophiesGalleryViewController : UIViewController
	{
		public TrophiesGalleryViewController () : base ()
		{
		}

		UIScrollView scrollView;
		UIPageControl pageControl;
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			var topPadding = this.NavigationController.NavigationBar.Frame.Y + this.NavigationController.NavigationBar.Frame.Height;
			var scrollFrame = new RectangleF (0, topPadding, this.View.Frame.Width, this.View.Frame.Height - topPadding);
			this.scrollView = new UIScrollView (scrollFrame);
			this.scrollView.ContentInset = new UIEdgeInsets (0, 0, 0, 0);
			this.scrollView.AutoresizingMask = UIViewAutoresizing.FlexibleBottomMargin;
			this.scrollView.ShowsHorizontalScrollIndicator = this.scrollView.ShowsVerticalScrollIndicator = false;
			this.View.AddSubview (scrollView);
			this.pageControl = new UIPageControl (new RectangleF (0, 400, 320, 100));
			// this.View.AddSubview (pageControl);

			this.TabBarController.TabBar.Hidden = true;
			this.scrollView.PagingEnabled = true;

			this.pageControl.Pages = this.viewModel.MyBuddyPictures.Count;
			this.pageControl.CurrentPage = this.currentIndex;
			this.scrollView.Scrolled += (object sender, EventArgs e) => {
				Console.WriteLine("Content Offset y=" + this.scrollView.ContentOffset.Y);
				var offset = this.scrollView.ContentOffset;
				offset.Y = 0;
				this.scrollView.ContentOffset = offset;
				LoadVisiblePages ();
			};

			var pageScrollViewSize = this.scrollView.Frame.Size;
			this.scrollView.ContentSize = new System.Drawing.SizeF (pageScrollViewSize.Width * this.pageControl.Pages, pageScrollViewSize.Height);

			var myFrame = this.scrollView.Frame;
			var visibleFrame = new RectangleF (myFrame.Width * this.currentIndex, myFrame.Y, myFrame.Width, myFrame.Height);
			this.scrollView.ScrollRectToVisible (visibleFrame, false);
			LoadVisiblePages ();
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			var myFrame = this.scrollView.Frame;
			var visibleFrame = new RectangleF (myFrame.Width * this.currentIndex, myFrame.Y, myFrame.Width, myFrame.Height);
			this.scrollView.ScrollRectToVisible (visibleFrame, false);
		}

		private void LoadPage(int index)
		{
			if (index < 0 || index >= this.pageControl.Pages) {
				// If it's outside the range of what you have to display, then do nothing
				return;
			}

			UIImageView pageView;
			if (this.pages.ContainsKey (index)) {
				pageView = this.pages [index];
			} else {
				var myFrame = this.scrollView.Frame;
				RectangleF frame = new RectangleF (myFrame.Width * index, 0, myFrame.Width, myFrame.Height);
				pageView = new UIImageView (frame);
				pageView.ContentMode = UIViewContentMode.ScaleAspectFit;

				var imageItem = this.viewModel.MyBuddyPictures [index];
				IOSUtilities.SetImageFromStream (PopPicImageCache.GetPhotoAlbumImage (0, imageItem.PhotoID, imageItem.FullUrl),
					pageView,
					this);

				this.scrollView.AddSubview (pageView);
			}

			this.pages [index] = pageView;
		}

		private void PurgePage(int index)
		{
			if (index < 0 || index >= this.pageControl.Pages) {
				// If it's outside the range of what you have to display, then do nothing
				return;
			}

			if (this.pages.ContainsKey (index)) {
				var pageView = this.pages [index];
				pageView.RemoveFromSuperview ();
				this.pages.Remove (index);
			}
		}

		private void LoadVisiblePages()
		{
			var pageWidth = this.scrollView.Frame.Width;
			int page = (int)Math.Floor (((this.scrollView.ContentOffset.X * 2.0f) + pageWidth) / (pageWidth * 2.0f));
			this.pageControl.CurrentPage = page;

			int firstPage = page - 1;
			int lastPage = page + 1;

			for (int i = firstPage - 1; i >= 0; i--) {
				PurgePage (i);
			}

			for (int i = lastPage + 1; i < this.pageControl.Pages; i++) {
				PurgePage (i);
			}

			for (int i = firstPage; i <= lastPage; i++) {
				LoadPage (i);
			}

		}

		MyTrophiesViewModel viewModel = null;
		int currentIndex = -1;
		Dictionary<int, UIImageView> pages = new Dictionary<int, UIImageView> ();
		public void SetViewModel(MyTrophiesViewModel viewModel, int currentIndex) {
			this.viewModel = viewModel;
			this.currentIndex = currentIndex;
		}
	}
}

