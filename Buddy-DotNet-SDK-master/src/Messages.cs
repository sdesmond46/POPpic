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
    /// Represents an object that can be used to send message from one user to another.
    /// <example>
    /// <code>
    ///     BuddyClient client = new BuddyClient("APPNAME", "APPPASS");
    ///     
    ///     // On WinPhone 7 - configure push for tiles and toast
    ///     client.LoginAsync((user, state) => {
    ///         user.Messages.SendAsync((r, state) => { }, someOtherUser, "Some Message");
    ///     }, "username", "password");
    /// </code>
    /// </example>
    /// </summary>
    public class Messages : BuddyBase
    {

        public MessageGroups Groups { get; protected set; }

        internal Messages (BuddyClient client, AuthenticatedUser user)
            : base(client, user)
        {
            if (client == null)
                throw new ArgumentNullException ("client");

            this.Groups = new MessageGroups (client, user);

        }

      

        /// <summary>
        /// Send a message to a user from the current authenticated user.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true on success, false otherwise.</param>
        /// <param name="toUser">The user to send a message to.</param>
        /// <param name="message">The message to send, must be less then 200 characters.</param>
        /// <param name="appTag">An optional application tag to set for the message.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of SendAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult SendAsync (Action<bool, BuddyCallbackParams> callback, User toUser, string message, string appTag = "", object state = null)
        {
            SendInternal (toUser, message, appTag, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void SendInternal (User toUser, string message, string appTag, Action<BuddyCallResult<bool>> callback)
        {
            if (toUser == null)
                throw new ArgumentNullException ("toUser");
            if (message == null || message.Length > 200)
                throw new ArgumentException ("Can't be null or larger then 200 characters.", "message");

            this.Client.Service.Messages_Message_Send (this.Client.AppName, this.Client.AppPassword, AuthUser.Token, message, toUser.ID.ToString (),
                    appTag == null ? "" : appTag, (bcr) =>
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
        /// Get all received message by the current user.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is a list of messages that the user received.</param>
        /// <param name="afterDate">Optionally retreive only messages after a certain DateTime.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetReceivedAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetReceivedAsync (Action<List<Message>, BuddyCallbackParams> callback, DateTime afterDate = default(DateTime), object state = null)
        {
            GetReceivedInternal (afterDate, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void GetReceivedInternal (DateTime afterDate, Action<BuddyCallResult<List<Message>>> callback)
        {
            this.Client.Service.Messages_Messages_Get (this.Client.AppName, this.Client.AppPassword, AuthUser.Token,
                    afterDate == DateTime.MinValue ? "1/1/1950" : afterDate.ToString (CultureInfo.InvariantCulture), (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<List<Message>> (null, bcr.Error));
                    return;
                }
                List<Message> lst = new List<Message> ();
                foreach (var d in result)
                    lst.Add (new Message (d, this.AuthUser.ID));
                {
                    callback (BuddyResultCreator.Create (lst, bcr.Error));
                    return; }
                ;
            });
            return;

        }

        /// <summary>
        /// Get all sent message by the current user.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is a list of messages that the user sent.</param>
        /// <param name="afterDate">Optionally retreive only messages after a certain DateTime.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetSentAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetSentAsync (Action<List<Message>, BuddyCallbackParams> callback, DateTime afterDate = default(DateTime), object state = null)
        {
            GetSentInternal (afterDate, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void GetSentInternal (DateTime afterDate, Action<BuddyCallResult<List<Message>>> callback)
        {
            this.Client.Service.Messages_SentMessages_Get (this.Client.AppName, this.Client.AppPassword, AuthUser.Token,
                    afterDate == DateTime.MinValue ? "1/1/1950" : afterDate.ToString (CultureInfo.InvariantCulture), (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<List<Message>> (null, bcr.Error));
                    return;
                }
                List<Message> lst = new List<Message> ();
                foreach (var d in result)
                    lst.Add (new Message (d, this.AuthUser.ID));
                {
                    callback (BuddyResultCreator.Create (lst, bcr.Error));
                    return; }
                ;
            });
            return;

        }
    }
}
