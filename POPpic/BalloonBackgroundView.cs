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
using Android.Graphics;
using POPpicLibrary;
using System.Diagnostics;
using System.Threading.Tasks;

namespace POPpic
{
	public class BalloonBackgroundView : View
	{
		public void CleanupBitmaps() {
			var copy1 = this.backgroundBmp;
			var copy2 = this.balloonImage;
			var copy3 = this.popImage;
			popImage = balloonImage = backgroundBmp = null;

			copy1.Recycle ();
			copy2.Recycle ();
			copy3.Recycle ();
		}

		public BalloonBackgroundView (Context context) :
			base (context)
		{
			Initialize ();
		}

		public BalloonBackgroundView (Context context, IAttributeSet attrs) :
			base (context, attrs)
		{
			Initialize ();
		}

		public BalloonBackgroundView (Context context, IAttributeSet attrs, int defStyle) :
			base (context, attrs, defStyle)
		{
			Initialize ();
		}

		public BalloonBackgroundView(Context context, GameplayViewModel viewModel) :
			base (context)
		{
			this.viewModel = viewModel;
			Initialize ();
		}

		GameplayViewModel viewModel;

		void Initialize ()
		{
			// TODO: don't load these here
			// this.backgroundBmp = BitmapFactory.DecodeResource (this.Context.Resources, Resource.Drawable.balloonbackground_small);
			// this.balloonImage = BitmapFactory.DecodeResource (this.Context.Resources, Resource.Drawable.balloon);

			var height = this.viewModel.BalloonDimension;
			var width = this.viewModel.BalloonDimension;
			this.balloonRect = new RectF (0, 0, width, height);
			this.paint = new Paint (PaintFlags.AntiAlias);

			Invalidate ();
		}

		public void SetImages(Bitmap background, Bitmap balloon, Bitmap popImage)
		{
			this.backgroundBmp = background;
			this.balloonImage = balloon;
			this.popImage = popImage;
		}

		Bitmap backgroundBmp;
		Bitmap balloonImage;
		Bitmap popImage;
		Paint paint;

		RectF balloonRect;
		protected override void OnDraw(Canvas canvas)
		{
			this.canvasWidth = canvas.Width;
			this.canvasHeight = canvas.Height;

			if (this.backgroundBmp != null && this.balloonImage != null && this.viewModel != null) {
				try {
				this.viewModel.TickExternal ();

				RectF rect = new RectF (0, 0, canvas.Width, canvas.Height);
				canvas.DrawBitmap (this.backgroundBmp, null, rect, paint);

				Rect dirty = new Rect ();

				switch (this.state) {
				case AnimationState.BALLOON_SHOWING:
					this.balloonRect = GenerateBalloonRect (this.viewModel.BalloonPercentage);
					paint.Alpha = GetAlpha(viewModel.BalloonPercentage);
					canvas.DrawBitmap (this.balloonImage, null, balloonRect, paint);
						paint.Alpha = 255;
					this.balloonRect.RoundOut (dirty);
					Invalidate (dirty);
					break;
				case AnimationState.POPPING_SHOWING:
					var popRect = GeneratePopRect (this.popWatch, this.popDuration);
					canvas.DrawBitmap (this.popImage, null, popRect, paint);
					popRect.RoundOut (dirty);
					Invalidate (dirty);
					break;
				case AnimationState.POPPED_AND_DONE:
					break;
				}

				} catch {
				}
			}
		}

		private double canvasWidth, canvasHeight;
		private RectF GenerateBalloonRect(double percentage) {
			var balloonAspectRatio = ((double)this.balloonImage.Width) / ((double)this.balloonImage.Height);

			// percentage is the percentage of the height to make the balloon
			var height = percentage * this.canvasHeight;
			var width = height * balloonAspectRatio;

			var left = (this.canvasWidth - width) * 0.5;
			var right = left + width;
			var bottom = this.canvasHeight - (this.canvasHeight * 0.1);
			var top = bottom - height;


//			var top = (this.canvasHeight - height) * 0.5;
//			var left = (this.canvasWidth - width) * 0.5;
//			var bottom = top + height;
//			var right = left + width;


			return new RectF ((float) left, (float) top, (float) right, (float) bottom);
		}

		private int GetAlpha(double percentage) {
			const double minPercentage = 0.2;
			const double subtractionRate = 0.7;
			double alpha = 1 + (minPercentage * subtractionRate);
			alpha = alpha - (subtractionRate * percentage);
			return (int) (alpha * 255);
		}

		private RectF GeneratePopRect(Stopwatch watch, long popDuration) {
			var targetSize = this.balloonRect.Width ();
			var percentDone = ((double) watch.ElapsedMilliseconds) / ((double) popDuration);
			if (percentDone > 1) {
				balloonPoppingTask.Start ();
				this.state = AnimationState.POPPED_AND_DONE;
				return new RectF (0, 0, 0, 0);
			} else {
				// always want at least half of the pop to be showing
				var dimension = targetSize * (0.5 + (0.5 * percentDone));
				var left = (this.canvasWidth - dimension) * 0.5;
				var top = (this.canvasHeight - dimension) * 0.5;
				var right = left + dimension;
				var bottom = top + dimension;
				return new RectF((float) left, (float) top, (float) right, (float) bottom);
			}
		}

		public event EventHandler<bool> BalloonTouchChanged;

		public override bool OnTouchEvent(MotionEvent e) {
			if (e.ActionMasked == MotionEventActions.Down) {
				BalloonTouchChanged (this, true);
				// this.viewModel.WatchStart ();
			} else if (e.ActionMasked == MotionEventActions.Cancel ||
				e.ActionMasked == MotionEventActions.Outside ||
				e.ActionMasked == MotionEventActions.Up) {
				BalloonTouchChanged (this, false);
				// this.viewModel.WatchStop ();
			}

			return true;
		}

		bool popInProgress = false;
		Stopwatch popWatch = new Stopwatch();
		long popDuration = 200;
		public Task PopBalloonAsync() {
			if (popInProgress) {
				return balloonPoppingTask;
			}

			this.state = AnimationState.POPPING_SHOWING;
			popInProgress = true;
			popWatch.Start ();


			Vibrator vi;
			vi = this.Context.GetSystemService(Context.VibratorService) as Vibrator;
			if(vi != null && vi.HasVibrator){
				vi.Vibrate(100);
			}


			this.Invalidate ();

			balloonPoppingTask = new Task (() => {
			});

			return balloonPoppingTask;
		}

		Task balloonPoppingTask;

		AnimationState state = AnimationState.BALLOON_SHOWING;
		enum AnimationState {
			BALLOON_SHOWING,
			POPPING_SHOWING,
			POPPED_AND_DONE
		};
	}


//	[Activity (Label = "BalloonViewTestActivity"
//		//	, MainLauncher = true
//	)]
//	public class BalloonViewTestActivity : Activity
//	{
//		BalloonBackgroundView backgroundView;
//
//		protected override void OnCreate (Bundle bundle)
//		{
//			base.OnCreate (bundle);
//
//			GameplayViewModel viewModel = new GameplayViewModel (Statics.Repository, new GameModel (), Resources.DisplayMetrics.HeightPixels, Resources.DisplayMetrics.WidthPixels);
//			backgroundView = new BalloonBackgroundView (this, viewModel);
//			var background = BitmapFactory.DecodeResource (Resources, Resource.Drawable.balloonbackground);
//			var balloon = BitmapFactory.DecodeResource (Resources, Resource.Drawable.balloon);
//			backgroundView.SetImages (background, balloon, null);
//
//
//			this.SetContentView (backgroundView);
//		}
//	}
}

