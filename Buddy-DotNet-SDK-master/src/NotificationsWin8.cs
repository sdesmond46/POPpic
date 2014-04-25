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
    /// Represents an object that can be used to register Win8 devices for push notifications. The class can also be used to query for all registered devices and
    /// to send them notifications.
    /// </summary>
    public class NotificationsWin8 : BuddyBase
    {


        protected override bool AuthUserRequired {
            get {
                return true;
            }
        }

        internal NotificationsWin8 (BuddyClient client, AuthenticatedUser user)
            : base(client, user)
        {

        }

      
        /// <summary>
        /// Register an Win8 device for notificatons with Buddy. 
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true on success, false otherwise.</param>
        /// <param name="deviceUri">The URI for the device as returned by the Windows push phone HttpNotificationChannel object.</param>
        /// <param name="clientId">The Package Security Identifier (SID) acquired when the app was registered with the Windows Store Dashboard. </param>
        /// <param name="clientSecret">The secret key corresponding to the SID acquired when the app was registered with the Windows Store Dashboard.</param>
        /// <param name="groupName">Register this device as part of a group, so that you can send the whole group messages.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of RegisterDeviceAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult RegisterDeviceAsync (Action<bool, BuddyCallbackParams> callback, string deviceUri, string clientId, string clientSecret, string groupName = "", object state = null)
        {
            RegisterDeviceInternal (deviceUri, clientId, clientSecret, groupName, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void RegisterDeviceInternal (string deviceUri, string clientId, string clientSecret, string groupName, Action<BuddyCallResult<bool>> callback)
        {
            if (String.IsNullOrEmpty (deviceUri))
                throw new ArgumentException ("Can't be null or empty.", "deviceUri");
            if (String.IsNullOrEmpty (clientId))
                throw new ArgumentException ("Can't be null or empty.", "clientId");
            if (String.IsNullOrEmpty (clientSecret))
                throw new ArgumentException ("Can't be null or empty.", "clientSecret");

            // deviceUri is already partially encoded; encode again so the original encoding isn't lost during decoding on the server.
            deviceUri = Uri.EscapeDataString(deviceUri);

            this.Client.Service.PushNotifications_Win8_RegisterDevice (this.Client.AppName, this.Client.AppPassword, AuthUser.Token, deviceUri, clientId, clientSecret, groupName, (bcr) =>
            {

                callback (BuddyResultCreator.Create (bcr.Result == "1", bcr.Error));

            });
            return;
        }

       

        /// <summary>
        /// Unregister the current user from push notifications for Win8 devices.
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
            this.Client.Service.PushNotifications_Win8_RemoveDevice (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token, (bcr) =>
            {
                callback (BuddyResultCreator.Create (bcr.Result == "1", bcr.Error));
            });


            return;
        }


        /// <summary>
        /// Get a paged list of registered Win8 devices for this Application. This list can then be used to iterate over the devices and send each user a push notification.
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
        public IAsyncResult GetRegisteredDevicesAsync (Action<List<RegisteredDeviceWin8>, BuddyCallbackParams> callback, string forGroup = "", int pageSize = 10, int currentPage = 1, object state = null)
        {
            GetRegisteredDevicesInternal (forGroup, pageSize, currentPage, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void GetRegisteredDevicesInternal (string forGroup, int pageSize, int currentPage, Action<BuddyCallResult<List<RegisteredDeviceWin8>>> callback)
        {
            if (pageSize <= 0)
                throw new ArgumentException ("Can't be smaller or equal to zero.", "pageSize");
            if (currentPage <= 0)
                throw new ArgumentException ("Can't be smaller or equal to zero.", "currentPage");

            this.Client.Service.PushNotifications_Win8_GetRegisteredDevices (this.Client.AppName, this.Client.AppPassword,
                    String.IsNullOrEmpty (forGroup) ? "" : forGroup, pageSize.ToString (), currentPage.ToString (), (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<List<RegisteredDeviceWin8>> (null, bcr.Error));
                    return;
                }
                List<RegisteredDeviceWin8> lst = new List<RegisteredDeviceWin8> ();
                foreach (var d in result)
                    lst.Add (new RegisteredDeviceWin8 (d, this.AuthUser));
                {
                    callback (BuddyResultCreator.Create (lst, bcr.Error));
                    return; }
                ;
            });
            return;
        }

     

        /// <summary>
        /// Get a list of Win8 groups that have been registered with Buddy as well as the number of users in each group. Groups can be used to batch-send
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
            this.Client.Service.PushNotifications_Win8_GetGroupNames (this.Client.AppName, this.Client.AppPassword, (bcr) =>
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
        /// Send a image tile to a Win8 device. The tile is represented by a image URL, you can take a look at the Windows phone docs for image dimensions and formats.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true on success, false otherwise.</param>
        /// <param name="xmlPayload">The xml schema describing the tile. Can be specified in the URL using proper character escaping or via the message body. For more information <see href="http://msdn.microsoft.com/en-us/library/windows/apps/hh761491.aspx"/>.</param>
        /// <param name="senderUserId">The ID of the user that sent the notification.</param>
        /// <param name="deliverAfter">Schedule the message to be delivered after a certain date.</param>
        /// <param name="groupName">Send messages to an entire group of users, not just a one.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of SendTileAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult SendTileAsync (Action<bool, BuddyCallbackParams> callback, string xmlPayload, int senderUserId, DateTime deliverAfter = default(DateTime), string groupName = "", object state = null)
        {
            SendTileInternal (xmlPayload, senderUserId, deliverAfter, groupName, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void SendTileInternal (string xmlPayload, int senderUserId, DateTime deliverAfter, string groupName, Action<BuddyCallResult<bool>> callback)
        {
            if (String.IsNullOrEmpty (xmlPayload))
                throw new ArgumentException ("Can't be null or empty.", "xmlPayload");

            this.Client.Service.PushNotifications_Win8_SendLiveTile (this.Client.AppName, this.Client.AppPassword, senderUserId.ToString (), xmlPayload,
                    deliverAfter == DateTime.MinValue ? "" : deliverAfter.ToString (CultureInfo.InvariantCulture), groupName, (bcr) =>
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
        /// Send a badge to a windows 8 device. The app needs to be active and the Raw message callback set in order to recieve this message.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true on success, false otherwise.</param>
        /// <param name="xmlPayload">The message to send.</param>
        /// <param name="senderUserId">The ID of the user that sent the notification.</param>
        /// <param name="deliverAfter">Schedule the message to be delivered after a certain date.</param>
        /// <param name="groupName">Send messages to an entire group of users, not just a one.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of SendBadgeAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult SendBadgeAsync (Action<bool, BuddyCallbackParams> callback, string xmlPayload, int senderUserId, DateTime deliverAfter = default(DateTime), string groupName = "", object state = null)
        {
            SendBadgeInternal (xmlPayload, senderUserId, deliverAfter, groupName, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void SendBadgeInternal (string xmlPayload, int senderUserId, DateTime deliverAfter, string groupName, Action<BuddyCallResult<bool>> callback)
        {
            if (String.IsNullOrEmpty (xmlPayload))
                throw new ArgumentException ("Can't be null or empty.", "xmlPayload");

            this.Client.Service.PushNotifications_Win8_SendBadge (this.Client.AppName, this.Client.AppPassword, senderUserId.ToString (), xmlPayload,
                    deliverAfter == DateTime.MinValue ? "" : deliverAfter.ToString (CultureInfo.InvariantCulture), groupName, (bcr) =>
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
        /// Send toast message to a windows 8 device. If the app is active the user will recieve this message in the toast message callback. Otherwise the message
        /// appears as a notification on top of the screen. Clicking it will launch the app.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true on success, false otherwise.</param>
        /// <param name="xmlPayload">The xml schema describing the tile. Can be specified in the URL using proper character escaping or via the message body. For more information <see href="http://msdn.microsoft.com/en-us/library/windows/apps/hh761491.aspx"/>.</param>
        /// <param name="senderUserId">The ID of the user that sent the notification.</param>
        /// <param name="deliverAfter">Schedule the message to be delivered after a certain date.</param>
        /// <param name="groupName">Send messages to an entire group of users, not just a one.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of SendToastMessageAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult SendToastMessageAsync (Action<bool, BuddyCallbackParams> callback, string xmlPayload, int senderUserId, DateTime deliverAfter = default(DateTime), string groupName = "", object state = null)
        {
            SendToastMessageInternal (xmlPayload, senderUserId, deliverAfter, groupName, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void SendToastMessageInternal (string xmlPayload, int senderUserId, DateTime deliverAfter, string groupName, Action<BuddyCallResult<bool>> callback)
        {
            if (xmlPayload == null)
                throw new ArgumentNullException ("xmlPayload");

            this.Client.Service.PushNotifications_Win8_SendToast (this.Client.AppName, this.Client.AppPassword, senderUserId.ToString (), xmlPayload,
                    deliverAfter == DateTime.MinValue ? "" : deliverAfter.ToString (CultureInfo.InvariantCulture), groupName, (bcr) =>
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
