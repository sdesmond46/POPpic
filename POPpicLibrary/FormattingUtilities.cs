using System;
using System.Linq;

namespace POPpicLibrary
{
	public static class FormattingUtilities
	{
		public static string ElapsedTime(DateTime dtEvent) 
		{ 
			var now = DateTime.UtcNow;
			TimeSpan timeSpan = now - dtEvent; 
			int intDays = (int) timeSpan.TotalDays;
			int intHours = (int) timeSpan.TotalHours;
			int intMinutes = (int) timeSpan.TotalMinutes;
			int intSeconds = (int) timeSpan.TotalSeconds;
			if (intDays > 0) return String.Format("{0} {1} ago", intDays, (intDays == 1) ? "day" : "days");
			else if (intHours > 0) return String.Format("{0} {1} ago", intHours, (intHours == 1) ? "hour" : "hours");
			else if (intMinutes > 0) return String.Format("{0} {1} ago", intMinutes, (intMinutes == 1) ? "minute" : "minutes");
			else if (intSeconds >= 0) return String.Format("{0} {1} ago", intSeconds, (intSeconds == 1) ? "second" : "seconds"); 
			else 
			{ 
				return String.Format("{0} {1} ago", dtEvent.ToShortDateString(), dtEvent.ToShortTimeString()); 
			} 
		} 

		public static string LastMoveDescription(GameModel model, int currentUserId) {
			switch (model.State) {
			case GameState.REQUEST_SENT:
			case GameState.REQUEST_REFUSED:
				return "Not Started";
			case GameState.IN_PROGRESS:
				return MoveDescription (model.GameMoves.Last (), currentUserId);
			case GameState.COMPLETED:
				return model.WhosTurn == currentUserId ? "You Won!" : "You Lost :(";
			}

			return "Invalid Move";
		}

		public static string MoveDescription(GameMoveModel move, int currentUserId) {
			return FormatMilliseconds (move.HoldDuration, true);
		}

		public static string FormatMilliseconds(long milliseconds, bool showSeconds) {
			return (milliseconds / 1000.0).ToString ("F2") + (showSeconds ? " seconds" : "");
		}
	}
}

