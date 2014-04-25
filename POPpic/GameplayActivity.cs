using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Webkit;
using System.Diagnostics;
using System.Timers;
using Android.Media;
// using Java.IO;
using POPpicLibrary;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using AndroidHUD;
using Android.Util;
using Android.Animation;
using Java.IO;

namespace POPpic
{
	[Activity (Label = "GameplayActivity"
		//	, MainLauncher = true
	)]
	public class GameplayActivity : Activity, TextureView.ISurfaceTextureListener, Android.Hardware.Camera.IPictureCallback
	{
		private TextView player1TimeText, player1NameText, player2TimeText, player2NameText, gameoverText, bottomSectionText;
		RelativeLayout balloonOverlayLayer, gameoverOverlayLayer;
		// ImageView balloonImage;
		ImageView gameOverImage;
		private Button finishRoundButton;
		private GameplayViewModel viewModel;
		private RelativeLayout balloonViewContainer;
		private BalloonBackgroundView balloonView;
		private bool overlayShowing = false;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			RequestWindowFeature (WindowFeatures.NoTitle);

			SetContentView (Resource.Layout.GameplayLayout);

			var gameData = Intent.GetStringExtra (GameModel.GetKey ());
			GameModel gameModel = GameModel.DeserializeFromJson (gameData);

			this.balloonOverlayLayer = FindViewById<RelativeLayout> (Resource.Id.balloonOverlay);
			this.bottomSectionText = balloonOverlayLayer.FindViewById<TextView>(Resource.Id.textView1);
			this.balloonViewContainer = this.balloonOverlayLayer.FindViewById<RelativeLayout> (Resource.Id.balloonContainerRelativeLayout);

			this.gameoverOverlayLayer = balloonOverlayLayer.FindViewById<RelativeLayout>(Resource.Id.gameOverOverlay);


			this.gameoverText = gameoverOverlayLayer.FindViewById<TextView> (Resource.Id.gameoverTextView);
			this.textureView = gameoverOverlayLayer.FindViewById<TextureView>(Resource.Id.textureView1);
			this.textureView.SurfaceTextureListener = this;


			AndHUD.Shared.Show (this, "Loading Game Data", -1, MaskType.Black);

			((POPpicApplication)Application).GetGameRepository (this).ContinueWith (r => {
				viewModel = new GameplayViewModel (r.Result, gameModel, Resources.DisplayMetrics.WidthPixels, Resources.DisplayMetrics.HeightPixels);
				viewModel.Tick += StopWatchTick;
				viewModel.InitializeAsync().ContinueWith((t) => {
					if (!t.IsFaulted) {
						this.player1TimeText = FindViewById<TextView> (Resource.Id.leftplayerTimeText);
						this.player1NameText = FindViewById<TextView> (Resource.Id.leftPlayerNameText);
						this.player1NameText.Text = this.viewModel.PlayerName1;

						this.player2TimeText = FindViewById<TextView> (Resource.Id.rightplayerTimeText);
						this.player2NameText = FindViewById<TextView> (Resource.Id.rightPlayerNameText);
						this.player2NameText.Text = this.viewModel.PlayerName2;

						this.finishRoundButton = balloonOverlayLayer.FindViewById<Button>(Resource.Id.button1);
						this.finishRoundButton.Text = this.viewModel.FinishTurnButtonText;

						this.finishRoundButton.Click += OnNextRoundClicked;

						// this.balloonImage.Touch += OnBalloonTouched;
						this.balloonView = new BalloonBackgroundView(this, this.viewModel);
						this.balloonView.BalloonTouchChanged += this.OnBalloonTouched;


//						
						Bitmap background;
						Bitmap balloon;
						if (this.viewModel.BackgroundImageStream != null && this.viewModel.BalloonImageStream != null) {
							background = BitmapFactory.DecodeStream(this.viewModel.BackgroundImageStream);
							balloon = BitmapFactory.DecodeStream(this.viewModel.BalloonImageStream);
						} else {
							background = BitmapFactory.DecodeResource (Resources, Resource.Drawable.balloonbackground);
							balloon = BitmapFactory.DecodeResource (Resources, Resource.Drawable.balloon);
						}


						var pop = BitmapFactory.DecodeResource(Resources, Resource.Drawable.popImage);
						balloonView.SetImages (background, balloon, pop);
						this.balloonViewContainer.AddView(this.balloonView);
						this.SetImage(Resource.Id.leftPlayerImage, this.viewModel.PlayerProfileImage1);
						this.SetImage(Resource.Id.rightPlayerImage, this.viewModel.PlayerProfileImage2);

						if (this.viewModel.LoserImgData != null) {
							Bitmap bmp = BitmapFactory.DecodeByteArray(this.viewModel.LoserImgData, 0, this.viewModel.LoserImgData.Length);
							SetResultImage(bmp);
							ShowOverlay();
						}

						this.bottomSectionText.Text = this.viewModel.BottomHintText;

						UpdateUI();
					} else {
						AndHUD.Shared.ShowError(this, "Error Loading Game", MaskType.Black, TimeSpan.FromSeconds(3));
						Finish();
					}

					AndHUD.Shared.Dismiss();
				}, TaskScheduler.FromCurrentSynchronizationContext ());
			}, TaskScheduler.FromCurrentSynchronizationContext ());


			inflationMediaPlayer = new MediaPlayer ();
			var fd = Application.Resources.OpenRawResourceFd (Resource.Raw.aria02);
			inflationMediaPlayer.SetDataSource (fd.FileDescriptor, fd.StartOffset, fd.Length);
			inflationMediaPlayer.SetAudioStreamType (Android.Media.Stream.System);
			inflationMediaPlayer.Prepare ();

			poppingMediaPlayer = new MediaPlayer ();
			var popFd = Application.Resources.OpenRawResourceFd (Resource.Raw.BalloonPop);
			poppingMediaPlayer.SetDataSource (popFd.FileDescriptor, popFd.StartOffset, popFd.Length);
			poppingMediaPlayer.SetAudioStreamType (Android.Media.Stream.System);
			poppingMediaPlayer.Prepare ();
		}

		private void SetImage(int imageViewId, string url) {
			var image = FindViewById<ImageView> (imageViewId);
			var webClient = new WebClient ();
			webClient.DownloadDataTaskAsync (url).ContinueWith (t => {
 				if (!t.IsFaulted) {
					var bitmap = BitmapFactory.DecodeByteArray(t.Result, 0, t.Result.Length);
					image.SetImageBitmap(bitmap);
				}
			}, TaskScheduler.FromCurrentSynchronizationContext ());
		}

		private void OnNextRoundClicked(object sender, EventArgs e) {
			if (this.viewModel.FinishTurnButtonEnabled) {
				AndHUD.Shared.Show(this, "Uploading Move", -1, MaskType.Black, null);
				GoToNextRoundAsync(null).ContinueWith(t => {
					AndHUD.Shared.Dismiss(this);
					if (t.IsFaulted) {
						AndHUD.Shared.ShowError(this, t.Exception.Message, MaskType.Black, TimeSpan.FromSeconds(3));
					} else {
						Intent returnIntent = new Intent ();
						returnIntent.PutExtra (GameModel.GetKey (), this.viewModel.model.SerializeToJson());
						SetResult (Result.Ok, returnIntent);
						OnBackPressed();
					}
				});
			} else {
				OnBackPressed ();
			}
		}

		private async Task<bool> GoToNextRoundAsync(System.IO.Stream stream = null) {
			return await this.viewModel.UpdateGameModelAsync (stream, "");
		}

		protected MediaPlayer inflationMediaPlayer, poppingMediaPlayer;

		private void StopWatchTick(object sender, bool popped) {
			UpdateUI ();
			if (popped) {
				RunOnUiThread (() => {
					PopBalloon();
				});
				this.TakePicture();
			}
		}

		private void UpdateUI() {
			this.RunOnUiThread (() => {
				this.player1TimeText.Text = this.viewModel.PlayerTime1;
				this.player2TimeText.Text = this.viewModel.PlayerTime2;
			});
		}


		private void PopBalloon() {
			// Make sure this only gets called once
			if (!overlayShowing) {
				this.viewModel.WatchStop ();
				this.inflationMediaPlayer.Pause ();
				overlayShowing = true;
				this.poppingMediaPlayer.Start ();
				this.balloonView.PopBalloonAsync ().ContinueWith((t) => {
					ShowOverlay();
				});
			}
		}

		private void ShowOverlay() {
			RunOnUiThread (() => {
				overlayShowing = true;
				gameoverOverlayLayer.Alpha = 1;
				this.bottomSectionText.Text = this.viewModel.BottomHintText;

				var animator = ObjectAnimator.OfFloat(gameoverOverlayLayer, "TranslationY", 1000, 0);
				animator.SetDuration(1000);
				animator.Start();
			});
		}

		protected void OnBalloonTouched(object sender, bool isTouched) {
			if (overlayShowing || !this.viewModel.IsCurrentUsersTurn) {
				return;
			}

			if (isTouched) {
				this.viewModel.WatchStart ();
				this.inflationMediaPlayer.Start ();
			} else {
				this.viewModel.WatchStop ();
				this.inflationMediaPlayer.Pause ();
			}
		}

		private int GetFrontCameraId()
		{
			//			int cameraCount = 0;
			//			Android.Hardware.Camera cam = null;
			//			Android.Hardware.Camera.CameraInfo cameraInfo = new Android.Hardware.Camera.CameraInfo();
			//			cameraCount = Android.Hardware.Camera.getNumberOfCameras();
			//			for ( int camIdx = 0; camIdx < cameraCount; camIdx++ ) {
			//				Android.Hardware.Camera.CameraInfo
			//				Android.Hardware.Camera.getCameraInfo( camIdx, cameraInfo );
			//				if ( cameraInfo.facing == Camera.CameraInfo.CAMERA_FACING_FRONT  ) {
			//					try {
			//						cam = Camera.open( camIdx );
			//					} catch (RuntimeException e) {
			//						Log.e(TAG, "Camera failed to open: " + e.getLocalizedMessage());
			//					}
			//				}
			//			}
			return 1;
		}

		Android.Hardware.Camera _camera;
		TextureView textureView;
		public void OnSurfaceTextureAvailable (SurfaceTexture surface, int width, int height)
		{

			try {
				_camera = Android.Hardware.Camera.Open(GetFrontCameraId());
				_camera.SetDisplayOrientation (90);

				var pictureSize = _camera.GetParameters ().PictureSize;
				var previewSize = _camera.GetParameters ().PreviewSize;

				// Need to flip around width and height because of the rotation
				textureView.LayoutParameters = new RelativeLayout.LayoutParams (pictureSize.Height, pictureSize.Width);
				var aspectRatio = ((double)pictureSize.Height) / ((double)pictureSize.Width);

				var lp = this.gameoverOverlayLayer.LayoutParameters;
				lp.Height = (int) (Resources.DisplayMetrics.HeightPixels * 0.6);
				lp.Width = (int) (lp.Height * aspectRatio);
				this.gameoverOverlayLayer.LayoutParameters = lp;

				_camera.SetPreviewTexture (surface);
				_camera.StartPreview ();

			}  catch (Java.IO.IOException ex) {
				System.Console.WriteLine (ex.Message);
			}
		}

		public bool OnSurfaceTextureDestroyed (SurfaceTexture surface)
		{
			_camera.StopPreview ();
			_camera.Release ();

			return true;
		}

		public void OnSurfaceTextureSizeChanged (SurfaceTexture surface, int width, int height)
		{
			// throw new NotImplementedException ();
		}

		public void OnSurfaceTextureUpdated (SurfaceTexture surface)
		{
			// throw new NotImplementedException ();
		}


		bool picInProgress = false;
		public void TakePicture()
		{
			lock (_camera) {
				if (picInProgress)
					return;
				_camera.TakePicture (null, null, this);
				picInProgress = true;
			}
		}

		private void SetResultImage(Bitmap bmp) {
			var resultImage = this.balloonOverlayLayer.FindViewById<ImageView>(Resource.Id.imageView1);
			textureView.Alpha = 0;
			resultImage.SetImageBitmap(bmp);
			resultImage.Invalidate();
		}

		public void OnPictureTaken (byte[] data, Android.Hardware.Camera camera)
		{
			RunOnUiThread(() => {
				// var bmp = BitmapFactory.DecodeFile (path);
				var bmp = BitmapFactory.DecodeByteArray(data, 0, data.Length);
				Matrix matrix = new Matrix();
				matrix.PostRotate(-90);
				matrix.PostScale(-1, 1);
				var rotatedBmp = Bitmap.CreateBitmap(bmp, 0, 0, bmp.Width, bmp.Height, matrix, true);

				// Save the file to the local disk
				var directory = new Java.IO.File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).AbsolutePath + "/popPIC");
				directory.Mkdirs();
				var file = Java.IO.File.CreateTempFile(this.viewModel.GameGUID, ".jpg", directory);
				var s = new FileStream(file.AbsolutePath, FileMode.Create);
				rotatedBmp.Compress(Bitmap.CompressFormat.Jpeg, 80, s);
				s.Seek(0, SeekOrigin.Begin);
				// s.Close();
//
//				using (var stream = OpenFileOutput(file.AbsolutePath, FileCreationMode.WorldReadable)) {
//					rotatedBmp.Compress(Bitmap.CompressFormat.Jpeg, 70, stream);
//				}

				SetResultImage(rotatedBmp);

				GoToNextRoundAsync(s).ContinueWith(t => {
					string message;
					if (t.IsFaulted) {
						message = "Error Uploading Photo";
					} else {
						message = "Finished Uploading Photo";
					}

					AndHUD.Shared.ShowToast(this, message, MaskType.Black, TimeSpan.FromSeconds(2));
				});
			});
		}

		public override void OnBackPressed() {
			this.Finish ();
			this.balloonView.CleanupBitmaps ();
			this.balloonView = null;
		}
	}
}

