using System;
using System.Collections.Generic;

namespace POPpicLibrary
{
	public class GameSetupViewModel
	{
		public GameSetupViewModel ()
		{
			this.BalloonImages = new List<GameImageryItemViewModel> ();
			this.BackgroundImages = new List<GameImageryItemViewModel> ();
		}

		private FriendViewModel noOponentSelectedFriendModel;

		public FriendViewModel Opponent {get;set;}

		public IList<GameImageryItemViewModel> BalloonImages{get;set;}
		GameImageryItemViewModel CurrentBalloonImage;
		public IList<GameImageryItemViewModel> BackgroundImages{get;set;}
		GameImageryItemViewModel CurrentBackgroundImage;
	}
}

