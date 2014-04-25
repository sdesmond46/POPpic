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

namespace POPpic
{
	public class AndroidSelectFriendSourceListAdapter : BaseAdapter<FriendSourceViewModel>
	{
		public event EventHandler<FriendSourceViewModel> OptionSelected;

		IList<FriendSourceViewModel> items;
		Activity context;
		public AndroidSelectFriendSourceListAdapter(Activity context, IList<FriendSourceViewModel> items) 
		{
			this.items = items;
			this.context = context;
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
			// if (view == null) // otherwise create a new one
			view = this.context.LayoutInflater.Inflate(Resource.Layout.FriendListItemLayout, null);

			view.FindViewById<TextView> (Resource.Id.textView1).Text = item.SourceTitle;
			view.FindViewById<TextView> (Resource.Id.textView2).Text = item.SourceDescription;
			var image = view.FindViewById<ImageView> (Resource.Id.imageView1);
			image.SetImageResource (item.IconResource);

			view.Click += (object sender, EventArgs e) => {
				if (OptionSelected != null) {
					OptionSelected(this, item);
				}
			};
			return view;
		}

		public override int Count {
			get {
				return this.items.Count;
			}
		}

		#endregion
		#region implemented abstract members of BaseAdapter
		public override FriendSourceViewModel this [int index] {
			get {
				return this.items [index];
			}
		}
		#endregion
	}
}

