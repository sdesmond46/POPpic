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
using System.Net;
using Android.Graphics;
using System.Threading.Tasks;

namespace POPpic
{
	class AndroidFriendListAdapter : BaseAdapter<FriendViewModel>
	{
		public event EventHandler<FriendViewModel> FriendSelected;

		private SelectFriendViewModel model;
		Activity context;
		public AndroidFriendListAdapter(Activity context, SelectFriendViewModel model)
		{
			this.context = context;
			this.model = model;
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			View view = convertView; // re-use an existing view, if one is available
			var item = this.model.Friends [position];
			// if (view == null) // otherwise create a new one
				view = this.context.LayoutInflater.Inflate(Resource.Layout.FriendListItemLayout, null);
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

			view.Click += (object sender, EventArgs e) => {
				if (this.FriendSelected != null) {
					this.FriendSelected(this, item);
				}
			};
			return view;
		}

		public override int Count {
			get {
				return this.model.Friends.Count;
			}
		}

		public override FriendViewModel this [int index] {
			get {
				return this.model.Friends [index];
			}
		}
	}
}

