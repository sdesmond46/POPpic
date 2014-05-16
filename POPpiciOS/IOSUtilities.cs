using System;
using System.Threading.Tasks;
using System.IO;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace POPpiciOS
{
	public static class IOSUtilities
	{
		public static void SetImageFromStream(Task<Stream> taskToAwait, UIImageView imageView, NSObject context)
		{
			taskToAwait.ContinueWith(t => {
					if (!t.IsFaulted) {
						var data = NSData.FromStream(t.Result);
					context.InvokeOnMainThread(() => {
							var image = UIImage.LoadFromData(data);
							imageView.Image = image;
						});
					} else {
						Console.WriteLine(t.Exception.ToString());
					}
				});
		}
	}
}

