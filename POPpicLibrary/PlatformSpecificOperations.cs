using System;
using System.IO;
using Android.Graphics;

namespace POPpicLibrary
{
	public class PlatformSpecificOperations
	{
		public static Stream CreateProfilePicture(Stream imageStream) {
			imageStream.Seek (0, SeekOrigin.Begin);
			var originalBmp = BitmapFactory.DecodeStream (imageStream);
			Bitmap dstBmp;
			if (originalBmp.Width >= originalBmp.Height){
				dstBmp = Bitmap.CreateBitmap(
					originalBmp, 
					originalBmp.Width/2 - originalBmp.Height/2,
					0,
					originalBmp.Height, 
					originalBmp.Height
				);
			} else {
				dstBmp = Bitmap.CreateBitmap(
					originalBmp,
					0, 
					originalBmp.Height/2 - originalBmp.Width/2,
					originalBmp.Width,
					originalBmp.Width 
				);
			}

			var finalImage = Bitmap.CreateScaledBitmap (dstBmp, 75, 75, false);

			MemoryStream memStream = new MemoryStream ();
			finalImage.Compress (Bitmap.CompressFormat.Jpeg, 90, memStream);

			memStream.Seek (0, SeekOrigin.Begin);
			return memStream;
		}

		public PlatformSpecificOperations ()
		{
		}
	}
}

