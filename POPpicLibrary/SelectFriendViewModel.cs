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
		}

		private IList<User> users;
		public IList<FriendViewModel> Friends;
		public bool Initialized { get; private set;}
		public async Task<bool> InitializeAsync()
		{
			Initialized = false;
			users = await this.repository.GetMyFriendsFQLAsync ();
			var friendList = new List<FriendViewModel> ();
			foreach (var user in users) {
				var vm = new FriendViewModel (user);
				friendList.Add (vm);
			}

			this.Friends = friendList;

			Initialized = true;
			return Initialized;
		}

		public static string SelectedFriendKey = "SELECTED_FRIEND";
		public static string NewGameGuidKey = "NEW_GAME_GUID";
	}
}

