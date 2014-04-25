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
    /// Represents a player in a game. The Player object tracks game specific items such as board, ranks, and other data specific to building game leader boards and other game related constructs. 
    /// </summary>
    public class GamePlayers : BuddyBase
    {


        internal GamePlayers (BuddyClient client, AuthenticatedUser authUser)
            : base(client, authUser)
        {

        }

     

        /// <summary>
        /// Creates a new game Player object for an existing user in Buddy.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true on success, false otherwise.</param>
        /// <param name="name">The name of the new player.</param>
        /// <param name="board">An optional name of a "Board" for the game. Used for grouping scores together either by user group, levels, or some other method relevant to the game. Although optional, a value is recommended such as "Default" for use in later searches of scores. If no board is to be stored, then pass in null or leave empty.</param>
        /// <param name="rank">An optional ranking to associate with the score. Can be any string ie: descriptions of achievements, ranking strings like "excellent", etc. Pass in null or an empty string if you do not wish to store a rank</param>
        /// <param name="latitude">The latitude of the location where the Player object is being created.</param>
        /// <param name="longitude">The longitude of the location where the Player object is being created.</param>
        /// <param name="appTag">Optional metadata to store with the Player object. ie: a list of players, game state, etc. Leave empty or set to null if there is no data to store with the score.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of AddAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult AddAsync (Action<bool, BuddyCallbackParams> callback, string name, string board = "", string rank = "", double latitude = 0.0, double longitude = 0.0, string appTag = "", object state = null)
        {
            AddInternal (name, board, rank, latitude, longitude, appTag, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void AddInternal (string name, string board, string rank, double latitude, double longitude, string appTag, Action<BuddyCallResult<bool>> callback)
        {
            if (latitude > 90.0 || latitude < -90.0)
                throw new ArgumentException ("Can't be bigger than 90.0 or smaller than -90.0.", "atLatitude");
            if (longitude > 180.0 || longitude < -180.0)
                throw new ArgumentException ("Can't be bigger than 180.0 or smaller than -180.0.", "atLongitude");

            this.Client.Service.Game_Player_Add (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token, name, latitude.ToString (CultureInfo.InvariantCulture), longitude.ToString (CultureInfo.InvariantCulture), rank == null ? "" : rank,
                    board == null ? "" : board, appTag == null ? "" : appTag, (bcr) =>
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
        /// Updates one or more fields of an existing Player object which was previously created.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true on success, false otherwise.</param>
        /// <param name="name">The name of the new player.</param>
        /// <param name="board">An optional name of a "Board" for the game. Used for grouping scores together either by user group, levels, or some other method relevant to the game. Although optional, a value is recommended such as "Default" for use in later searches of scores. If no board is to be stored, then pass in null or leave empty.</param>
        /// <param name="rank">An optional ranking to associate with the score. Can be any string ie: descriptions of achievements, ranking strings like "excellent", etc. Pass in null or an empty string if you do not wish to store a rank</param>
        /// <param name="latitude">The latitude of the location where the Player object is being updated.</param>
        /// <param name="longitude">The longitude of the location where the Player object is being updated. </param>
        /// <param name="appTag">Optional metadata to store with the Player object. ie: a list of players, game state, etc. Leave empty or set to null if there is no data to store with the score.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of UpdateAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult UpdateAsync (Action<bool, BuddyCallbackParams> callback, string name, string board = "", string rank = "", double latitude = 0.0, double longitude = 0.0, string appTag = "", object state = null)
        {
            UpdateInternal (name, board, rank, latitude, longitude, appTag, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void UpdateInternal (string name, string board, string rank, double latitude, double longitude, string appTag, Action<BuddyCallResult<bool>> callback)
        {
            if (latitude > 90.0 || latitude < -90.0)
                throw new ArgumentException ("Can't be bigger than 90.0 or smaller than -90.0.", "atLatitude");
            if (longitude > 180.0 || longitude < -180.0)
                throw new ArgumentException ("Can't be bigger than 180.0 or smaller than -180.0.", "atLongitude");

            this.Client.Service.Game_Player_Update (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token, name, latitude.ToString (CultureInfo.InvariantCulture), longitude.ToString (CultureInfo.InvariantCulture), rank == null ? "" : rank,
                    board == null ? "" : board, appTag == null ? "" : appTag, (bcr) =>
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
        /// Delete the player object for this user.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true on success, false otherwise.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of DeleteAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult DeleteAsync (Action<bool, BuddyCallbackParams> callback, object state = null)
        {
            DeleteInternal ((bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void DeleteInternal (Action<BuddyCallResult<bool>> callback)
        {
            this.Client.Service.Game_Player_Delete (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token, (bcr) =>
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
        /// Get all the player info for this user.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is the player info for this user.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetInfoAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetInfoAsync (Action<GamePlayer, BuddyCallbackParams> callback, object state = null)
        {
            GetInfoInternal ((bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void GetInfoInternal (Action<BuddyCallResult<GamePlayer>> callback)
        {
            this.Client.Service.Game_Player_GetPlayerInfo (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<GamePlayer> (null, bcr.Error));
                    return;
                }
                if (result.Length == 0) {
                    callback (BuddyResultCreator.Create<GamePlayer> (null, bcr.Error));
                    return;
                } else {
                    callback (BuddyResultCreator.Create (new GamePlayer (this.Client, this.AuthUser, result [0]), bcr.Error));
                    return;
                }
            });
            return;

        }

        /// <summary>
        /// Searches for Player objects stored in the Buddy system. Searches can optionally be performed based on location.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is a list of player that were found.</param>
        /// <param name="searchDistanceInMeters">The radius (in meters) around the specified location in which to look for locations. Pass in -1 to ignore this field.  </param>
        /// <param name="latitude">The latitude of the location around which to search for locations within the specified SearchDistance.   </param>
        /// <param name="longitude">The longitude of the location around which to search for locations within the specified SearchDistance. </param>
        /// <param name="recordLimit">The maximum number of search results to return or -1 to return all search results.    </param>
        /// <param name="boardName">Searches for scores which contain the specified board. Leave empty or pass in null if no board filter is to be used.    </param>
        /// <param name="onlyForLastNumberOfDays">The number of days into the past for which to look for scores. ie: passing in 5 will filter scores to include those which were added/updated on or after 5 days ago. Pass in -1 to ignore this filter.    </param>
        /// <param name="minimumScore">The minimum score value to search for. Pass in -1 to ignore this filter. </param>
        /// <param name="appTag">Searches for scores with the specified ApplicationTag stored with them. Leave empty or pass in null to ignore this filter. </param>
        /// <param name="rank">Optionally search for a player rank.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of FindAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult FindAsync (Action<List<GamePlayer>, BuddyCallbackParams> callback, int searchDistanceInMeters = -1, double latitude = 0.0, double longitude = 0.0, int recordLimit = 100,
                            string boardName = "", int onlyForLastNumberOfDays = -1, int minimumScore = -1, string appTag = "", string rank = "", object state = null)
        {
            FindInternal (searchDistanceInMeters, latitude, longitude, recordLimit, boardName, onlyForLastNumberOfDays, minimumScore, appTag, rank, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void FindInternal (int searchDistanceInMeters, double latitude, double longitude, int recordLimit,
                    string boardName, int onlyForLastNumberOfDays, int minimumScore, string rank, string appTag, Action<BuddyCallResult<List<GamePlayer>>> callback)
        {
            if (latitude > 90.0 || latitude < -90.0)
                throw new ArgumentException ("Can't be bigger than 90.0 or smaller than -90.0.", "atLatitude");
            if (longitude > 180.0 || longitude < -180.0)
                throw new ArgumentException ("Can't be bigger than 180.0 or smaller than -180.0.", "atLongitude");

            this.Client.Service.Game_Player_SearchPlayers (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token, recordLimit.ToString (),
                    onlyForLastNumberOfDays.ToString (), appTag == null ? "" : appTag, rank == null ? "" : rank, boardName == null ? "" : boardName, searchDistanceInMeters.ToString (),
                    latitude.ToString (CultureInfo.InvariantCulture), longitude.ToString (CultureInfo.InvariantCulture), (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<List<GamePlayer>> (null, bcr.Error));
                    return;
                }
                List<GamePlayer> players = new List<GamePlayer> ();
                foreach (var d in result)
                    players.Add (new GamePlayer (this.Client, this.AuthUser, d));
                {
                    callback (BuddyResultCreator.Create (players, bcr.Error));
                    return; }
                ;
            });
            return;

        }

#if AWAIT_SUPPORTED
        
            /// <summary>
            /// Creates a new game Player object for an existing user in Buddy.
            /// </summary>
            /// <param name="name">The name of the new player.</param>
            /// <param name="board">An optional name of a "Board" for the game. Used for grouping scores together either by user group, levels, or some other method relevant to the game. Although optional, a value is recommended such as "Default" for use in later searches of scores. If no board is to be stored, then pass in null or leave empty.</param>
            /// <param name="rank">An optional ranking to associate with the score. Can be any string ie: descriptions of achievements, ranking strings like "excellent", etc. Pass in null or an empty string if you do not wish to store a rank</param>
            /// <param name="latitude">The latitude of the location where the Player object is being created.</param>
            /// <param name="longitude">The longitude of the location where the Player object is being created.</param>
            /// <param name="appTag">Optional metadata to store with the Player object. ie: a list of players, game state, etc. Leave empty or set to null if there is no data to store with the score.</param>
            /// <returns>A Task&lt;Boolean&gt;that can be used to monitor progress on this call.</returns>
           public System.Threading.Tasks.Task<Boolean> AddAsync( string name, string board = "", string rank = "", double latitude = 0, double longitude = 0, string appTag = "")
            {
                var tcs = new System.Threading.Tasks.TaskCompletionSource<Boolean>();
                this.AddInternal(name, board, rank, latitude, longitude, appTag, (bcr) =>
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
            /// Updates one or more fields of an existing Player object which was previously created.
            /// </summary>
            /// <param name="name">The name of the new player.</param>
            /// <param name="board">An optional name of a "Board" for the game. Used for grouping scores together either by user group, levels, or some other method relevant to the game. Although optional, a value is recommended such as "Default" for use in later searches of scores. If no board is to be stored, then pass in null or leave empty.</param>
            /// <param name="rank">An optional ranking to associate with the score. Can be any string ie: descriptions of achievements, ranking strings like "excellent", etc. Pass in null or an empty string if you do not wish to store a rank</param>
            /// <param name="latitude">The latitude of the location where the Player object is being updated.</param>
            /// <param name="longitude">The longitude of the location where the Player object is being updated. </param>
            /// <param name="appTag">Optional metadata to store with the Player object. ie: a list of players, game state, etc. Leave empty or set to null if there is no data to store with the score.</param>
            /// <returns>A Task&lt;Boolean&gt;that can be used to monitor progress on this call.</returns>
           public System.Threading.Tasks.Task<Boolean> UpdateAsync( string name, string board = "", string rank = "", double latitude = 0, double longitude = 0, string appTag = "")
            {
                var tcs = new System.Threading.Tasks.TaskCompletionSource<Boolean>();
                this.UpdateInternal(name, board, rank, latitude, longitude, appTag, (bcr) =>
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
            /// Delete the player object for this user.
            /// </summary>
            /// <returns>A Task&lt;Boolean&gt;that can be used to monitor progress on this call.</returns>
           public System.Threading.Tasks.Task<Boolean> DeleteAsync()
            {
                var tcs = new System.Threading.Tasks.TaskCompletionSource<Boolean>();
                this.DeleteInternal((bcr) =>
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
            /// Get all the player info for this user.
            /// </summary>
            /// <returns>A Task&lt;GamePlayer&gt;that can be used to monitor progress on this call.</returns>
           public System.Threading.Tasks.Task<GamePlayer> GetInfoAsync()
            {
                var tcs = new System.Threading.Tasks.TaskCompletionSource<GamePlayer>();
                this.GetInfoInternal((bcr) =>
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
            /// Searches for Player objects stored in the Buddy system. Searches can optionally be performed based on location.
            /// </summary>
            /// <param name="searchDistanceInMeters">The radius (in meters) around the specified location in which to look for locations. Pass in -1 to ignore this field.  </param>
            /// <param name="latitude">The latitude of the location around which to search for locations within the specified SearchDistance.   </param>
            /// <param name="longitude">The longitude of the location around which to search for locations within the specified SearchDistance. </param>
            /// <param name="recordLimit">The maximum number of search results to return or -1 to return all search results.    </param>
            /// <param name="boardName">Searches for scores which contain the specified board. Leave empty or pass in null if no board filter is to be used.    </param>
            /// <param name="onlyForLastNumberOfDays">The number of days into the past for which to look for scores. ie: passing in 5 will filter scores to include those which were added/updated on or after 5 days ago. Pass in -1 to ignore this filter.    </param>
            /// <param name="minimumScore">The minimum score value to search for. Pass in -1 to ignore this filter. </param>
            /// <param name="appTag">Searches for scores with the specified ApplicationTag stored with them. Leave empty or pass in null to ignore this filter. </param>
            /// <param name="rank">Optionally search for a player rank.</param>
            /// <returns>A Task&lt;IEnumerable&lt;GamePlayer&gt; &gt;that can be used to monitor progress on this call.</returns>
           public System.Threading.Tasks.Task<IEnumerable<GamePlayer>> FindAsync( int searchDistanceInMeters = -1, double latitude = 0, double longitude = 0, int recordLimit = 100, string boardName = "", int onlyForLastNumberOfDays = -1, int minimumScore = -1, string appTag = "", string rank = "")
            {
                var tcs = new System.Threading.Tasks.TaskCompletionSource<IEnumerable<GamePlayer>>();
                this.FindInternal(searchDistanceInMeters, latitude, longitude, recordLimit, boardName, onlyForLastNumberOfDays, minimumScore, appTag, rank, (bcr) =>
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
