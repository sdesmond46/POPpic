using System.Text;
using Android.App;
using Android.Content;
using Android.Util;


//VERY VERY VERY IMPORTANT NOTE!!!!
// Your package name MUST NOT start with an uppercase letter.
// Android does not allow permissions to start with an upper case letter
// If it does you will get a very cryptic error in logcat and it will not be obvious why you are crying!
// So please, for the love of all that is kind on this earth, use a LOWERCASE first letter in your Package Name!!!!

using System.Diagnostics;
using System.Collections.Generic;
using System;
using Android.OS;
using Gcm.Client;
using POPpicLibrary;
using System.Timers;
using System.Threading.Tasks;
using Android.Support.V4.App;
using Newtonsoft.Json;
using System.Net;
using Android.Graphics;


[assembly: Permission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "com.google.android.c2dm.permission.RECEIVE")]

//GET_ACCOUNTS is only needed for android versions 4.0.3 and below
[assembly: UsesPermission(Name = "android.permission.GET_ACCOUNTS")]
[assembly: UsesPermission(Name = "android.permission.INTERNET")]
[assembly: UsesPermission(Name = "android.permission.WAKE_LOCK")]

namespace POPpic
{
	[BroadcastReceiver(Permission=Constants.PERMISSION_GCM_INTENTS)]
	[IntentFilter(new string[] { Constants.INTENT_FROM_GCM_MESSAGE }, 
		Categories = new string[] { "@PACKAGE_NAME@" })]
	[IntentFilter(new string[] { Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK }, 
		Categories = new string[] { "@PACKAGE_NAME@" })]
	[IntentFilter(new string[] { Constants.INTENT_FROM_GCM_LIBRARY_RETRY }, 
		Categories = new string[] { "@PACKAGE_NAME@" })]
	public class GcmBroadcastReceiver : GcmBroadcastReceiverBase<GcmService>
	{
		//IMPORTANT: Change this to your own Sender ID!
		//The SENDER_ID is your Google API Console App Project Number
		public static string[] SENDER_IDS = new string[] {"549522234657", "324229631029"};
	}

	[Service] //Must use the service tag
	public class GcmService : GcmServiceBase
	{
		public GcmService() : base(GcmBroadcastReceiver.SENDER_IDS) { }

		protected override void OnRegistered (Context context, string registrationId)
		{
			//Receive registration Id for sending GCM Push Notifications to
			Task.Delay (TimeSpan.FromSeconds (10)).ContinueWith ((timer) => {

				if (((POPpicApplication)Application).RawGameRepository != null) {
					((POPpicApplication)Application).RawGameRepository.RegisterAndroidPushAsync (registrationId).ContinueWith ((t) => {
						Console.WriteLine("Registration ID: " + registrationId);
						// AndroidHUD.AndHUD.Shared.ShowToast(context, "Registration " + (!t.IsFaulted && t.Result ? "Successful" : "Unsuccessful"), AndroidHUD.MaskType.Black, TimeSpan.FromSeconds(5));
					});
				}
			});

		}

		protected override void OnUnRegistered (Context context, string registrationId)
		{
			//Receive notice that the app no longer wants notifications
		}

		protected override void OnMessage (Context context, Intent intent)
		{
			var payloadString = intent.Extras.GetString ("payload");
			PushNotificationData data = JsonConvert.DeserializeObject<PushNotificationData> (payloadString);
			string title = "";
			string detail = "";
			PushNotificationUtils.GetDetailedActionDescription (data, out title, out detail);
			long[] vibratePattern = { 0, 500, 100, 200, 100, 200 };

			Intent todoIntent = new Intent(context, typeof(MyGamesActivity));
			PendingIntent pendingIntent = PendingIntent.GetActivity (context, 123, todoIntent, PendingIntentFlags.CancelCurrent);

			NotificationCompat.Builder builder = new NotificationCompat.Builder (context)
				.SetContentTitle (title)
				.SetContentText (detail)
				.SetSmallIcon (Resource.Drawable.Icon)
				.SetVibrate (vibratePattern)
				.SetLights (Color.Purple.ToArgb (), 1000, 2000)
				.SetContentIntent (pendingIntent)
				.SetOnlyAlertOnce(true);
			if (!string.IsNullOrEmpty (data.ThumbnailUri)) {
				WebClient client = new WebClient ();
				var imgData = client.DownloadData (data.ThumbnailUri);
				var bmp = BitmapFactory.DecodeByteArray (imgData, 0, imgData.Length);
				builder.SetLargeIcon (bmp);
			}

			// Obtain a reference to the NotificationManager
			var notification = builder.Build ();
			NotificationManager notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
			notificationManager.Notify (125165626, notification);
		}

		protected override bool OnRecoverableError (Context context, string errorId)
		{
			return true;
			//Some recoverable error happened
		}

		protected override void OnError (Context context, string errorId)
		{
			//Some more serious error happened
		}
	}
}

