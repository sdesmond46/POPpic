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
using POPpicLibrary;
using System.Threading.Tasks;

namespace POPpic
{
	public class SelectFacebookFriendFragment : Fragment
	{
		public event EventHandler<FriendViewModel> FriendSelected;

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
		}

		View myView = null;
		SelectFriendViewModel viewModel;
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			if (myView == null) {
				myView = inflater.Inflate (Resource.Layout.SelectFriendSourceLayout, container, false);
				var rootView = myView.FindViewById<LinearLayout> (Resource.Id.linearLayout1);
				var friendList = Utility.CreateListView ("Select Friend", rootView, Resources, inflater);

				((POPpicApplication)this.Activity.Application).GetGameRepository (this.Activity).ContinueWith (r => {
					this.viewModel = new SelectFriendViewModel (r.Result);
					viewModel.InitializeAsync ().ContinueWith (t => {
						if (!t.IsFaulted && t.Result) {
							this.Activity.RunOnUiThread(() => {
								listAdapter = new AndroidFriendListAdapter(this.Activity, viewModel);
								listAdapter.FriendSelected += (object sender, FriendViewModel e) => {
									if (this.FriendSelected != null) 
										this.FriendSelected(sender, e);
								};
								friendList.Adapter = listAdapter;
							});
						}
					});
				});
			}

			return myView;
		}

		AndroidFriendListAdapter listAdapter;
	}
}

