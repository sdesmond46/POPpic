using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace POPpicLibrary
{
	public class GameModel
	{
		public static long BalloonMinTime = 2500;
		public static long BalloonMaxTime = 10000;

		public GameModel ()
		{
			this.GameMoves = new List<GameMoveModel> ();
			this.ImageIds = new List<long> ();
		}

		public string GameGUID { get; set; }

		public List<GameMoveModel> GameMoves {get;set;}

		public long TotalDuration {get;set;}

		public int GameRequesterId {get;set;}
		public int GameResponderId {get;set;}

		public GameState State { get; set; }

		public DateTime UpdatedTime { get; set; }

		public List<long> ImageIds { get; set; }

		public string BalloonImageUrl { get; set; }
		public string BackgroundImageUrl { get; set; }

		public int OtherUser(int currentId)
		{
			return GameRequesterId == currentId ? GameResponderId : GameRequesterId;
		}

		public int WhosTurn { 
			get 
			{ 
				if (State == GameState.REQUEST_SENT) {
					return this.GameRequesterId;
				} else if (State == GameState.REQUEST_REFUSED) {
					return this.GameRequesterId;
				} else {
					if (GameMoves.Count == 0) {
						return this.GameResponderId;
					} else {
						var lastMove = GameMoves.Last ();
						return lastMove.PlayerId == this.GameRequesterId ? this.GameResponderId : this.GameRequesterId;
					}
				}
			}
		}

		public string SerializeToJson() {
			return JsonConvert.SerializeObject (this);
		}

		public static GameModel DeserializeFromJson(string data) {
			return JsonConvert.DeserializeObject<GameModel> (data);
		}

		public static string GetKey() {
			return "GameModel";
		}
	}

	public enum GameState
	{
		REQUEST_SENT,
		REQUEST_REFUSED,
		IN_PROGRESS,
		COMPLETED
	}
}

