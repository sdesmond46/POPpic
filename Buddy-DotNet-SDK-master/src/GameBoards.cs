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
    /// Represents an object the can be used to retrieve Buddy Game Boards and Scores.
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
    ///     }, "username", "password");
    /// </code>
    /// </example>
    /// </summary>
    public class GameBoards : BuddyBase
    {

        internal GameBoards (BuddyClient client)
            : base(client)
        {

        }


        /// <summary>
        /// Gets a list of high scores for a specific game board.
        /// </summary>
        /// <param name="callback">The callback to call when this method completes. The first parameter is a list of game scores.</param>
        /// <param name="boardName">The board name can be a specific string or a 'LIKE' pattern using %.</param>
        /// <param name="recordLimit">The maximum number of scores to return.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetHighScoresAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetHighScoresAsync (Action<List<GameScore>, BuddyCallbackParams> callback, string boardName, int recordLimit = 100, object state = null)
        {
            GetHighScoresInternal (boardName, recordLimit, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void GetHighScoresInternal (string boardName, int recordLimit, Action<BuddyCallResult<List<GameScore>>> callback)
        {
            if (boardName == null)
                throw new ArgumentNullException ("boardName");
            if (recordLimit <= 0)
                throw new ArgumentException ("Can't be smaller or equal to zero.", "recordLimit");

            this.Client.Service.Game_Score_GetBoardHighScores (this.Client.AppName, this.Client.AppPassword,
                    boardName, recordLimit.ToString (), (bcr) =>
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

        internal void GetLowScoresInternal(string boardName, int recordLimit, Action<BuddyCallResult<List<GameScore>>> callback)
        {
            if (boardName == null)
                throw new ArgumentNullException("boardName");
            if (recordLimit <= 0)
                throw new ArgumentException("Can't be smaller or equal to zero.", "recordLimit");

            this.Client.Service.Game_Score_GetBoardLowScores(this.Client.AppName, this.Client.AppPassword,
                    boardName, recordLimit.ToString(), (bcr) =>
                    {
                        var result = bcr.Result;
                        if (bcr.Error != BuddyError.None)
                        {
                            callback(BuddyResultCreator.Create<List<GameScore>>(null, bcr.Error));
                            return;
                        }
                        List<GameScore> scores = new List<GameScore>();
                        foreach (var d in result)
                            scores.Add(new GameScore(this.Client, d));
                        {
                            callback(BuddyResultCreator.Create(scores, bcr.Error));
                            return;
                        }
                        ;
                    });
            return;

        }

        /// <summary>
        /// Search for game scores based on a number of different parameters.
        /// </summary>
        /// <param name="callback">The callback to call when this method completes. The first parameter is a list of game scores.</param>
        /// <param name="user">Optionally limit the search to a spcific user.</param>
        /// <param name="distanceInMeters">Optionally specify a distance from a lat/long to search on. By default this is ignored.</param>
        /// <param name="latitude">Optional latitude where we can start the search.</param>
        /// <param name="longitude">Optional longitude where we can start the search.</param>
        /// <param name="recordLimit">Optionally limit the number of records returned by this search.</param>
        /// <param name="boardName">Optionally filter on a specific board name.</param>
        /// <param name="daysOld">Optionally only return scores that are X number of days old.</param>
        /// <param name="minimumScore">Optionally only return scores that are above a certain minimum score.</param>
        /// <param name="appTag">Optionally return only scores that have a certain app tag.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of FindScoresAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult FindScoresAsync (Action<List<GameScore>, BuddyCallbackParams> callback, User user = null, int distanceInMeters = -1, double latitude = 0.0,
                            double longitude = 0.0, int recordLimit = 100, string boardName = "", int daysOld = 999999, double minimumScore = -1.0, string appTag = "", object state = null)
        {
            FindScoresInternal (user, distanceInMeters, latitude, longitude, recordLimit, boardName, daysOld, minimumScore, appTag, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void FindScoresInternal (User user, int distanceInMeters, double latitude,
                    double longitude, int recordLimit, string boardName, int daysOld, double minimumScore, string appTag, Action<BuddyCallResult<List<GameScore>>> callback)
        {
            if (latitude > 90.0 || latitude < -90.0)
                throw new ArgumentException ("Can't be bigger than 90.0 or smaller than -90.0.", "atLatitude");
            if (longitude > 180.0 || longitude < -180.0)
                throw new ArgumentException ("Can't be bigger than 180.0 or smaller than -180.0.", "atLongitude");

            this.Client.Service.Game_Score_SearchScores (this.Client.AppName, this.Client.AppPassword, user == null ? "-1" : user.ID.ToString (),
                    distanceInMeters.ToString (), latitude.ToString (CultureInfo.InvariantCulture), longitude.ToString (CultureInfo.InvariantCulture), recordLimit.ToString (), boardName, daysOld.ToString (),
                    minimumScore.ToString (CultureInfo.InvariantCulture), appTag, (bcr) =>
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

#if AWAIT_SUPPORTED

        /// <summary>
        /// Gets a list of high scores for a specific game board.
        /// </summary>
        /// <param name="boardName">The board name can be a specific string or a 'LIKE' pattern using %.</param>
        /// <param name="recordLimit">The maximum number of scores to return.</param>
        /// <returns>A Task&lt;IEnumerable&lt;GameScore&gt; &gt;that can be used to monitor progress on this call.</returns>
        public  System.Threading.Tasks.Task<IEnumerable<GameScore>> GetHighScoresAsync( string boardName, int recordLimit = 100)
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<IEnumerable<GameScore>>();
            GetHighScoresInternal(boardName, recordLimit, (bcr) =>
            {
                if (bcr.Error != BuddyServiceClient.BuddyError.None)
                {
                    tcs.TrySetException(new BuddyServiceException(bcr.Error));
                }
                else
                {
                    tcs.TrySetResult(bcr.Result);
                }
            });
            return tcs.Task;
        }

        /// <summary>
        /// Gets a list of lowest scores for a specific game board.
        /// </summary>
        /// <param name="boardName">The board name can be a specific string or a 'LIKE' pattern using %.</param>
        /// <param name="recordLimit">The maximum number of scores to return.</param>
        /// <returns>A Task&lt;IEnumerable&lt;GameScore&gt; &gt;that can be used to monitor progress on this call.</returns>
        public System.Threading.Tasks.Task<IEnumerable<GameScore>> GetLowScoresAsync(string boardName, int recordLimit = 100)
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<IEnumerable<GameScore>>();
            GetLowScoresInternal(boardName, recordLimit, (bcr) =>
            {
                if (bcr.Error != BuddyServiceClient.BuddyError.None)
                {
                    tcs.TrySetException(new BuddyServiceException(bcr.Error));
                }
                else
                {
                    tcs.TrySetResult(bcr.Result);
                }
            });
            return tcs.Task;
        }

        /// <summary>
        /// Search for game scores based on a number of different parameters.
        /// </summary>
        /// <param name="user">Optionally limit the search to a spcific user.</param>
        /// <param name="distanceInMeters">Optionally specify a distance from a lat/long to search on. By default this is ignored.</param>
        /// <param name="latitude">Optional latitude where we can start the search.</param>
        /// <param name="longitude">Optional longitude where we can start the search.</param>
        /// <param name="recordLimit">Optionally limit the number of records returned by this search.</param>
        /// <param name="boardName">Optionally filter on a specific board name.</param>
        /// <param name="daysOld">Optionally only return scores that are X number of days old.</param>
        /// <param name="minimumScore">Optionally only return scores that are above a certain minimum score.</param>
        /// <param name="appTag">Optionally return only scores that have a certain app tag.</param>
        /// <returns>A Task&lt;IEnumerable&lt;GameScore&gt; &gt;that can be used to monitor progress on this call.</returns>
        public  System.Threading.Tasks.Task<IEnumerable<GameScore>> FindScoresAsync(Buddy.User user = null, int distanceInMeters = -1, double latitude = 0, double longitude = 0, int recordLimit = 100, string boardName = "", int daysOld = 999999, double minimumScore = -1, string appTag = "")
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<IEnumerable<GameScore>>();
            this.FindScoresInternal(user, distanceInMeters, latitude, longitude, recordLimit, boardName, daysOld, minimumScore, appTag, (bcr) =>
            {
                if (bcr.Error != BuddyServiceClient.BuddyError.None)
                {
                    tcs.TrySetException(new BuddyServiceException(bcr.Error));
                }
                else
                {
                    tcs.TrySetResult(bcr.Result);
                }
            });
            return tcs.Task;
        }
#endif
    }
}
