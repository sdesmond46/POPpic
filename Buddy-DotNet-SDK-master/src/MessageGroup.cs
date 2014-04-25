using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using BuddyServiceClient;
using System.Globalization;

namespace Buddy
{
    /// <summary>
    /// Represents a single message group. Message groups are groups of users that can message each other. Groups can either be public, with anyone being able
    /// to join them, or private - where only the user that create the group can add other users to it.
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
    public class MessageGroup : BuddyBase
    {



        /// <summary>
        /// Gets the App unique ID of the message group.
        /// </summary>
        public int ID { get; protected set; }

        /// <summary>
        /// Gets the name of the message group.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets the DateTime the message group was created.
        /// </summary>
        public DateTime CreatedOn { get; protected set; }

        /// <summary>
        /// Gets the app tag that was associated with this message group.
        /// </summary>
        public string AppTag { get; protected set; }

        /// <summary>
        /// Gets the ID of the user that created this message group.
        /// </summary>
        public int OwnerUserID { get; protected set; }

        /// <summary>
        /// Gets a list of IDs of users that belong to this message group.
        /// </summary>
        public List<int> MemberUserIDs { get; protected set; }

        internal MessageGroup (BuddyClient client, AuthenticatedUser user, int groupId, string name)
            : base(client, user)
        {
            if (client == null)
                throw new ArgumentNullException ("client");
            this.ID = groupId;
            this.Name = name;
        }

        internal MessageGroup (BuddyClient client, AuthenticatedUser user, InternalModels.DataContract_GroupChatMemberships group)
            : base(client, user)
        {
            this.ID = Int32.Parse (group.ChatGroupID);
            this.Name = group.ChatGroupName;
            this.CreatedOn = Convert.ToDateTime (group.CreatedDateTime, CultureInfo.InvariantCulture);
            this.AppTag = group.ApplicationTag;
            this.OwnerUserID = Int32.Parse (group.OwnerUserID);
            this.MemberUserIDs = new List<int> ();
            foreach (string id in group.MemberUserIDList.Split(';'))
                this.MemberUserIDs.Add (Int32.Parse (id));
        }


        
        /// <summary>
        /// This method has the current user join this message group.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true on success, false otherwise.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of JoinAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult JoinAsync (Action<bool, BuddyCallbackParams> callback, object state = null)
        {
            JoinInternal ((bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void JoinInternal (Action<BuddyCallResult<bool>> callback)
        {
            this.Client.Service.GroupMessages_Membership_JoinGroup (this.Client.AppName, this.Client.AppPassword, AuthUser.Token,
                    this.ID.ToString (), (bcr) =>
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
        /// This methods has the current user leave this message group.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true on success, false otherwise.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of LeaveAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult LeaveAsync (Action<bool, BuddyCallbackParams> callback, object state = null)
        {
            LeaveInternal ((bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void LeaveInternal (Action<BuddyCallResult<bool>> callback)
        {
            this.Client.Service.GroupMessages_Membership_DepartGroup (this.Client.AppName, this.Client.AppPassword, AuthUser.Token,
                    this.ID.ToString (), (bcr) =>
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
        /// Add a user to this message group. 
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true on success, false otherwise.</param>
        /// <param name="userToAdd">The User to add to the message group.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of AddUserAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult AddUserAsync (Action<bool, BuddyCallbackParams> callback, User userToAdd, object state = null)
        {
            AddUserInternal (userToAdd, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void AddUserInternal (User userToAdd, Action<BuddyCallResult<bool>> callback)
        {
            this.Client.Service.GroupMessages_Membership_AddNewMember (this.Client.AppName, this.Client.AppPassword, AuthUser.Token,
                    this.ID.ToString (), userToAdd.ID.ToString (), (bcr) =>
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
        /// Remove a user from this message group.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true on success, false otherwise.</param>
        /// <param name="userToRemove">The user to remove from the group.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of RemoveUserAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult RemoveUserAsync (Action<bool, BuddyCallbackParams> callback, User userToRemove, object state = null)
        {
            RemoveUserInternal (userToRemove, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void RemoveUserInternal (User userToRemove, Action<BuddyCallResult<bool>> callback)
        {
            this.Client.Service.GroupMessages_Membership_RemoveUser (this.Client.AppName, this.Client.AppPassword, AuthUser.Token, userToRemove.ID.ToString (),
                    this.ID.ToString (), (bcr) =>
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
        /// Send a message to the entire message group.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is a map of users and a boolean indicating whether the message was successfully sent to them.</param>
        /// <param name="message">The message to send to this group. Must be less then 1000 characters.</param>
        /// <param name="latitude">The optional latitude from where this message was sent.</param>
        /// <param name="longitude">The optional longitude from where this message was sent.</param>
        /// <param name="appTag">An optional application tag for this message.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of SendMessageAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult SendMessageAsync (Action<Dictionary<int, bool>, BuddyCallbackParams> callback, string message, double latitude = 0.0, double longitude = 0.0, string appTag = "", object state = null)
        {
            SendMessageInternal (message, latitude, longitude, appTag, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void SendMessageInternal (string message, double latitude, double longitude, string appTag, Action<BuddyCallResult<Dictionary<int, bool>>> callback)
        {
            if (message == null || message.Length > 1000)
                throw new ArgumentException ("Can't be null or larger then 200 characters.", "message");
            if (latitude > 90.0 || latitude < -90.0)
                throw new ArgumentException ("Can't be bigger than 90.0 or smaller than -90.0.", "atLatitude");
            if (longitude > 180.0 || longitude < -180.0)
                throw new ArgumentException ("Can't be bigger than 180.0 or smaller than -180.0.", "atLongitude");

            this.Client.Service.GroupMessages_Message_Send (this.Client.AppName, this.Client.AppPassword, AuthUser.Token, this.ID.ToString (), message, latitude.ToString (CultureInfo.InvariantCulture),
                    longitude.ToString (CultureInfo.InvariantCulture), appTag == null ? "" : appTag, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<Dictionary<int, bool>> (null, bcr.Error));
                    return;
                }
                Dictionary<int, bool> dict = new Dictionary<int, bool> ();
                foreach (var d in result) {
                    foreach (var id in d.MemberUserIDList.Split(';')) {
                        int userId;
                        if (Int32.TryParse (id, out userId)) {
                            dict.Add (userId, d.SendResult == "1");
                        }
                    }
                }
                {
                    callback (BuddyResultCreator.Create (dict, bcr.Error));
                    return; }
                ;
            });
            return;

        }


        /// <summary>
        /// Get all messages this group has received.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is a list of messages the group has received.</param>
        /// <param name="afterDate">Optionally return only messages sent after this date.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetReceivedAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetReceivedAsync (Action<List<GroupMessage>, BuddyCallbackParams> callback, DateTime afterDate = default(DateTime), object state = null)
        {
            GetReceivedInternal (afterDate, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void GetReceivedInternal (DateTime afterDate, Action<BuddyCallResult<List<GroupMessage>>> callback)
        {
            this.Client.Service.GroupMessages_Message_Get (this.Client.AppName, this.Client.AppPassword, AuthUser.Token,
                    this.ID.ToString (), afterDate == DateTime.MinValue ? "1/1/1950" : afterDate.ToString (CultureInfo.InvariantCulture), (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<List<GroupMessage>> (null, bcr.Error));
                    return;
                }
                List<GroupMessage> lst = new List<GroupMessage> ();
                foreach (var d in result)
                    lst.Add (new GroupMessage (this.Client, d, this));
                {
                    callback (BuddyResultCreator.Create (lst, bcr.Error));
                    return; }
                ;
            });
            return;

        }

      

        /// <summary>
        /// Delete this message group.
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
            this.Client.Service.GroupMessages_Manage_DeleteGroup (this.Client.AppName, this.Client.AppPassword, this.AuthUser.ID.ToString (),
                    this.ID.ToString (), (bcr) =>
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
