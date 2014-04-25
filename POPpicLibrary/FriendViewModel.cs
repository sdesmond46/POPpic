using System;
using Buddy;

namespace POPpicLibrary
{
	public class FriendViewModel
	{
		public User User{ get; private set; } 
		public FriendViewModel (User user)
		{
			this.User = user;
			this.Record = "22 - 4";
		}

		public Uri ProfilePictureUri { get { return this.User.ProfilePicture; } }
		public string Name { get { return this.User.Name; } }
		public string Record { get; private set; }
		public int UserId{get { return User.ID; }}

		public static FriendViewModel GetInvalidUser(string caption) {
			var invUser = new InvalidUser (null, caption);
			FriendViewModel model = new FriendViewModel (invUser);
			model.Record = "";
			return model;
		}
	}

	public class InvalidUser : User
	{
		public InvalidUser(BuddyClient client, string caption) : base(client) {	
			this.ProfilePicture = new Uri("http://www.artifacting.com/blog/wp-content/uploads/2010/11/Abe_Lincoln.jpg");
			this.Name = caption;
			this.ID = -1;
		}
	}
}

