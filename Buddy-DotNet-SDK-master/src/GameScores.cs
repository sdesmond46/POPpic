using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BuddyServiceClient;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Buddy
{
    /// <summary>
    /// Represents a class that can be used to add, retrieve or delete game scores for any user in the system.
    /// <example>
    /// <code>
    ///     BuddyClient client = new BuddyClient("APPNAME", "APPPASS");
    ///     client.LoginAsync((user, state) => {
    ///       
    ///         user.GameBoards.AddAsync(null, 100, "My Board", "Master", appTag: "MyTag");
    ///     
    ///         client.GameBoards.GetHighScores((r, state2) => {
    ///             var scores = r;
    ///         }, "My Board");
    ///     }, "username", "password", );
    /// </code>
    /// </example>
    /// </summary>
    public class GameScores : BuddyBase
    {


        public User User {
            get;
            private set;
        }

        internal GameScores (BuddyClient client, AuthenticatedUser authUser, User user)
            : base(client, authUser)
        {
            this.User = user;

        }

        /// <summary>
        /// Add a new score for this user.
        /// </summary>
        /// <param name="callback">The callback to call when this method completes. The first parameter is true on success, false otherwise.</param>
        /// <param name="score">The numeric value of the score.</param>
        /// <param name="board">The optional name of the game board.</param>
        /// <param name="rank">The optional rank for this score. This can be used for adding badges, achievements, etc.</param>
        /// <param name="latitude">The optional latitude for this score.</param>
        /// <param name="longitude">The optional longitude for this score.</param>
        /// <param name="oneScorePerPlayer">The optional one-score-per-player paramter. Setting this to true will always update the score for this user, instead of creating a new one.</param>
        /// <param name="appTag">An optional application tag for this score.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of AddAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult AddAsync (Action<bool, BuddyCallbackParams> callback, double score, string board = null, string rank = null, double latitude = 0.0,
                            double longitude = 0.0, bool oneScorePerPlayer = false, string appTag = null, object state = null)
        {
            AddInternal (score, board, rank, latitude, longitude, oneScorePerPlayer, appTag, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void AddInternal (double score, string board, string rank, double latitude,
                    double longitude, bool oneScorePerPlayer, string appTag, Action<BuddyCallResult<bool>> callback)
        {
            if (latitude > 90.0 || latitude < -90.0)
                throw new ArgumentException ("Can't be bigger than 90.0 or smaller than -90.0.", "atLatitude");
            if (longitude > 180.0 || longitude < -180.0)
                throw new ArgumentException ("Can't be bigger than 180.0 or smaller than -180.0.", "atLongitude");

            this.Client.Service.Game_Score_Add (this.Client.AppName, this.Client.AppPassword, AuthUser != null ? AuthUser.Token : User.ID.ToString (),
                    latitude.ToString (CultureInfo.InvariantCulture), longitude.ToString (CultureInfo.InvariantCulture), rank == null ? "" : rank, score.ToString (CultureInfo.InvariantCulture), board == null ? "" : board, appTag == null ? "" : appTag,
                    oneScorePerPlayer ? "1" : "0", (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create (default(bool), bcr.Error));
                    return;
                }
                if (result == "1") {
                    callback (BuddyResultCreator.Create (true, bcr.Error));
                    return;
                } else {
                    callback (BuddyResultCreator.Create (false, bcr.Error));
                    return;
                }
                ;
            });
            return;

        }

        /// <summary>
        /// Delete all scores for this user.
        /// </summary>
        /// <param name="callback">The callback to call when this method completes. The first parameter is true on success, false otherwise.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of DeleteAllAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult DeleteAllAsync (Action<bool, BuddyCallbackParams> callback, object state = null)
        {
            DeleteAllInternal ((bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void DeleteAllInternal (Action<BuddyCallResult<bool>> callback)
        {
            this.Client.Service.Game_Score_DeleteAllScoresForUser (this.Client.AppName, this.Client.AppPassword,
                    AuthUser != null ? AuthUser.Token : User.ID.ToString (), (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create (default(bool), bcr.Error));
                    return;
                }
                if (result == "1") {
                    callback (BuddyResultCreator.Create (true, bcr.Error));
                    return;
                } else {
                    callback (BuddyResultCreator.Create (false, bcr.Error));
                    return;
                }
                ;
            });
            return;

        }

       

        /// <summary>
        /// Return all score entries for this user.
        /// </summary>
        /// <param name="callback">The callback to call when this method completes. The first parameter is a list of game score entries.</param>
        /// <param name="recordLimit">Limit the number of entries returned.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetAllAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetAllAsync (Action<List<GameScore>, BuddyCallbackParams> callback, int recordLimit = 100, object state = null)
        {
            GetAllInternal (recordLimit, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void GetAllInternal (int recordLimit, Action<BuddyCallResult<List<GameScore>>> callback)
        {
            if (recordLimit <= 0)
                throw new ArgumentException ("Can't be smaller or equal to zero.", "recordLimit");

            this.Client.Service.Game_Score_GetScoresForUser (this.Client.AppName, this.Client.AppPassword,
                    AuthUser != null ? AuthUser.Token : User.ID.ToString (), recordLimit.ToString (), (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<List<GameScore>> (null, bcr.Error));
                    return;
                }
                List<GameScore> scores = new List<GameScore> ();
                foreach (var d in result)
                    scores.Add (new GameScore (this.Client, d));
                {
                    callback (BuddyResultCreator.Create (scores, bcr.Error));
                    return; }
                ;
            });
            return;

        }
    }
}
