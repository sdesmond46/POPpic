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
    /// Represents an object that can be used to register Apple devices for push notifications. The class can also be used to query for all registered devices and
    /// to send them notifications.
    /// </summary>
    public class NotificationsApple : BuddyBase
    {
        protected override bool AuthUserRequired {
            get {
                return true;
            }
        }

        internal NotificationsApple (BuddyClient client, AuthenticatedUser user)
            : base(client, user)
        {

        }


        /// <summary>
        /// Register an Apple device for notificatons with Buddy. 
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true on success, false otherwise.</param>
        /// <param name="appleDeviceToken">A token provided by the Apple Push Notification Service (APNs) which identifies the device to register (analogous to a phone number).</param>
        /// <param name="groupName">Register this device as part of a group, so that you can send the whole group messages.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of RegisterDeviceAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult RegisterDeviceAsync (Action<bool, BuddyCallbackParams> callback, string appleDeviceToken, string groupName = "", object state = null)
        {
            RegisterDeviceInternal (appleDeviceToken, groupName, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void RegisterDeviceInternal (string appleDeviceToken, string groupName, Action<BuddyCallResult<bool>> callback)
        {
            if (String.IsNullOrEmpty (appleDeviceToken))
                throw new ArgumentException ("Can't be null or empty.", "appleDeviceToken");

            this.Client.Service.PushNotifications_Apple_RegisterDevice (this.Client.AppName, this.Client.AppPassword, AuthUser.Token, groupName,
                    appleDeviceToken, (bcr) =>
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
        /// Unregister the current user from push notifications for Apple devices.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true on success, false otherwise.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of UnregisterDeviceAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult UnregisterDeviceAsync (Action<bool, BuddyCallbackParams> callback, object state = null)
        {
            UnregisterDeviceInternal ((bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void UnregisterDeviceInternal (Action<BuddyCallResult<bool>> callback)
        {
            this.Client.Service.PushNotifications_Apple_RemoveDevice (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token, (bcr) =>
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
        /// Get a paged list of registered Apple devices for this Application. This list can then be used to iterate over the devices and send each user a push notification.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is a list of registered devices with user IDs. You can then user the IDs to send notifications to those users.</param>
        /// <param name="forGroup">Optionally filter only devices in a certain group.</param>
        /// <param name="pageSize">Set the number of devices that will be returned for each call of this method.</param>
        /// <param name="currentPage">Set the current page.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetRegisteredDevicesAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetRegisteredDevicesAsync (Action<List<RegisteredDeviceApple>, BuddyCallbackParams> callback, string forGroup = "", int pageSize = 10, int currentPage = 1, object state = null)
        {
            GetRegisteredDevicesInternal (forGroup, pageSize, currentPage, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void GetRegisteredDevicesInternal (string forGroup, int pageSize, int currentPage, Action<BuddyCallResult<List<RegisteredDeviceApple>>> callback)
        {
            if (pageSize <= 0)
                throw new ArgumentException ("Can't be smaller or equal to zero.", "pageSize");
            if (currentPage <= 0)
                throw new ArgumentException ("Can't be smaller or equal to zero.", "currentPage");

            this.Client.Service.PushNotifications_Apple_GetRegisteredDevices (this.Client.AppName, this.Client.AppPassword,
                    String.IsNullOrEmpty (forGroup) ? "" : forGroup, pageSize.ToString (), currentPage.ToString (), (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<List<RegisteredDeviceApple>> (null, bcr.Error));
                    return;
                }
                List<RegisteredDeviceApple> lst = new List<RegisteredDeviceApple> ();
                foreach (var d in result)
                    lst.Add (new RegisteredDeviceApple (d, this.AuthUser));
                {
                    callback (BuddyResultCreator.Create (lst, bcr.Error));
                    return; }
                ;
            });
            return;

        }


        /// <summary>
        /// Get a list of Apple groups that have been registered with Buddy as well as the number of users in each group. Groups can be used to batch-send
        /// push notifications to a number of users at the same time.
        /// <param name="callback">The async callback to call on success or error. The first parameter is a list of group names with counts per group.</param>
        /// </summary>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetGroupsAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetGroupsAsync (Action<Dictionary<string, int>, BuddyCallbackParams> callback, object state = null)
        {
            GetGroupsInternal ((bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void GetGroupsInternal (Action<BuddyCallResult<Dictionary<string, int>>> callback)
        {
            this.Client.Service.PushNotifications_Apple_GetGroupNames (this.Client.AppName, this.Client.AppPassword, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<Dictionary<string, int>> (null, bcr.Error));
                    return;
                }
                Dictionary<string, int> dict = new Dictionary<string, int> ();
                foreach (var d in result)
                    dict.Add (d.GroupName, Int32.Parse (d.DeviceCount));
                {
                    callback (BuddyResultCreator.Create (dict, bcr.Error));
                    return; }
                ;
            });
            return;

        }

      
        /// <summary>
        /// Send a raw message to a Apple device. Note that this call does not directly send the message but rather, adds the raw message to the queue of messages to be sent.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true on success, false otherwise.</param>
        /// <param name="message">The message to send to the user.</param>
        /// <param name="badge">The badge number to set on the icon. It is the application's responsibility to determine what number to set.</param>
        /// <param name="sound">The notification sound to play.</param>
        /// <param name="customItems">Metadata to send with the message for the receiving application to use. Data should be specified as key/value pairs where each key and value are seperated by a comma and each pair is seperated by a ";" character including the last pair ie: key,value;key,value;. Leave empty or set to null if there is no metadata to send.</param>
        /// <param name="senderUserId">The ID of the user that sent the notification.</param>
        /// <param name="deliverAfter">Schedule the message to be delivered after a certain date.</param>
        /// <param name="groupName">Send messages to an entire group of users, not just a one.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of SendRawMessageAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult SendRawMessageAsync (Action<bool, BuddyCallbackParams> callback, int senderUserId, string message, string badge, string sound, string customItems = "", DateTime deliverAfter = default(DateTime), string groupName = "", object state = null)
        {
            SendRawMessageInternal (senderUserId, message, badge, sound, customItems, deliverAfter, groupName, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void SendRawMessageInternal (int senderUserId, string message, string badge, string sound, string customItems, DateTime deliverAfter, string groupName, Action<BuddyCallResult<bool>> callback)
        {
            if (String.IsNullOrEmpty (message))
                throw new ArgumentException ("Can't be null or empty.", "message");

            if (String.IsNullOrEmpty (badge))
                badge = "";
            if (String.IsNullOrEmpty (sound))
                sound = "";
            if (String.IsNullOrEmpty (customItems))
                customItems = "";

            this.Client.Service.PushNotifications_Apple_SendRawMessage (this.Client.AppName, this.Client.AppPassword, senderUserId.ToString (),
                    deliverAfter == DateTime.MinValue ? "" : deliverAfter.ToString (CultureInfo.InvariantCulture), groupName, message, badge, sound, customItems, (bcr) =>
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
    }
}
