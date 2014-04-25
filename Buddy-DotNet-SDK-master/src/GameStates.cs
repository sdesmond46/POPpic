using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BuddyServiceClient;
using System.Collections.ObjectModel;

namespace Buddy
{
    /// <summary>
    /// Represents a class that can be used to add, retrieve or delete game state data for any user in the system.
    /// <example>
    /// <code>
    ///     BuddyClient client = new BuddyClient("APPNAME", "APPPASS");
    ///     client.LoginAsync((user, state) => {
    ///       
    ///         user.GameState.AddAsync("MyGameState", "MyGameStateValue", (state, p)=>{});
    ///     
    ///         user.GameState.GetAsync((s, state2) => {
    ///             var value = s.Value;
    ///         }, "MyGameState");
    ///     }, "username", "password", );
    /// </code>
    /// </example>
    /// </summary>
    public class GameStates : BuddyBase
    {
        protected override bool AuthUserRequired {
            get {
                return true;
            }
        }

        protected User User { get; set; }

        internal GameStates (BuddyClient client, User user)
            : base(client)
        {
            if (user == null)
                throw new ArgumentNullException ("user");
            this.User = user;
        }




        /// <summary>
        /// Adds a key/value pair to the User GameState.
        /// </summary>
        /// <param name="callback">The callback to call when this method completes. The first parameter is true on success, false otherwise.</param>
        /// <param name="gameStateKey">The game state key.</param>
        /// <param name="gameStateValue">The value to persist.</param>
        /// <param name="appTag">An optional application tag for this score.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of AddAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult AddAsync (Action<bool, BuddyCallbackParams> callback, string gameStateKey, string gameStateValue, string appTag = "", object state = null)
        {
            AddInternal (gameStateKey, gameStateValue, appTag, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void AddInternal (string gameStateKey, string gameStateValue, string appTag, Action<BuddyCallResult<bool>> callback)
        {
            if (String.IsNullOrEmpty (gameStateKey))
                throw new ArgumentException ("gameStateKey", "Key can not be null or empty.");
            if (gameStateValue == null)
                throw new ArgumentNullException ("gameStateValue", "Value can not be null.");



            this.Client.Service.Game_State_Add (this.Client.AppName, this.Client.AppPassword, User.TokenOrId, gameStateKey, gameStateValue, appTag ?? "", (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None && bcr.Error != BuddyError.ServiceErrorNegativeOne) {
                    callback (BuddyResultCreator.Create (false, bcr.Error));
                    return;
                }
                callback (BuddyResultCreator.Create (result == "1", BuddyError.None));
                return;
            });
            return;

        }



        /// <summary>
        /// Get a GameState item with a key. The key can't be null or an empty string.
        /// </summary>
        /// <param name="gameStateKey">The gameStateKey to use to reference the GameState item.</param>
        /// <param name="callback">The async callback to call on success or error. The first parameter is the state value or null if it doesn't exist.</param>
        /// <exception cref="System.ArgumentException">When key is null or empty.</exception>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetAsync (Action<GameState, BuddyCallbackParams> callback, string gameStateKey, object state = null)
        {
            GetInternal (gameStateKey, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void GetInternal (string gameStateKey, Action<BuddyCallResult<GameState>> callback)
        {
            if (String.IsNullOrEmpty (gameStateKey))
                throw new ArgumentException ("gameStateKey", "Can't be null or empty.");


            this.Client.Service.Game_State_Get (
                        this.Client.AppName,
                        this.Client.AppPassword,
                        User.TokenOrId, gameStateKey, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<GameState> (null, bcr.Error));
                    return;
                }
                if (result.Length == 0) {
                    callback (BuddyResultCreator.Create<GameState> (null, bcr.Error));
                    return;
                }
                ;

                {
                    callback (BuddyResultCreator.Create (new GameState (result [0]), bcr.Error));
                    return; }
                ;

            });
            return;

        }

      

        /// <summary>
        /// Update a GameState value.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true on success, false otherwise.</param>
        /// <param name="gameStateKey">The key to update.</param>
        /// <param name="gameStateValue">The value to update.</param>
        /// <param name="newAppTag">An optional new application tag for the value.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of UpdateAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult UpdateAsync (Action<bool, BuddyCallbackParams> callback, string gameStateKey, string gameStateValue, string newAppTag = "", object state = null)
        {
            UpdateInternal (gameStateKey, gameStateValue, newAppTag, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void UpdateInternal (string gameStateKey, string gameStateValue, string newAppTag, Action<BuddyCallResult<bool>> callback)
        {
            if (String.IsNullOrEmpty (gameStateKey))
                throw new ArgumentException ("gameStateValue", "Can't be null or empty.");
            if (gameStateValue == null)
                throw new ArgumentNullException ("gameState", "gameState can not be null.");


            this.Client.Service.Game_State_Update (this.Client.AppName, this.Client.AppPassword, User.TokenOrId, gameStateKey,
                        gameStateValue, newAppTag == null ? "" : newAppTag, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None && bcr.Error != BuddyError.ServiceErrorNegativeOne) {
                    callback (BuddyResultCreator.Create (false, bcr.Error));
                    return;
                }
                callback (BuddyResultCreator.Create (result == "1", BuddyError.None));
                return;
            });
            return;

        }


    

        /// <summary>
        /// Remove a GameState key.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true on success, false otherwise.</param>
        /// <param name="gameStateKey">The key to remove from the GameState.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of RemoveAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult RemoveAsync (Action<bool, BuddyCallbackParams> callback, string gameStateKey, object state = null)
        {
            RemoveInternal (gameStateKey, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void RemoveInternal (string gameStateKey, Action<BuddyCallResult<bool>> callback)
        {
            if (String.IsNullOrEmpty (gameStateKey))
                throw new ArgumentException ("gameStateKey", "gameStateKey can not be null or empty");

            this.Client.Service.Game_State_Remove (this.Client.AppName, this.Client.AppPassword, User.TokenOrId, gameStateKey, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None && bcr.Error != BuddyError.ServiceErrorNegativeOne) {
                    callback (BuddyResultCreator.Create (false, bcr.Error));
                    return;
                }
                callback (BuddyResultCreator.Create (result == "1", BuddyError.None));
                return;
            });
            return;

        }



        /// <summary>
        /// Get all GameState keys and values.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is a Dictionary of name/value pairs for this User's GameState.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetAllAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetAllAsync (Action<Dictionary<string, GameState>, BuddyCallbackParams> callback, object state = null)
        {
            GetAllInternal ((bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void GetAllInternal (Action<BuddyCallResult<Dictionary<string, GameState>>> callback)
        {
            this.Client.Service.Game_State_GetAll (this.Client.AppName, this.Client.AppPassword, User.TokenOrId,
                       (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<Dictionary<string, GameState>> (null, bcr.Error));
                    return;
                }
                Dictionary<string, GameState> dict = new Dictionary<string, GameState> ();

                foreach (var d in result) {
                    dict [d.StateKey] = new GameState (d);

                }
                ;

                {
                    callback (BuddyResultCreator.Create (dict, bcr.Error));
                    return; }
                ;
            });
            return;

        }
    }
}
