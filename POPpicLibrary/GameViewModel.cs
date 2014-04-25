using System;
using Buddy;

namespace POPpicLibrary
{
	public class GameViewModel
	{
		public GameViewModel ()
		{

		}

		public User CurrentUser { get; set;}
		public User Opponent { get; set; }
		public GameModel Model { get; set; }

		public string OpponentName { get { return this.Opponent.Name; }}
		public Uri PictureUrl { get { return this.Opponent.ProfilePicture; } }
		public string PreviousActionDescription { get { return FormattingUtilities.LastMoveDescription(Model, CurrentUser.ID); } }
		public string PreviousActionTimeStamp { get { return FormattingUtilities.ElapsedTime(this.Model.UpdatedTime); } }
	}
}

