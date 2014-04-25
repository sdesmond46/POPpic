using System;
using Buddy;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Timers;
using System.Collections.Generic;
using System.IO;

namespace POPpicLibrary
{
	public class GameplayViewModel
	{
		private GameRepository repository;
		public GameModel model { get; private set; }
		private User requester, responder;
		private User currentUser, otherUser;
		private long playerTime1, playerTime2;
		private long playerAddedTime1, playerAddedTime2;
		private bool isPlayer1Turn;
		private long totalElapsedStart, totalElapsedSession;
		private Stopwatch stopWatch;
		private double balloonContainerDimension;

		public string PlayerName1{ get { return this.requester.Name; } }
		public string PlayerProfileImage1 { get { return (this.requester.ProfilePicture ?? new Uri("http://www.artifacting.com/blog/wp-content/uploads/2010/11/Abe_Lincoln.jpg")).ToString (); } }
		public string PlayerTime1 { get { return FormattingUtilities.FormatMilliseconds (this.playerTime1 + playerAddedTime1, false); } }

		public string PlayerName2{ get { return this.responder.Name; } }
		public string PlayerProfileImage2 { get { return (this.responder.ProfilePicture ??  new Uri("http://www.artifacting.com/blog/wp-content/uploads/2010/11/Abe_Lincoln.jpg")).ToString (); } }
		public string PlayerTime2 { get { return FormattingUtilities.FormatMilliseconds (this.playerTime2 + playerAddedTime2, false); } }

		public string BottomHintText { get { 
				string text = "Not in valid state";
				if (this.model.State == GameState.COMPLETED) {
					text = IsCurrentUsersTurn ? "You Won!" : "You Lost";
				} else if (this.model.State == GameState.IN_PROGRESS || this.model.State == GameState.REQUEST_SENT) {
					text = IsCurrentUsersTurn ? "Tap & Hold the Balloon to Inflate" : "It's Their Turn";
				}

				return text;
			} }

		public string FinishTurnButtonText { get { return FinishTurnButtonEnabled ? "Finish Turn" : "Go Back";	} }

		public bool IsCurrentUsersTurn { get { return isPlayer1Turn ? this.currentUser.ID == this.requester.ID : this.currentUser.ID == this.responder.ID; } }

		public bool FinishTurnButtonEnabled { get { return this.model.State != GameState.COMPLETED && IsCurrentUsersTurn; } }

		public bool IsPlayer1sTurn { get { return isPlayer1Turn; } }

		public bool BalloonIsPopped { get { return this.totalElapsedStart + this.totalElapsedSession > this.model.TotalDuration; } }

		public int BalloonDimension { get { return (int) ComputeBalloonDimension(this.totalElapsedStart + this.totalElapsedSession, this.balloonContainerDimension); } }
		public double BalloonPercentage { get { return ComputeBalloonDimension (this.totalElapsedStart + this.totalElapsedSession, 1); } }

		public string GameGUID { get { return this.model.GameGUID; } }
		public POPpicLibrary.GameState GameState { get { return this.model.State; } }

		public byte[] LoserImgData { get; private set; }

		public Stream BackgroundImageStream { get; private set; }
		public Stream BalloonImageStream { get; private set; }

		public event EventHandler<bool> Tick;

		public GameplayViewModel (GameRepository repository, GameModel model, double balloonContainerHeight, double balloonContainerWidth)
		{
			this.model = model;
			this.repository = repository;
			this.stopWatch = new Stopwatch ();
			this.balloonContainerDimension = balloonContainerHeight < balloonContainerWidth ? balloonContainerHeight : balloonContainerWidth;
		}

		// Call this to have the view model update itself
		public void TickExternal() {
			TickInternal (null, null);
		}

		private void TickInternal(object sender, ElapsedEventArgs e) {
			this.totalElapsedSession = this.stopWatch.ElapsedMilliseconds;
			if (this.isPlayer1Turn) {
				this.playerAddedTime1 = this.totalElapsedSession;
			} else {
				this.playerAddedTime2 = this.totalElapsedSession;
			}

			if (this.GameState != GameState.COMPLETED) {
				this.Tick (this, this.BalloonIsPopped);
			}
		}

		private double ComputeBalloonDimension(long tickCount, double dimension) {
			const double minPercent = .2;
			const double maxPercent = .95;

			double slope = (maxPercent - minPercent) / ((double)(GameModel.BalloonMaxTime - GameModel.BalloonMinTime));
			double percent = (slope * tickCount) + minPercent;
			return percent * dimension;
		}




		public async Task<bool> InitializeAsync() {
			var tasks = new List<Task> ();

			this.requester = await this.repository.GetUserAsync (this.model.GameRequesterId);
			this.responder = await this.repository.GetUserAsync (this.model.GameResponderId);

			this.currentUser = this.requester.ID == this.repository.CurrentUserBuddyId ? this.requester : this.responder;
			this.otherUser = this.requester.ID == this.repository.CurrentUserBuddyId ? this.responder : this.requester;

			totalElapsedSession = 0;
			playerTime1 = playerTime2 = playerAddedTime1 = playerAddedTime2 = 0;
			isPlayer1Turn = this.model.GameMoves.Count % 2 == 0;
			User lastPlayer = null, winner = null;

			for (int i=0; i<this.model.GameMoves.Count; i++) {
				var move = this.model.GameMoves [i];
				lastPlayer = move.PlayerId == this.requester.ID ? requester : responder;
				if (i % 2 == 0) {
					playerTime1 += move.HoldDuration;
				} else {
					playerTime2 += move.HoldDuration;
				}
			}

			if (lastPlayer != null) {
				winner = lastPlayer.ID == this.requester.ID ? this.responder : this.requester;
			}

			totalElapsedStart = playerTime1 + playerTime2;

			if (model.State == GameState.COMPLETED && model.ImageIds.Count > 0) {
				// TODO crash or something if there is no image
				LoserImgData = await this.repository.GetLoserPictureImageStreamAsync (model.ImageIds [0], winner, model.GameGUID);
			}

			if (!string.IsNullOrEmpty (this.model.BalloonImageUrl) && !string.IsNullOrEmpty (this.model.BackgroundImageUrl)) {
				var imageRepository = new GameImageryRepository ();
				tasks.Add(imageRepository.GetImageAsync(this.model.BalloonImageUrl).ContinueWith(t => this.BalloonImageStream = t.Result));
				tasks.Add(imageRepository.GetImageAsync(this.model.BackgroundImageUrl).ContinueWith(t => this.BackgroundImageStream = t.Result));
				await Task.WhenAll (tasks);
			} else {
				this.BalloonImageStream = this.BackgroundImageStream = null;
			}




//
//			this.requester = await this.repository.GetUserAsync (this.model.GameRequesterId);
//			this.responder = await this.repository.GetUserAsync (this.model.GameResponderId);
//
//			this.currentUser = this.requester.ID == this.repository.CurrentUserBuddyId ? this.requester : this.responder;
//			this.otherUser = this.requester.ID == this.repository.CurrentUserBuddyId ? this.responder : this.requester;
//
//			totalElapsedSession = 0;
//			playerTime1 = playerTime2 = playerAddedTime1 = playerAddedTime2 = 0;
//			isPlayer1Turn = this.model.GameMoves.Count % 2 == 0;
//			User lastPlayer = null, winner = null;
//
//			for (int i=0; i<this.model.GameMoves.Count; i++) {
//				var move = this.model.GameMoves [i];
//				lastPlayer = move.PlayerId == this.requester.ID ? requester : responder;
//				if (i % 2 == 0) {
//					playerTime1 += move.HoldDuration;
//				} else {
//					playerTime2 += move.HoldDuration;
//				}
//			}
//
//			if (lastPlayer != null) {
//				winner = lastPlayer.ID == this.requester.ID ? this.responder : this.requester;
//			}
//
//			totalElapsedStart = playerTime1 + playerTime2;
//
//			if (model.State == GameState.COMPLETED && model.ImageIds.Count > 0) {
//				// TODO crash or something if there is no image
//				LoserImgData = await this.repository.GetLoserPictureImageStreamAsync (model.ImageIds [0], winner, model.GameGUID);
//			}

			return true;
		}

		public void WatchStart() {
			this.stopWatch.Start ();
		}

		public void WatchStop() {
			this.stopWatch.Stop ();
		}

//		private LoadGameOverImage() {
////			if (this.model.ImageIds.Count > 0) {
////				var imgId = this.model.ImageIds[0];
////				this.repository.
////			}
//		}



		public GameModel GetUpdatedGameModel() {
			var now = DateTime.UtcNow;
			GameModel newModel = new GameModel ();
			newModel.GameGUID = this.model.GameGUID;
			newModel.GameRequesterId = model.GameRequesterId;
			newModel.GameResponderId = model.GameResponderId;
			newModel.BackgroundImageUrl = model.BackgroundImageUrl;
			newModel.BalloonImageUrl = model.BalloonImageUrl;
			newModel.TotalDuration = model.TotalDuration;
			newModel.State = this.BalloonIsPopped ? GameState.COMPLETED : GameState.IN_PROGRESS;
			newModel.UpdatedTime = now;

			var currentRoundMove = new GameMoveModel ();
			currentRoundMove.HoldDuration = this.totalElapsedSession;
			currentRoundMove.PlayerId = this.IsPlayer1sTurn ? model.GameRequesterId : model.GameResponderId;
			currentRoundMove.Timestamp = now;

			var newMoves = new List<GameMoveModel> ();
			newMoves.AddRange (model.GameMoves);
			newMoves.Add (currentRoundMove);
			newModel.GameMoves = newMoves;

			return newModel;
		}

		public async Task<bool> UpdateGameModelAsync(Stream imageData, string message) {
			var updatedModel = GetUpdatedGameModel ();
//
//			if (imageData != null) {
//				long imageId = await repository.UploadLoserPhotoAsync (imageData, message, this.model.GameGUID, this.otherUser);
//				updatedModel.ImageIds.Add (imageId);
//			}
//
			bool result = await repository.UpdateGameModelAsync (updatedModel, this.otherUser, imageData);
			this.model = updatedModel;
			return result;
		}
	}
}

