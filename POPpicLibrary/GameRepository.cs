using System;
using Buddy;
using Facebook;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Text;
using System.Dynamic;
using System.Net;
using System.Collections.Concurrent;
namespace POPpicLibrary
{
	public static class Statics 
	{
		//	public static GameRepository Repository = null;
	}

	public class GameRepository
	{

		private const char messageFormatSeparator = ':';
		private const string messageFormatString = "{0}:{1}:1.1";
		private const string appVersionString = "1.1";
		private const string newRequestMessage = "accept";
		private const string acceptMessage = "accept";
		private const string rejectMessage = "reject";
		private const string mimeType = "application/json";
		private const string blobUploadTag = "gameBlobUpload";
		private const string facebookMetadataPrefix = "facebook:";

		private AuthenticatedUser buddyUser;
		private FacebookClient facebookClient;
		private BuddyClient buddyClient;
		public GameRepository (AuthenticatedUser buddyUser, BuddyClient buddyClient, FacebookClient facebookClient)
		{
			this.buddyUser = buddyUser;
			this.buddyClient = buddyClient;
			this.facebookClient = facebookClient;
		}

		public int CurrentUserBuddyId { get { return this.buddyUser.ID; } }
		public User CurrentUser { get { return this.buddyUser; } }

		public async Task<IList<GameModel>> GetAllGamesAsync()
		{
			var receivedMessages = await buddyUser.Messages.GetReceivedAsync ();
			var sentMessage = await buddyUser.Messages.GetSentAsync ();
			var gameGuids = new HashSet<string> ();
			Action<Message> foreachLambda = (Message m) => {
				var tokens = m.Text.Split(messageFormatSeparator);
				if (tokens.Length == 3 && tokens[2] == appVersionString) {
					string guid = tokens[0];
					if (!gameGuids.Contains(guid)) {
						gameGuids.Add(guid);
					}
				}
			};

			foreach (var message in receivedMessages) {
				foreachLambda (message);
			}
			foreach (var message in sentMessage) {
				foreachLambda (message);
			}

			Console.WriteLine ("We have " + gameGuids.Count + " games");

			var getGameTasks = gameGuids.Select(gameGuid => {
				var game = GetGameAsync (gameGuid);
				return game;
			});

			var gamesFromTask = await Task.WhenAll (getGameTasks);

			return gamesFromTask.Where((g) => g != null).ToList();
		}

		public async Task<GameModel> GetGameAsync(string gameGuid)
		{
			Console.WriteLine ("Getting Game " + gameGuid);
			try {
				var metadata = await buddyClient.Metadata.GetAsync(gameGuid);
				var text = metadata.Value;
				Console.WriteLine(text);
				var gameModel = JsonConvert.DeserializeObject<GameModel>(text);
				return gameModel;
			} catch (Exception e) {

				Console.WriteLine ("Getting Game " + gameGuid + " Failed");
				return null;
			}

		}

		public async Task<string> SendGameRequestAsync(GameImageryItemViewModel balloonImage, GameImageryItemViewModel backgroundImage, User user)
		{
			string gameGuid = Guid.NewGuid ().ToString ();
			GameModel model = new GameModel ();
			model.GameGUID = gameGuid;
			model.GameRequesterId = this.buddyUser.ID;
			model.GameResponderId = user.ID;
			model.State = GameState.REQUEST_SENT;
			model.TotalDuration = new Random().Next((int)GameModel.BalloonMinTime, (int)GameModel.BalloonMaxTime);
			model.UpdatedTime = DateTime.UtcNow;
			model.BalloonImageUrl = balloonImage.ImageUrl;
			model.BackgroundImageUrl = backgroundImage.ImageUrl;

			if (await AddGameModelAsync (model) &&
				await this.buddyUser.Messages.SendAsync (user, string.Format(messageFormatString, gameGuid, newRequestMessage), "newGameRequest")) {

				PushNotificationData pushData = new PushNotificationData ();
				pushData.Action = PushNotificationData.PushAction.GAME_CREATED;
				pushData.MoveDuration = 0;
				pushData.OpponentName = this.buddyUser.Name;
				pushData.ThumbnailUri = "";
				await SendPushAsync (pushData, user.ID);

				return gameGuid;
			} else {
				return null;
			}

		}

		public async Task<bool> SendPushAsync(PushNotificationData data, int receiverId) {
			var dataString = JsonConvert.SerializeObject (data);
			bool result = await this.buddyUser.PushNotifications.Android.SendRawMessageAsync (dataString, this.buddyUser.ID, default(DateTime), receiverId.ToString ());
			return result;
		}


		public async Task<GameModel> RespondToGameRequestAsync(GameModel model, bool accept)
		{
			// Here is where we send back the message and create the game blob
			var userToRespondTo = await this.buddyUser.FindUserAsync (model.GameRequesterId);
			model.State = accept ? GameState.IN_PROGRESS : GameState.REQUEST_REFUSED;
			string messageText = string.Format (messageFormatString, model.GameGUID, accept ? acceptMessage : rejectMessage);
			if (await UpdateGameModelAsync (model) &&
				await this.buddyUser.Messages.SendAsync (userToRespondTo, messageText, "gameRequestResponse"))
			{
				Console.WriteLine ("Response sent successfully");
				return model;
			} else {
				Console.WriteLine ("Sending response failed");
				return null;
			}
		}

		public async Task<bool> AddGameModelAsync(GameModel model)
		{
			string output = JsonConvert.SerializeObject (model);
			Console.WriteLine(output);
			return await this.buddyClient.Metadata.SetAsync (model.GameGUID, output);
		}

		public async Task<bool> UpdateGameModelAsync(GameModel model, User opponent = null, Stream photoStream = null)
		{
			model.UpdatedTime = DateTime.UtcNow;
			// Console.WriteLine(output);

			string thumbnail = "";
			PushNotificationData.PushAction pushAction = PushNotificationData.PushAction.MADE_MOVE;
			if (opponent != null && photoStream != null) {
				var userExtraData = JsonConvert.DeserializeObject<UserExtraData> (this.buddyUser.ApplicationTag);
				var opponentExtraData = JsonConvert.DeserializeObject<UserExtraData> (opponent.ApplicationTag);

				var album = await this.buddyUser.PhotoAlbums.GetAsync (userExtraData.UploadAlbumId);
				PicturePublic uploadedPicture = await album.AddPictureAsync (photoStream, "", 0, 0, model.GameGUID);
				var virtualAlbum = await buddyUser.VirtualAlbums.GetAsync (opponentExtraData.WinnerAblumVirtualId);
				var imageId = await virtualAlbum.AddPictureAsync (uploadedPicture);

				// TODO make this happen in parallel
				{
					var profileStream = PlatformSpecificOperations.CreateProfilePicture (photoStream);
					bool profileSuccess = await this.buddyUser.AddProfilePhotoAsync (profileStream);
				}

				model.ImageIds.Add (imageId);
				thumbnail = uploadedPicture.ThumbnailUrl;
				pushAction = PushNotificationData.PushAction.LOST_GAME;
			}

			string output = JsonConvert.SerializeObject (model);
			bool metadata = await this.buddyClient.Metadata.SetAsync (model.GameGUID, output);

			PushNotificationData pushData = new PushNotificationData ();
			pushData.Action = pushAction;
			pushData.MoveDuration = model.GameMoves.LastOrDefault().HoldDuration;
			pushData.OpponentName = this.buddyUser.Name;
			pushData.ThumbnailUri = thumbnail;
			bool messageResult = await SendPushAsync (pushData, model.OtherUser(this.buddyUser.ID));

			return messageResult && metadata;
		}

		public async Task<IList<User>> GetMyFriendsFQLAsync()
		{
			var query = @"SELECT uid FROM user WHERE is_app_user = '1' AND uid IN (SELECT uid2 FROM friend WHERE uid1 = me())";
			var results = (IDictionary<string, object>)(await this.facebookClient.GetTaskAsync ("fql", new { q = query }));

			var friendsFacebookIds = new List<string> ();
			var list = (IEnumerable<object>)results ["data"];
			foreach (var friend in list) {
				var friendData = (IDictionary<string, object>)friend;
				var userId = (string)friendData ["uid"];
				friendsFacebookIds.Add (facebookMetadataPrefix + userId);
			}

			var resultList = new List<User> ();
			int index = 0;
			while (index < friendsFacebookIds.Count) {
				string searchString = GetMetadataString (friendsFacebookIds, ref index);
				var metadata = await this.buddyUser.IdentityValues.CheckForValuesAsync (searchString);
				foreach (var metadataResult in metadata) {
					if (metadataResult.Found) {
						var user = await  this.buddyUser.FindUserAsync (metadataResult.BelongsToUserId);
						resultList.Add (user);
					}
				}
			}

			return resultList;
		}

		public async Task<IList<User>> GetMyFriendsAsync()
		{
			var result = (IDictionary<string, object>) (await this.facebookClient.GetTaskAsync ("me/friends"));
			var friendsFacebookIds = new List<string> ();
			var list = (IEnumerable<object>)result ["data"];
			foreach (var friend in list) {
				var friendData = (IDictionary<string, object>)friend;
				var userName = (string)friendData ["name"];
				var userId = (string)friendData ["id"];
				friendsFacebookIds.Add (facebookMetadataPrefix + userId);
				Console.WriteLine ("Friend is " + userName + " - " + userId);
			}

			var resultList = new List<User> ();
			int index = 0;
			while (index < friendsFacebookIds.Count) {
				string searchString = GetMetadataString (friendsFacebookIds, ref index);
				var metadata = await this.buddyUser.IdentityValues.CheckForValuesAsync (searchString);
				foreach (var metadataResult in metadata) {
					if (metadataResult.Found) {
						var user = await  this.buddyUser.FindUserAsync (metadataResult.BelongsToUserId);
						resultList.Add (user);
					}
				}
			}

			return resultList;
		}

		public async Task<User> GetUserAsync(int id)
		{
			if (id == this.buddyUser.ID) {
				return this.buddyUser;
			}
			return await this.buddyUser.FindUserAsync (id);
		}

//		public async Task<long> UploadLoserPhotoAsync(Stream photoStream, string message, string gameGUID, User opponentUserModel, out Picture picture) {
//			var userExtraData = JsonConvert.DeserializeObject<UserExtraData> (this.buddyUser.ApplicationTag);
//			var opponentExtraData = JsonConvert.DeserializeObject<UserExtraData> (opponentUserModel.ApplicationTag);
//
//			var album = await this.buddyUser.PhotoAlbums.GetAsync (userExtraData.UploadAlbumId);
//			picture = await album.AddPictureAsync (photoStream, message, 0, 0, gameGUID);
//			var virtualAlbum = await buddyUser.VirtualAlbums.GetAsync (opponentExtraData.WinnerAblumVirtualId);
//			var imageId = await virtualAlbum.AddPictureAsync (picture);
//
//			return imageId;
//		}

		private string GetMetadataString(IList<string> ids, ref int endIndex)
		{
			string data = "";
			int startIndex = endIndex;
			endIndex = ids.Count;
			for (int i=startIndex; i<ids.Count; i++)
			{
				if (data.Length + ids [i].Length <= 100) {
					data += ids [i] + ";";
					endIndex = i + 1;
				} else {
					break;
				}
			}

			return data.Substring(0, data.Length - 1); // cut off the last ;
		}

		public async Task<byte[]> GetLoserPictureImageStreamAsync(long imgId, User winnerUserModel, string gameGUID) {
			var winnerExtraData = JsonConvert.DeserializeObject<UserExtraData> (winnerUserModel.ApplicationTag);
			var album = await this.buddyUser.VirtualAlbums.GetAsync ((int)(winnerExtraData.WinnerAblumVirtualId));
			var picture = album.Pictures.Where (p => p.AppTag == gameGUID).FirstOrDefault();
			// var picture = await this.buddyUser.GetPictureAsync ((int) imgId);
			if (picture != null) {
				var webClient = new WebClient ();
				var data = await webClient.DownloadDataTaskAsync (picture.FullUrl);
				return data;
			}

			return null;
		}

		public async Task<bool> RegisterAndroidPushAsync(string registrationId) {
			var result = await this.buddyUser.PushNotifications.Android.RegisterDeviceAsync (registrationId, this.buddyUser.ID.ToString ());
			return result;
		}

		public async Task<IList<PicturePublic>> GetMyWinnerPicturesAsync() {
			var userExtraData = JsonConvert.DeserializeObject<UserExtraData> (this.buddyUser.ApplicationTag);
			var photoAlbum = await this.buddyUser.VirtualAlbums.GetAsync (userExtraData.WinnerAblumVirtualId);
			return photoAlbum.Pictures.ToList ();
		}
	}
}

