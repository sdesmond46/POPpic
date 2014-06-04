using System;
using System.Collections.Generic;
using Buddy;
using System.Threading.Tasks;

namespace POPpicLibrary
{
	public class SelectFriendViewModel
	{
		GameRepository repository;
		public SelectFriendViewModel (GameRepository repository)
		{
			this.repository = repository;
			this.PageTitle = "Facebook Friends";
		}

		private IList<User> users;
		public IList<FriendViewModel> Friends;
		public bool IsInitialized { get; private set;}
		public string PageTitle { get; set; }
		public async Task<bool> InitializeAsync()
		{
			IsInitialized = false;
			users = await this.repository.GetMyFriendsFQLAsync ();
			var friendList = new List<FriendViewModel> ();
			foreach (var user in users) {
				var vm = new FriendViewModel (user);
				friendList.Add (vm);
			}

			this.Friends = friendList;

			IsInitialized = true;
			return IsInitialized;
		}

		public static string SelectedFriendKey = "SELECTED_FRIEND";
		public static string NewGameGuidKey = "NEW_GAME_GUID";
	}
}

