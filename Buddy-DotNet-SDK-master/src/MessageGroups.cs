using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BuddyServiceClient;
using System.Collections.ObjectModel;

namespace Buddy
{
    /// <summary>
    /// Represents an object that can be used to create or query message groups for the app.
    /// <example>
    /// <code>
    ///     BuddyClient client = new BuddyClient("APPNAME", "APPPASS");
    ///     
    ///     // On WinPhone 7 - configure push for tiles and toast
    ///     client.LoginAsync((user, state) => {
    ///         user.Messages.Groups.CreateAsync((group, state) => { }, "My Group", true);
    ///     }, "username", "password");
    /// </code>
    /// </example>
    /// </summary>
    public class MessageGroups : BuddyBase
    {


        internal MessageGroups (BuddyClient client, AuthenticatedUser user)
            : base(client, user)
        {

        }

       
        /// <summary>
        /// Create a new message group.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is the Group that was create or null if a group with that name already exists.</param>
        /// <param name="name">The name of the new group, must be unique for the app.</param>
        /// <param name="openGroup">Optionally whether to make to group open for all user (anyone can join), or closed (only the owner can add users to it).</param>
        /// <param name="appTag">An optional application tag for this message group.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of CreateAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult CreateAsync (Action<MessageGroup, BuddyCallbackParams> callback, string name, bool openGroup, string appTag = "", object state = null)
        {
            CreateInternal (name, openGroup, appTag, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void CreateInternal (string name, bool openGroup, string appTag, Action<BuddyCallResult<MessageGroup>> callback)
        {
            if (String.IsNullOrEmpty (name))
                throw new ArgumentNullException ("name");

            this.Client.Service.GroupMessages_Manage_CreateGroup (this.Client.AppName, this.Client.AppPassword, AuthUser.Token, name,
                    openGroup ? "1" : "0", appTag == null ? "" : appTag, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<MessageGroup> (null, bcr.Error));
                    return;
                }
                {
                    callback (BuddyResultCreator.Create (new MessageGroup (this.Client, this.AuthUser, Int32.Parse (result), name), bcr.Error));
                    return; }
                ;
            });
            return;

        }

      

        /// <summary>
        /// Check if a group with this name already exists.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true if the group exists, false otherwise.</param>
        /// <param name="name">The name of the group to check for.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of CheckIfExistsAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult CheckIfExistsAsync (Action<bool, BuddyCallbackParams> callback, string name, object state = null)
        {
            CheckIfExistsInternal (name, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void CheckIfExistsInternal (string name, Action<BuddyCallResult<bool>> callback)
        {
            if (String.IsNullOrEmpty (name))
                throw new ArgumentNullException ("name");

            this.Client.Service.GroupMessages_Manage_CheckForGroup (this.Client.AppName, this.Client.AppPassword, AuthUser.Token,
                    name, (bcr) =>
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
        /// Get all message groups for this app.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is a list of message groups on success.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetAllAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetAllAsync (Action<List<MessageGroup>, BuddyCallbackParams> callback, object state = null)
        {
            GetAllInternal ((bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void GetAllInternal (Action<BuddyCallResult<List<MessageGroup>>> callback)
        {
            this.Client.Service.GroupMessages_Membership_GetAllGroups (this.Client.AppName, this.Client.AppPassword, AuthUser.Token, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<List<MessageGroup>> (null, bcr.Error));
                    return;
                }
                List<MessageGroup> groups = new List<MessageGroup> ();
                foreach (var d in result)
                    groups.Add (new MessageGroup (this.Client, this.AuthUser, d));
                {
                    callback (BuddyResultCreator.Create (groups, bcr.Error));
                    return; }
                ;
            });
            return;

        }


        /// <summary>
        /// Get all message groups that this user is part of.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is a list of message groups.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetMyAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetMyAsync (Action<List<MessageGroup>, BuddyCallbackParams> callback, object state = null)
        {
            GetMyInternal ((bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void GetMyInternal (Action<BuddyCallResult<List<MessageGroup>>> callback)
        {
            this.Client.Service.GroupMessages_Membership_GetMyList (this.Client.AppName, this.Client.AppPassword, AuthUser.Token, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<List<MessageGroup>> (null, bcr.Error));
                    return;
                }
                List<MessageGroup> groups = new List<MessageGroup> ();
                foreach (var d in result)
                    groups.Add (new MessageGroup (this.Client, this.AuthUser, d));
                {
                    callback (BuddyResultCreator.Create (groups, bcr.Error));
                    return; }
                ;
            });
            return;

        }
    }
}
