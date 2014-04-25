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
	class AndroidGameListAdapter : BaseAdapter<GameViewModel>
	{
		public event EventHandler<GameViewModel> GameSelected;
		public event EventHandler<bool> CreateNewGameSelected;
		public event EventHandler<MyGamesViewModel.ListType> SeeAllClicked;

		private MyGamesViewModel model;
		private IList<GameViewModel> games;
		Activity context;
		MyGamesViewModel.ListType listType;
		public AndroidGameListAdapter(Activity context, MyGamesViewModel model, MyGamesViewModel.ListType listType) // , ActionMode.ICallback contextMenuCallback)
		{
			this.context = context;
			this.listType = listType;
			this.model = model;
			this.games = model.GetMyGames (listType);
		}

		#region implemented abstract members of BaseAdapter
		public override long GetItemId (int position)
		{
			return position;
		}
		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			View view = convertView; // re-use an existing view, if one is available
			if (view == null || view.FindViewById<RelativeLayout>(Resource.Id.myGameListItemLayout) == null) // otherwise create a new one
				view = this.context.LayoutInflater.Inflate(Resource.Layout.GameListItemLayout, null);


			var textView1 = view.FindViewById<TextView> (Resource.Id.textView1);
			var textView2 = view.FindViewById<TextView> (Resource.Id.textView2);
			var textView3 = view.FindViewById<TextView> (Resource.Id.textView3);

			string text1, text2, text3;
			var image = view.FindViewById<ImageView> (Resource.Id.imageView1);
			image.Visibility = ViewStates.Visible;
			if (position < this.maxItems && position < this.games.Count) { // rendering a game item
				textView2.Visibility = textView3.Visibility = ViewStates.Visible;
				var item = this.games [position];
				text1 = item.OpponentName;
				text2 = item.PreviousActionDescription;
				text3 = item.PreviousActionTimeStamp;

				var webClient = new WebClient ();
				webClient.DownloadDataTaskAsync (item.PictureUrl).ContinueWith (t => {
					if (!t.IsFaulted) {
						var bitmap = BitmapFactory.DecodeByteArray(t.Result, 0, t.Result.Length);
						image.SetImageBitmap(bitmap);
					}
				}, TaskScheduler.FromCurrentSynchronizationContext ());

				view.Click += (object sender, EventArgs e) => {
					if (this.GameSelected != null) {
						this.GameSelected(this, item);
					}
				};
			} else {
				textView2.Visibility = textView3.Visibility = ViewStates.Gone;
				text2 = text3 = "";
				if (this.listType == MyGamesViewModel.ListType.MY_TURN) {
					text1 = "Create Game";
					image.SetImageResource (Android.Resource.Drawable.IcInputAdd);

					view.Click += (object sender, EventArgs e) => {
						if (this.CreateNewGameSelected != null) {
							CreateNewGameSelected (this, true);
						}
					};
				} else if (this.GetTargetCount() == 0) {
					text1 = "No Games";
					image.Visibility = ViewStates.Invisible;
				}
				else {
					text1 = "View All";
					image.SetImageResource (Resource.Drawable.ic_action_expand);

					view.Click += (object sender, EventArgs e) => {
						if (this.SeeAllClicked != null) {
							this.SeeAllClicked(this, this.listType);
						}
					};
				}
			}

			textView1.Text = text1;
			textView2.Text = text2;
			textView3.Text = text3;

			return view;
		}

		private int maxItems = 30000;
		// private int maxItems = 3;
		public void SetMaxItems(int items) {
			this.maxItems = items;
			NotifyDataSetChanged ();
		}

		public override int Count {
			get {
				int targetCount = GetTargetCount ();
				if (targetCount == 0) {
					return 1;
				}

				return targetCount;
			}
		}

		private int GetTargetCount() {
			int gamesToShow = Math.Min (this.maxItems, this.games.Count);
			int additionalItem = 0;
			if (this.listType == MyGamesViewModel.ListType.MY_TURN) {
				additionalItem = 1;
			} else if (this.maxItems < this.games.Count) {
				additionalItem = 1;
			}

			// return gamesToShow + additionalItem;
			return Math.Max (1, this.games.Count);
		}

		#endregion
		#region implemented abstract members of BaseAdapter
		public override GameViewModel this [int index] {
			get {
				return index < this.games.Count ? this.games [index] : null;
			}
		}
		#endregion

	}
}

