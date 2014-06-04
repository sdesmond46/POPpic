using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace POPpicLibrary
{
	public class GameSetupViewModel
	{
		GameRepository gameRepository;
		public GameSetupViewModel (GameRepository gameRepository)
		{
			this.gameRepository = gameRepository;
			this.IsInitialized = false;
			this.BalloonImages = new List<GameImageryItemViewModel> ();
			this.BackgroundImages = new List<GameImageryItemViewModel> ();
		}

		public async Task<bool> InitializeAsync() {
			var repository = new GameImageryRepository ();
			this.BalloonImages = await repository.GetBalloonImagesAsync ();
			this.BackgroundImages = await repository.GetBackgroundImagesAsync ();
			this.CurrentBackgroundImage = repository.DefaultBackground;
			this.CurrentBalloonImage = repository.DefaultBallon;

			return this.IsInitialized = true;
		}

		public async Task<GameplayViewModel> CreateNewGame(float height, float width) {
			var gameGuid = await this.gameRepository.SendGameRequestAsync (this.CurrentBalloonImage, this.CurrentBackgroundImage, this.Opponent.User);
			var gameModel = await this.gameRepository.GetGameAsync (gameGuid);
			var gameplayViewModel = new GameplayViewModel (this.gameRepository, gameModel, height, width);
			var result = await gameplayViewModel.InitializeAsync ();
			return gameplayViewModel;
		}

		public FriendViewModel Opponent {get;set;}
		public bool IsInitialized{ get; private set; }

		public IList<GameImageryItemViewModel> BalloonImages{get;set;}
		public GameImageryItemViewModel CurrentBalloonImage{ get; set;}
		public IList<GameImageryItemViewModel> BackgroundImages{get;set;}
		GameImageryItemViewModel CurrentBackgroundImage{ get; set;}
	}
}

