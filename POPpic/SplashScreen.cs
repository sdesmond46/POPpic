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
using System.Threading;
using System.Threading.Tasks;

namespace POPpic
{

	[Activity (Theme="@style/Theme.Splash"
		 , MainLauncher = true
			, NoHistory = true
	)]			
	public class SplashScreen : Activity
	{
		ProgressBar progressBar;
		TextView loadingStatus;
		Button reloadButton;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.SplashScreenLayout);

			progressBar = FindViewById<ProgressBar> (Resource.Id.progressBar1);
			loadingStatus = FindViewById<TextView> (Resource.Id.textView1);
			reloadButton = FindViewById<Button> (Resource.Id.button1);
			reloadButton.Click += HandleClick;

			LoadGameRepository ();
		}

		void HandleClick (object sender, EventArgs e)
		{
			LoadGameRepository ();
		}

		private void LoadGameRepository() 
		{
			progressBar.Visibility = ViewStates.Visible;
			loadingStatus.Text = "Loading Your Account";
			reloadButton.Visibility = ViewStates.Gone;

			((POPpicApplication)Application).GetGameRepository (this).ContinueWith (r => {
				if (r.IsFaulted || r.Result == null) {
					progressBar.Visibility = ViewStates.Gone;
					loadingStatus.Text = "Error loading account";
					reloadButton.Visibility = ViewStates.Visible;
					reloadButton.Text = "Reload";
				} else {
					// Intent intent = new Intent(this, typeof(MyGamesActivity));
					Intent intent = new Intent(this, typeof(MyGamesTabbedActivity));

					intent.AddFlags(ActivityFlags.ClearTop);
					intent.AddFlags(ActivityFlags.NewTask);
					StartActivity(intent);
				}
			}, TaskScheduler.FromCurrentSynchronizationContext());
		}
	}
}

