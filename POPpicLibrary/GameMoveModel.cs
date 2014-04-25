using System;

namespace POPpicLibrary
{
	public class GameMoveModel
	{
		public GameMoveModel ()
		{

		}

		public int PlayerId { get; set; }
		public long HoldDuration{ get; set; }
		public DateTime Timestamp{ get; set; }
	}
}

