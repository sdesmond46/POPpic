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
using System.IO;
using Android.Graphics;

namespace POPpic
{
	public class AndroidUtilities
	{
		public static void ShowAlert (Context context, String title, String message, string buttonMessage = "ok", EventHandler<DialogClickEventArgs> okHandler = null)
		{

			new AlertDialog.Builder (context)
				.SetTitle (title)
				.SetMessage (message)
				.SetPositiveButton (buttonMessage, okHandler)
				.Show ();
		}

		public static Bitmap DecodeBitmapToSize(Stream inputStream, int desiredDimension) {
			try {
				inputStream.Seek(0, SeekOrigin.Begin);
				BitmapFactory.Options o = new BitmapFactory.Options();
				o.InJustDecodeBounds = true;
				BitmapFactory.DecodeStream(inputStream, null, o);


				inputStream.Seek(0, SeekOrigin.Begin);
				int scale = 1;
				while (o.OutWidth/scale/2>=desiredDimension && o.OutWidth/scale/2>=desiredDimension) {
					scale*=2;
				}

				BitmapFactory.Options finalOptions = new BitmapFactory.Options();
				finalOptions.InSampleSize = scale;
				return BitmapFactory.DecodeStream(inputStream, null, finalOptions);
			} catch {return null;}
		}
	}
}

