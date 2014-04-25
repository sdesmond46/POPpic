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
    /// Represents a collection of friends. Use the AuthenticatedUser.Friends property to access this object.
    /// <example>
    /// <code>
    ///     BuddyClient client = new BuddyClient("APPNAME", "APPPASS");
    ///     
    ///     // Create two users
    ///     client.CreateAsync((user, state) => { 
    ///         client.CreateAsync((user2, state2) => { 
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
    public class Friends : BuddyBase
    {
        protected override bool AuthUserRequired {
            get {
                return true;
            }
        }



        /// <summary>
        /// Gets a list of friend requests that are still pending for this user.
        /// </summary>
        public FriendRequests Requests { get; protected set; }

        internal Friends (BuddyClient client, AuthenticatedUser user)
            : base(client, user)
        {
            this.Requests = new FriendRequests (client, user);
        }


        /// <summary>
        /// Returns the list of friends for the user.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is a list of users.</param>
        /// <param name="afterDate">Filter the list by friends added 'afterDate'.</param>
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
            this.Client.Service.Friends_Friends_GetList (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token,
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
        /// Remove a user from the current list of friends.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true if the user was remove from the list, false otherwise.</param>
        /// <param name="user">The user to remove from the friends list. Must be already on the list and can't be null.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of RemoveAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult RemoveAsync (Action<bool, BuddyCallbackParams> callback, User user, object state = null)
        {
            RemoveInternal (user, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void RemoveInternal (User user, Action<BuddyCallResult<bool>> callback)
        {
            if (user == null)
                throw new ArgumentNullException ("user");

            this.Client.Service.Friends_Friends_Remove (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token, user.ID.ToString (), (bcr) =>
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
    }
}
