using System;

namespace POPpicLibrary
{
	public class PushNotificationData
	{
		public enum PushAction
		{
			GAME_CREATED,
			MADE_MOVE,
			LOST_GAME
		}

		public string OpponentName { get; set;}
		public PushAction Action { get; set;}
		public long MoveDuration { get; set;}
		public string ThumbnailUri { get; set; }

		public PushNotificationData ()
		{

		}
	}

	public static class PushNotificationUtils {

		public static void GetDetailedActionDescription(PushNotificationData data, out string titleDescription, out string detailedDescription) {
			titleDescription = "Unknown";
			detailedDescription = "Unknown";

			switch (data.Action) {
			case PushNotificationData.PushAction.GAME_CREATED:
				titleDescription = "New Game Created";
				detailedDescription = string.Format ("{0} invited you to play popPIC", data.OpponentName);
				break;
			case PushNotificationData.PushAction.MADE_MOVE:
				titleDescription = "It's Your Turn";
				detailedDescription = string.Format ("{0} pressed for {1}", data.OpponentName, FormattingUtilities.FormatMilliseconds (data.MoveDuration, true));
				break;
			case PushNotificationData.PushAction.LOST_GAME:
				titleDescription = "You Won!";
				detailedDescription = string.Format ("{0} popped the balloon", data.OpponentName);
				break;
			}

		}
	}
}

