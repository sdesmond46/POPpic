using System;
using System.Collections.Generic;

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
		public FriendSourceId SourceId { get; set; }

		public static FriendSourceViewModel CreateModel (string title, string description, int resource, FriendSourceId sourceId) {
			var model = new FriendSourceViewModel ();
			model.SourceTitle = title;
			model.SourceDescription = description;
			model.IconResource = resource;
			model.SourceId = sourceId;

			return model;
		}
	}

	public class SelectFriendSourceViewModel
	{
		public SelectFriendSourceViewModel ()
		{
			this.FriendSources = new List<FriendSourceViewModel> ();
			this.FriendSourceHeader = "Choose Opponent";
			this.RecentFriendsHeader = "Recent Opponents";
			this.RecentFriends = new List<FriendViewModel> ();

		}



		public string FriendSourceHeader{ get; set;}
		public IList<FriendSourceViewModel> FriendSources {get;set;}

		public string RecentFriendsHeader { get; set;}
		public IList<FriendViewModel> RecentFriends {get;set;}
	}
}

