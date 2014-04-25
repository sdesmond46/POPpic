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
    /// Represents a collection of friend requests. Use the Add method to request a friend connection from another user.
    /// <example>
    /// <code>
    ///     BuddyClient client = new BuddyClient("APPNAME", "APPPASS");
    ///     
    ///     // Create two users
    ///     client.CreateUserAsync((user, state) => { 
    ///         client.CreateUserAsync((user2, state2) => { 
    ///             
    ///             // user sends a friend request to user2.
    ///             user.Friends.FriendRequests.AddAsync((r, state3) => {
    ///             
    ///                 // user2 accepts the friend request.
    ///                 user2.FriendRequests.AcceptAsync(null, user);
    ///             }, user2);
    ///         }, "username2", "password2");
    ///     }, "username", "password");
    /// </code>
    /// </example>
    /// </summary>
    public class FriendRequests : BuddyBase
    {
        protected override bool AuthUserRequired {
            get {
                return true;
            }
        }

        internal FriendRequests (BuddyClient client, AuthenticatedUser user)
            : base(client, user)
        {
        }

      
        /// <summary>
        /// Add a friend request to a user.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true if the friend request was added, false otherwise.</param>
        /// <param name="user">The user to send the request to, can't be null.</param>
        /// <param name="appTag">Mark this request with an tag, can be used on the user's side to make a decision on whether to accept the request.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of AddAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult AddAsync (Action<bool, BuddyCallbackParams> callback, User user, string appTag = "", object state = null)
        {
            AddInternal (user, appTag, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void AddInternal (User user, string appTag, Action<BuddyCallResult<bool>> callback)
        {
            if (user == null)
                throw new ArgumentNullException ("user");

            this.Client.Service.Friends_FriendRequest_Add (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token, user.ID.ToString (), appTag, (bcr) =>
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
        /// A list of all users that have request to be friends with this user.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first paramater is a list of users that have pending friend requests.</param>
        /// <param name="afterDate">Filter the list by returning only the friend requests after a ceratin date.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetAllAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetAllAsync (Action<List<User>, BuddyCallbackParams> callback, DateTime afterDate = default(DateTime), object state = null)
        {
            GetAllInternal (afterDate, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void GetAllInternal (DateTime afterDate, Action<BuddyCallResult<List<User>>> callback)
        {
            this.Client.Service.Friends_FriendRequest_Get (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token,
                afterDate == DateTime.MinValue ? "1/1/1950" : afterDate.ToString (CultureInfo.InvariantCulture), (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<List<User>> (null, bcr.Error));
                    return;
                }
                List<User> friends = new List<Buddy.User> ();
                foreach (var d in result)
                    friends.Add (new User (this.Client, d, this.AuthUser.ID));
                {
                    callback (BuddyResultCreator.Create (friends, bcr.Error));
                    return; }
                ;
            });
            return;

        }

        /// <summary>
        /// Accept a friend request from a user.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true if the friend request was accepted, false otherwise (i.e. the user doesn't exist).</param>
        /// <param name="user">The user to accept as friend. Can't be null and must be on the friend requests list.</param>
        /// <param name="appTag">Tag this friend accept with a string.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of AcceptAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult AcceptAsync (Action<bool, BuddyCallbackParams> callback, User user, string appTag = "", object state = null)
        {
            AcceptInternal (user, appTag, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void AcceptInternal (User user, string appTag, Action<BuddyCallResult<bool>> callback)
        {
            if (user == null)
                throw new ArgumentNullException ("user");

            this.Client.Service.Friends_FriendRequest_Accept (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token, user.ID.ToString (), appTag, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None && bcr.Error != BuddyError.ServiceErrorNegativeOne) {
                    callback (BuddyResultCreator.Create (default(bool), bcr.Error));
                    return;
                }
                callback (BuddyResultCreator.Create (result == "1", BuddyError.None));
            });
            return;

        }


        /// <summary>
        /// Deny the friend request from a user.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. First parameter is true if the friend request was accepted, 
        /// false otherwise (i.e. user is not on the friends request list).</param>
        /// <param name="user">The user to deny the friend request from. User can't be null and must be on the friend request list.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of DenyAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult DenyAsync (Action<bool, BuddyCallbackParams> callback, User user, object state = null)
        {
            DenyInternal (user, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void DenyInternal (User user, Action<BuddyCallResult<bool>> callback)
        {
            if (user == null)
                throw new ArgumentNullException ("user");

            this.Client.Service.Friends_FriendRequest_Deny (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token, user.ID.ToString (), (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None && bcr.Error != BuddyError.ServiceErrorNegativeOne) {
                    callback (BuddyResultCreator.Create (default(bool), bcr.Error));
                    return;
                }
                callback (BuddyResultCreator.Create (result == "1", BuddyError.None));
            });
            return;

        }
    }
}
