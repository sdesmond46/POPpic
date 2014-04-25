using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using POPpicLibrary;
using System.Net;
using Android.Graphics;
using System.Threading.Tasks;
using AndroidHUD;

namespace POPpic
{
	public class GameSetupFragment : Fragment
	{
		public GameSetupFragment(FriendViewModel opponent) {
			this.viewModel = new GameSetupViewModel ();
			viewModel.Opponent = opponent;
		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);


		}

		View myView = null;
		GameSetupViewModel viewModel;
		Gallery balloonsGallery, backgroundsGallery;
		AndroidSelectGameImageryListAdapter balloonsAdapter, backgroundsAdapter;
		Button createGameButton;
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			if (myView == null) {

				myView = inflater.Inflate (Resource.Layout.GameSetupFragmentLayout, container, false);

				SetupFriendSection ();
				SetupGalleries ();
				createGameButton = myView.FindViewById<Button> (Resource.Id.button1);
				createGameButton.Click += HandleCreateGameClick;
			}

			return myView;
		}

		async void HandleCreateGameClick (object sender, EventArgs e)
		{
			if (createGameButton.Enabled) {
				AndHUD.Shared.Show (this.Activity, "Creating Game", -1, MaskType.Black);
				var balloonImage = this.balloonsAdapter [this.balloonsGallery.SelectedItemPosition];
				var backgroundImage = this.backgroundsAdapter [this.backgroundsGallery.SelectedItemPosition];
				var repository = await ((POPpicApplication)this.Activity.Application).GetGameRepository (this.Activity);
				var gameGuid = await repository.SendGameRequestAsync (balloonImage, backgroundImage, viewModel.Opponent.User);

				Intent returnIntent = new Intent ();
				returnIntent.PutExtra (SelectFriendViewModel.SelectedFriendKey, viewModel.Opponent.UserId);
				returnIntent.PutExtra (SelectFriendViewModel.NewGameGuidKey, gameGuid);
				this.Activity.SetResult (Result.Ok, returnIntent);
				this.Activity.Finish ();

				AndHUD.Shared.Dismiss ();
			}
		}

		void SetupGalleries() {
			balloonsGallery = myView.FindViewById<Gallery> (Resource.Id.gallery1);
			backgroundsGallery = myView.FindViewById<Gallery> (Resource.Id.gallery2);

			GameImageryRepository repository = new GameImageryRepository ();

			repository.GetBalloonImagesAsync ().ContinueWith (t => {
				Activity.RunOnUiThread(() => {
					this.viewModel.BalloonImages = t.Result;
					balloonsAdapter = new AndroidSelectGameImageryListAdapter (this.Activity, this.viewModel.BalloonImages, repository);
					balloonsGallery.Adapter = balloonsAdapter;
				});
			});

			repository.GetBackgroundImagesAsync ().ContinueWith (t => {
				Activity.RunOnUiThread(() => {
					this.viewModel.BackgroundImages = t.Result;
					backgroundsAdapter = new AndroidSelectGameImageryListAdapter (this.Activity, this.viewModel.BackgroundImages, repository);
					backgroundsGallery.Adapter = backgroundsAdapter;
				});
			});
		}

		void SetupFriendSection() {
			var item = viewModel.Opponent;
			var view = myView.FindViewById<RelativeLayout> (Resource.Id.friendListItem);
			view.FindViewById<TextView> (Resource.Id.textView1).Text = item.Name;
			view.FindViewById<TextView> (Resource.Id.textView2).Text = item.Record;
			var image = view.FindViewById<ImageView> (Resource.Id.imageView1);

			var webClient = new WebClient ();
			webClient.DownloadDataTaskAsync (item.ProfilePictureUri).ContinueWith (t => {
				if (!t.IsFaulted) {
					var bitmap = BitmapFactory.DecodeByteArray(t.Result, 0, t.Result.Length);
					image.SetImageBitmap(bitmap);
				}
			}, TaskScheduler.FromCurrentSynchronizationContext ());
		}

		// TODO USE THE REPOS
		void SetupMockBalloonImages() {
//			this.viewModel.BalloonImages.Add (GameImageryItemViewModel.Create ("Orange", "Orange Balloon", Resource.Drawable.balloon_orange, ImageryType.BALLOON));
//			this.viewModel.BalloonImages.Add (GameImageryItemViewModel.Create ("Blue", "Blue Balloon", Resource.Drawable.balloon_blue, ImageryType.BALLOON));
//			this.viewModel.BalloonImages.Add (GameImageryItemViewModel.Create ("Gray", "Gray Balloon", Resource.Drawable.balloon_gray, ImageryType.BALLOON));
//			this.viewModel.BalloonImages.Add (GameImageryItemViewModel.Create ("Green", "Green Balloon", Resource.Drawable.balloon_green, ImageryType.BALLOON));
//			this.viewModel.BalloonImages.Add (GameImageryItemViewModel.Create ("Orange", "Orange Balloon", Resource.Drawable.balloon_orange, ImageryType.BALLOON));
//
//			this.viewModel.BackgroundImages.Add(GameImageryItemViewModel.Create ("Rolling Hills", "Hills", Resource.Drawable.balloonbackground, ImageryType.BALLOON_BACKGROUND));
//			this.viewModel.BackgroundImages.Add(GameImageryItemViewModel.Create ("Outer space", "Hills", Resource.Drawable.outerspace, ImageryType.BALLOON_BACKGROUND));
		}
	}
}

