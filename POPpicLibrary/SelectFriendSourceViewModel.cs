using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace POPpicLibrary
{
	public enum FriendSourceId {
		FACEBOOK,
		RANDOM,
		RANDOM_NEARBY
	}

	public class FriendSourceViewModel
	{
		public string SourceTitle {get;set;}
		public string SourceDescription {get;set;}
		public int IconResource { get;set;}
		public string IconResourceName { get; set; }
		public FriendSourceId SourceId { get; set; }

		public static FriendSourceViewModel CreateModel (string title, string description, int resource, string resourceName, FriendSourceId sourceId) {
			var model = new FriendSourceViewModel ();
			model.SourceTitle = title;
			model.SourceDescription = description;
			model.IconResource = resource;
			model.IconResourceName = resourceName;
			model.SourceId = sourceId;

			return model;
		}
	}

	public class SelectFriendSourceViewModel
	{
		GameRepository repository;
		public SelectFriendSourceViewModel (GameRepository repository)
		{
			this.repository = repository;
			this.FriendSources = new List<FriendSourceViewModel> ();
			this.FriendSourceHeader = "New Opponent";
			this.RecentFriendsHeader = "Recent Opponents";
			this.Title = "Choose Opponent";
			this.RecentFriends = new List<FriendViewModel> ();
			this.IsInitialized = false;
		}

		public Task<bool> InitializeAsync() {
			this.FriendSources.Add (FriendSourceViewModel.CreateModel ("Select Facebook Friend", "Facebook", 0, "facebook_icon.png", FriendSourceId.FACEBOOK));
			this.FriendSources.Add (FriendSourceViewModel.CreateModel ("Play A Random Person", "Random", 0, "randomUserIcon.png", FriendSourceId.RANDOM));

			int num = 0;
			return repository.GetRecentOpponentsAsync ().ContinueWith (t => {
				if (!t.IsFaulted) {
					this.RecentFriends = t.Result.TakeWhile (((Buddy.User arg) => num++ < 3)).Select ((Buddy.User user, int arg2) => {
						FriendViewModel model = new FriendViewModel (user);
						return model;
					}).ToList ();

					return this.IsInitialized = true;
				} else {
					return this.IsInitialized = false;
				}
			});
		}

		public bool IsInitialized {get;private set;}

		public string Title {get;set;}

		public string FriendSourceHeader{ get; set;}
		public IList<FriendSourceViewModel> FriendSources {get;set;}

		public string RecentFriendsHeader { get; set;}
		public IList<FriendViewModel> RecentFriends {get;set;}
	}
}

