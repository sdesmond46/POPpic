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
using POPpicLibrary;
using System.IO;
using Android.Graphics;
using System.Threading.Tasks;

namespace POPpic
{
	class AndroidSelectGameImageryListAdapter : BaseAdapter<GameImageryItemViewModel>
	{
		private IList<GameImageryItemViewModel> items;
		Activity context;
		GameImageryRepository imageryRepository;
		public AndroidSelectGameImageryListAdapter(Activity context, IList<GameImageryItemViewModel> items, GameImageryRepository imageryRepository) {
			this.context = context;
			this.items = items;
			this.imageryRepository = imageryRepository;
		}

		#region implemented abstract members of BaseAdapter
		public override long GetItemId (int position)
		{
			return position;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			View view = convertView; // re-use an existing view, if one is available
			var item = this.items [position];
			if (view == null) // otherwise create a new one
				view = this.context.LayoutInflater.Inflate(Resource.Layout.GameImageryLayout, null);

			var description = view.FindViewById<TextView> (Resource.Id.textView1);
			var image = view.FindViewById<ImageView> (Resource.Id.imageView1);
			var progress = view.FindViewById<ProgressBar> (Resource.Id.progressBar1);
			progress.Visibility = ViewStates.Visible;
			image.Visibility = ViewStates.Invisible;
			description.Text = item.ItemName;
			imageryRepository.GetImageAsync(item.ThumbnailUrl).ContinueWith(t => {
				if (!t.IsFaulted) {
					Stream s = t.Result;
					var bmp = BitmapFactory.DecodeStream (s);
					image.SetImageBitmap(bmp);
				} else {
					image.SetImageResource(Android.Resource.Drawable.IcDialogAlert);
				}
				progress.Visibility = ViewStates.Gone;
				image.Visibility = ViewStates.Visible;
			}, TaskScheduler.FromCurrentSynchronizationContext());

			return view;
		}

		public override int Count {
			get {
				return items.Count;
			}
		}
		#endregion
		#region implemented abstract members of BaseAdapter
		public override GameImageryItemViewModel this [int index] {
			get {
				return items [index];
			}
		}
		#endregion
	}
}

