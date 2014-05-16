using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Buddy;

namespace POPpicLibrary
{
	public class MyGamesViewModel
	{
		public enum ListType { 
			MY_TURN,
			THEIR_TURN,
			COMPLETED
		}

		GameRepository repository;
		public MyGamesViewModel (GameRepository repository)
		{
			this.repository = repository;
			Initialized = false;
		}

		public async Task<bool> InitializeAsync()
		{
			Initialized = false;
			Initialized = await LoadMyGamesAsync ();
			return Initialized;
		}

		public async Task<bool> LoadMyGamesAsync()
		{
			var myGames = new List<GameViewModel> ();
			var games = await this.repository.GetAllGamesAsync ();
			var userDictionary = new Dictionary<int, User>();
			foreach (var game in games) {
				var gameViewModel = new GameViewModel ();
				gameViewModel.Model = game;
				gameViewModel.CurrentUser = this.repository.CurrentUser;

				int otherUserId = game.OtherUser (repository.CurrentUserBuddyId);
				if (!userDictionary.ContainsKey (otherUserId)) {
					var opponent = await this.repository.GetUserAsync (otherUserId);
					userDictionary [otherUserId] = opponent;
				} 

				gameViewModel.Opponent = userDictionary [otherUserId];
				myGames.Add (gameViewModel);
			}

			this.myGames = myGames;


			return true;
		}

		public string MyGamesHeader { get { return "My Turn"; } }
		public string TheirGamesHeader { get { return "Their Turn"; } }
		public string CompletedGamesHeader { get { return "Completed"; } }

		public async Task<string> CreateGameAsync(int userId)
		{
			return "";
//
//			var user = await this.repository.GetUserAsync (userId);
//			var gameGuid = await this.repository.SendGameRequestAsync (user);
//			return gameGuid;
		}

		public bool Initialized { get; private set; }
		private IList<GameViewModel> myGames;

		public static string GetGameTypeDescription(ListType listType) {
			switch (listType) {
			case ListType.MY_TURN:
				return "Your Turn";
			case ListType.THEIR_TURN:
				return "Their Turn";
			case ListType.COMPLETED:
				return "Completed";
			}

			return "";
		}

		public IList<GameViewModel> GetMyGames(ListType type)
		{
			if (type == ListType.MY_TURN) {
				return MyTurnGames;
			} else if (type == ListType.THEIR_TURN) {
				return TheirTurnGames;
			} else {
				return CompletedGames;
			}
		}

		public GameViewModel GetGameById(string gameGuid) {
			var viewModel = this.myGames.Where ((model) => model.Model.GameGUID == gameGuid).FirstOrDefault ();
			return viewModel;
		}

		public IList<GameViewModel> MyTurnGames { get { 
				var games = this.myGames.Where ((m) => {
					return m.Model.State != GameState.COMPLETED && m.Model.WhosTurn == this.repository.CurrentUserBuddyId;
				}).ToList ();
				games.Sort ((x, y) => -DateTime.Compare(x.Model.UpdatedTime, y.Model.UpdatedTime));
				return games;
			} }

		public IList<GameViewModel> TheirTurnGames { get { 
				var games = this.myGames.Where ((m) => {
					return m.Model.State != GameState.COMPLETED && m.Model.WhosTurn != this.repository.CurrentUserBuddyId;
				}).ToList();
				games.Sort ((x, y) => -DateTime.Compare(x.Model.UpdatedTime, y.Model.UpdatedTime));
				return games;
			} }

		public IList<GameViewModel> CompletedGames { get { 
				var games = this.myGames.Where ((m) => {
					return m.Model.State == GameState.COMPLETED;
				}).ToList();
				games.Sort ((x, y) => -DateTime.Compare(x.Model.UpdatedTime, y.Model.UpdatedTime));
				return games;
			} }

		public class GameSorter : IComparer<GameViewModel>
		{
			public int Compare (GameViewModel x, GameViewModel y)
			{
				return (int) (x.Model.UpdatedTime.Ticks - y.Model.UpdatedTime.Ticks);
			}
		}
	}
}

