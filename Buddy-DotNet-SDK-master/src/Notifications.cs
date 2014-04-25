using System;
using System.Net;
using BuddyServiceClient;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

#if WINDOWS_PHONE
using Microsoft.Phone.Notification;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
#endif

namespace Buddy
{
    /// <summary>
    /// Represents an object that you can use to send or recieve push notifications. Note that you can only recieve phone notifications on Windows Phone or Windows 8 Metro Apps.
    /// <example>
    /// <code>
    ///     BuddyClient client = new BuddyClient("APPNAME", "APPPASS");
    ///     
    ///     // On WinPhone 7 - configure push for tiles and toast
    ///     client.LoginAsync((user, state) => {
    ///         user.PushNotifications.ConfigurePushAsync((r, state2) => {
    ///         
    ///             // Send myself a toast
    ///             user.PushNotifications.SendToastMessageAsync(null, "test title", "test", "test", user.ID);
    ///         }, "My Channel", true, true, toastMessageCallback: (msg) => {
    ///         
    ///             Dispatcher.BeginInvoke(() => { MessageBox.Show("TOAST!!!"); });
    ///         });
    ///     }, "username", "password");
    /// </code>
    /// </example>
    /// </summary>
    public class Notifications : BuddyBase
    {
        protected override bool AuthUserRequired {
            get {
                return true;
            }
        }

        public NotificationsAndroid Android { get; set; }

        public NotificationsApple Apple { get; set; }

        public NotificationsWin8 Win8 { get; set; }

        internal Notifications (BuddyClient client, AuthenticatedUser user)
            : base(client, user)
        {


            Android = new NotificationsAndroid (client, user);
            Apple = new NotificationsApple (client, user);
            Win8 = new NotificationsWin8 (client, user);
        }


        /// <summary>
        /// Register a Windows device for notificatons with Buddy. The URL is the notifications channel link that provided by the platform. Most of the time
        /// you don't need to call this API directly, you can use ConfigurePushAsync instead which will configure everyting for you. Note that if you call this method,
        /// you are responsible to configure the device for push notifications.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true on success, false otherwise.</param>
        /// <param name="deviceUri">The device notification channel URI.</param>
        /// <param name="enableTile">Optionally enable tile notifications</param>
        /// <param name="enableRaw">Optionally enable raw notifications.</param>
        /// <param name="enableToast">Optionally enable toast notifications.</param>
        /// <param name="groupName">Register this device as part of a group, so that you can send the whole group messages.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of RegisterDeviceAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult RegisterDeviceAsync (Action<bool, BuddyCallbackParams> callback, string deviceUri, bool enableTile = true,
                            bool enableRaw = true, bool enableToast = true, string groupName = "", object state = null)
        {
            RegisterDeviceInternal (deviceUri, enableTile, enableRaw, enableToast, groupName, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void RegisterDeviceInternal (string deviceUri, bool enableTile, bool enableRaw, bool enableToast, string groupName, Action<BuddyCallResult<bool>> callback)
        {
            if (String.IsNullOrEmpty (deviceUri))
                throw new ArgumentException ("Can't be null or empty.", "deviceUri");

            this.Client.Service.PushNotifications_WP_RegisterDevice (this.Client.AppName, this.Client.AppPassword, AuthUser.Token, deviceUri,
                    groupName, enableTile, enableRaw, enableToast, (bcr) =>
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
        /// Unregister the current user from push notifications.
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
            this.Client.Service.PushNotifications_WP_RemoveDevice (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token, (bcr) =>
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
        /// Get a paged list of registered devices for this Application. This list can then be used to iterate over the devices and send each user a push notification.
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
        public IAsyncResult GetRegisteredDevicesAsync (Action<List<RegisteredDevice>, BuddyCallbackParams> callback, string forGroup = "", int pageSize = 10, int currentPage = 1, object state = null)
        {
            GetRegisteredDevicesInternal (forGroup, pageSize, currentPage, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void GetRegisteredDevicesInternal (string forGroup, int pageSize, int currentPage, Action<BuddyCallResult<List<RegisteredDevice>>> callback)
        {
            if (pageSize <= 0)
                throw new ArgumentException ("Can't be smaller or equal to zero.", "pageSize");
            if (currentPage <= 0)
                throw new ArgumentException ("Can't be smaller or equal to zero.", "currentPage");

            this.Client.Service.PushNotifications_WP_GetRegisteredDevices (this.Client.AppName, this.Client.AppPassword,
                    String.IsNullOrEmpty (forGroup) ? "" : forGroup, pageSize.ToString (), currentPage.ToString (), (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<List<RegisteredDevice>> (null, bcr.Error));
                    return;
                }
                List<RegisteredDevice> lst = new List<RegisteredDevice> ();
                foreach (var d in result)
                    lst.Add (new RegisteredDevice (d, this.AuthUser));
                {
                    callback (BuddyResultCreator.Create (lst, bcr.Error));
                    return; }
                ;
            });
            return;
        }

        /// <summary>
        /// Get a list of groups that have been registered with Buddy as well as the number of users in each group. Groups can be used to batch-send
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
            this.Client.Service.PushNotifications_WP_GetGroupNames (this.Client.AppName, this.Client.AppPassword, (bcr) =>
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
        /// Send a image tile to a windows phone device. The tile is represented by a image URL, you can take a look at the Windows phone docs for image dimensions and formats.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true on success, false otherwise.</param>
        /// <param name="imageUri">The URL of the tile image.</param>
        /// <param name="senderUserId">The ID of the user that sent the notification.</param>
        /// <param name="messageCount">The message count for this tile.</param>
        /// <param name="messageTitle">The message title for the tile.</param>
        /// <param name="deliverAfter">Schedule the message to be delivered after a certain date.</param>
        /// <param name="groupName">Send messages to an entire group of users, not just a one.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of SendTileAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult SendTileAsync (Action<bool, BuddyCallbackParams> callback, string imageUri, int senderUserId, int messageCount = -1, string messageTitle = "", DateTime deliverAfter = default(DateTime), string groupName = "", object state = null)
        {
            SendTileInternal (imageUri, senderUserId, messageCount, messageTitle, deliverAfter, groupName, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void SendTileInternal (string imageUri, int senderUserId, int messageCount, string messageTitle, DateTime deliverAfter, string groupName, Action<BuddyCallResult<bool>> callback)
        {
            if (String.IsNullOrEmpty (imageUri))
                throw new ArgumentException ("Can't be null or empty.", "imageUri");

            this.Client.Service.PushNotifications_WP_SendLiveTile (this.Client.AppName, this.Client.AppPassword, senderUserId.ToString (), imageUri, messageCount.ToString (),
                    messageTitle.ToString (), deliverAfter == DateTime.MinValue ? "" : deliverAfter.ToString (CultureInfo.InvariantCulture), groupName, (bcr) =>
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
        /// Send a raw message to a windows phone device. The app needs to be active and the Raw message callback set in order to recieve this message.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true on success, false otherwise.</param>
        /// <param name="rawMessage">The message to send.</param>
        /// <param name="senderUserId">The ID of the user that sent the notification.</param>
        /// <param name="deliverAfter">Schedule the message to be delivered after a certain date.</param>
        /// <param name="groupName">Send messages to an entire group of users, not just a one.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of SendRawMessageAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult SendRawMessageAsync (Action<bool, BuddyCallbackParams> callback, string rawMessage, int senderUserId, DateTime deliverAfter = default(DateTime), string groupName = "", object state = null)
        {
            SendRawMessageInternal (rawMessage, senderUserId, deliverAfter, groupName, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void SendRawMessageInternal (string rawMessage, int senderUserId, DateTime deliverAfter, string groupName, Action<BuddyCallResult<bool>> callback)
        {
            if (String.IsNullOrEmpty (rawMessage))
                throw new ArgumentException ("Can't be null or empty.", "rawMessage");

            this.Client.Service.PushNotifications_WP_SendRawMessage (this.Client.AppName, this.Client.AppPassword, senderUserId.ToString (), rawMessage,
                    deliverAfter == DateTime.MinValue ? "" : deliverAfter.ToString (CultureInfo.InvariantCulture), groupName, (bcr) =>
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
        /// Send toast message to a windows phone device. If the app is active the user will recieve this message in the toast message callback. Otherwise the message
        /// appears as a notification on top of the screen. Clicking it will launch the app.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true on success, false otherwise.</param>
        /// <param name="toastTitle">The title of the toast message/</param>
        /// <param name="toastSubtitle">The subtitle of the toast message.</param>
        /// <param name="toastParameter">An optional parameter for the toast message.</param>
        /// <param name="senderUserId">The ID of the user that sent the notification.</param>
        /// <param name="deliverAfter">Schedule the message to be delivered after a certain date.</param>
        /// <param name="groupName">Send messages to an entire group of users, not just a one.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of SendToastMessageAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult SendToastMessageAsync (Action<bool, BuddyCallbackParams> callback, string toastTitle, string toastSubtitle, int senderUserId, string toastParameter = "", DateTime deliverAfter = default(DateTime), string groupName = "", object state = null)
        {
            SendToastMessageInternal (toastTitle, toastSubtitle,  senderUserId, toastParameter,deliverAfter, groupName, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void SendToastMessageInternal (string toastTitle, string toastSubtitle, int senderUserId, string toastParameter, DateTime deliverAfter, string groupName, Action<BuddyCallResult<bool>> callback)
        {
            if (toastTitle == null)
                throw new ArgumentNullException ("toastTitle");
            if (toastSubtitle == null)
                throw new ArgumentNullException ("toastSubtitle");
            if (toastParameter == null)
                throw new ArgumentNullException ("toastParameter");

            this.Client.Service.PushNotifications_WP_SendToastMessage (this.Client.AppName, this.Client.AppPassword, senderUserId.ToString (), toastTitle, toastSubtitle, toastParameter,
                    deliverAfter == DateTime.MinValue ? "" : deliverAfter.ToString (CultureInfo.InvariantCulture), groupName, (bcr) =>
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
        /// Register a Windows device for notificatons with Buddy. The URL is the notifications channel link that provided by the platform. Most of the time
        /// you don't need to call this API directly, you can use ConfigurePushAsync instead which will configure everyting for you. Note that if you call this method,
        /// you are responsible to configure the device for push notifications.
        /// </summary>
        /// <param name="deviceUri">The device notification channel URI.</param>
        /// <param name="enableTile">Optionally enable tile notifications</param>
        /// <param name="enableRaw">Optionally enable raw notifications.</param>
        /// <param name="enableToast">Optionally enable toast notifications.</param>
        /// <param name="groupName">Register this device as part of a group, so that you can send the whole group messages.</param>
        /// <returns>A Task&lt;Boolean&gt;that can be used to monitor progress on this call.</returns>
        public System.Threading.Tasks.Task<Boolean> RegisterDeviceAsync(string deviceUri, bool enableTile = true, bool enableRaw = true, bool enableToast = true, string groupName = "")
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<Boolean>();
            RegisterDeviceInternal(deviceUri, enableTile, enableRaw, enableToast, groupName, (bcr) =>
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
        /// Unregister the current user from push notifications.
        /// </summary>
        /// <returns>A Task&lt;Boolean&gt;that can be used to monitor progress on this call.</returns>
        public System.Threading.Tasks.Task<Boolean> UnregisterDeviceAsync()
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<Boolean>();
            UnregisterDeviceInternal((bcr) =>
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
        /// Get a paged list of registered devices for this Application. This list can then be used to iterate over the devices and send each user a push notification.
        /// </summary>
        /// <param name="forGroup">Optionally filter only devices in a certain group.</param>
        /// <param name="pageSize">Set the number of devices that will be returned for each call of this method.</param>
        /// <param name="currentPage">Set the current page.</param>
        /// <returns>A Task&lt;IEnumerable&lt;RegisteredDevice&gt; &gt;that can be used to monitor progress on this call.</returns>
        public System.Threading.Tasks.Task<IEnumerable<RegisteredDevice>> GetRegisteredDevicesAsync(string forGroup = "", int pageSize = 10, int currentPage = 1)
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<IEnumerable<RegisteredDevice>>();
            GetRegisteredDevicesInternal(forGroup, pageSize, currentPage, (bcr) =>
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
        /// Get a list of groups that have been registered with Buddy as well as the number of users in each group. Groups can be used to batch-send
        /// push notifications to a number of users at the same time.
        /// </summary>
        /// <returns>A Task&lt;IDictionary&lt;String,Int32&gt; &gt;that can be used to monitor progress on this call.</returns>
        public System.Threading.Tasks.Task<IDictionary<String, Int32>> GetGroupsAsync()
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<IDictionary<String, Int32>>();
            GetGroupsInternal((bcr) =>
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
        /// Send a image tile to a windows phone device. The tile is represented by a image URL, you can take a look at the Windows phone docs for image dimensions and formats.
        /// </summary>
        /// <param name="imageUri">The URL of the tile image.</param>
        /// <param name="senderUserId">The ID of the user that sent the notification.</param>
        /// <param name="messageCount">The message count for this tile.</param>
        /// <param name="messageTitle">The message title for the tile.</param>
        /// <param name="deliverAfter">Schedule the message to be delivered after a certain date.</param>
        /// <param name="groupName">Send messages to an entire group of users, not just a one.</param>
        /// <returns>A Task&lt;Boolean&gt;that can be used to monitor progress on this call.</returns>
        public  System.Threading.Tasks.Task<Boolean> SendTileAsync(string imageUri, int senderUserId, int messageCount = -1, string messageTitle = "", System.DateTime deliverAfter = default(DateTime), string groupName = "")
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<Boolean>();
            SendTileInternal(imageUri, senderUserId, messageCount, messageTitle, deliverAfter, groupName, (bcr) =>
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
        /// Send a raw message to a windows phone device. The app needs to be active and the Raw message callback set in order to recieve this message.
        /// </summary>
        /// <param name="rawMessage">The message to send.</param>
        /// <param name="senderUserId">The ID of the user that sent the notification.</param>
        /// <param name="deliverAfter">Schedule the message to be delivered after a certain date.</param>
        /// <param name="groupName">Send messages to an entire group of users, not just a one.</param>
        /// <returns>A Task&lt;Boolean&gt;that can be used to monitor progress on this call.</returns>
        public System.Threading.Tasks.Task<Boolean> SendRawMessageAsync(string rawMessage, int senderUserId, System.DateTime deliverAfter = default(DateTime), string groupName = "")
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<Boolean>();
            SendRawMessageInternal(rawMessage, senderUserId, deliverAfter, groupName, (bcr) =>
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
        /// Send toast message to a windows phone device. If the app is active the user will recieve this message in the toast message callback. Otherwise the message
        /// appears as a notification on top of the screen. Clicking it will launch the app.
        /// </summary>
        /// <param name="toastTitle">The title of the toast message/</param>
        /// <param name="toastSubtitle">The subtitle of the toast message.</param>
        /// <param name="toastParameter">An optional parameter for the toast message.</param>
        /// <param name="senderUserId">The ID of the user that sent the notification.</param>
        /// <param name="deliverAfter">Schedule the message to be delivered after a certain date.</param>
        /// <param name="groupName">Send messages to an entire group of users, not just a one.</param>
        /// <returns>A Task&lt;Boolean&gt;that can be used to monitor progress on this call.</returns>
        public System.Threading.Tasks.Task<Boolean> SendToastMessageAsync(string toastTitle, string toastSubtitle, int senderUserId, string toastParameter = "", System.DateTime deliverAfter = default(DateTime), string groupName = "")
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<Boolean>();
            SendToastMessageInternal(toastTitle, toastSubtitle, senderUserId, toastParameter, deliverAfter, groupName, (bcr) =>
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

#if WINDOWS_PHONE
        /// <summary>
        /// Configure this Windows Phone device for push notifications
        /// </summary>
        /// <param name="channelName"></param>
        /// <param name="enableTiles"></param>
        /// <param name="enableToastMessages"></param>
        /// <param name="groupName"></param>
        /// <param name="allowedDomains"></param>
        /// <param name="rawMessageCallback"></param>
        /// <param name="toastMessageCallback"></param>
        public System.Threading.Tasks.Task<Boolean> ConfigurePushAsync(bool enableTiles, bool enableToastMessages, string groupName = "",
            List<string> allowedDomains = null, Action<string> rawMessageCallback = null, Action<IDictionary<string, string>> toastMessageCallback = null, string channelName = null)
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<Boolean>();

            Action<bool, Exception> finish = (result, ex) =>
            {
                if (ex != null)
                {
                    tcs.TrySetException(ex);
                }
                else
                {
                    tcs.TrySetResult(result);
                }
            };
           
            if (allowedDomains == null) allowedDomains = new List<string>();
            allowedDomains.Add("http://buddyplatform.s3.amazonaws.com/");
            
            channelName = this.GetChannelName(channelName);

            HttpNotificationChannel channel = null;
            bool done = false;
            if ((channel = HttpNotificationChannel.Find(channelName)) == null)
            {
                channel = new HttpNotificationChannel(channelName, "www.buddy.com");
                if (channel == null)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        finish(false, new Exception("Couldn't create HttpNotificationChannel."));
                    });

                    done = true;
                }
                else
                {
                    channel.Open();
                }
            }

            if (!done && rawMessageCallback != null) channel.HttpNotificationReceived += (s, ev) =>
            {
                StreamReader reader = new StreamReader(ev.Notification.Body);

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    rawMessageCallback(reader.ReadToEnd());
                });
            };


            if (!done && toastMessageCallback != null) channel.ShellToastNotificationReceived += (s, ev) =>
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    toastMessageCallback(ev.Collection);
                });
            };

            Action<HttpNotificationChannel> registerUri = (newChannel) =>
            {
                if (enableTiles)
                {
                    BindTile(newChannel, allowedDomains);
                }

                if (enableToastMessages)
                {
                    BindToast(newChannel);
                }

                this.RegisterDeviceInternal(newChannel.ChannelUri.ToString(), enableTiles, rawMessageCallback != null, enableToastMessages, groupName, (bcr) =>
                {
                   
                    finish(bcr.Result, bcr.Error == BuddyError.None ? null : new BuddyServiceException(bcr.Error.ToString()));
                   
                });

              
            };

            if (!done)
            {
                channel.ChannelUriUpdated += (s, ev) =>
                {
                    registerUri(channel);
                };

                if (channel.ChannelUri != null)
                {
                    registerUri(channel);
                }
            }
            return tcs.Task;
        }

        private string GetChannelName(string channelName)
        {
            if (String.IsNullOrEmpty(channelName))
            {
                var settings = IsolatedStorageSettings.ApplicationSettings;

                if (!settings.Contains(BuddyChannelKey))
                {
                    var guid = Guid.NewGuid();
                    settings[BuddyChannelKey] = BuddyChannelKey + guid.ToString();
                    settings.Save();
                }
                channelName = (string) settings[BuddyChannelKey];

            }
            return channelName;
        }

        private static string BuddyChannelKey = "__BuddyChannel";

        private void BindToast(HttpNotificationChannel channel)
        {
            if (!channel.IsShellToastBound) channel.BindToShellToast();
        }

        private void BindTile(HttpNotificationChannel channel, List<string> allowedDomains)
        {
            // Check if the tile is bound, if not, bind it with the domain our image is coming from.
            if (!channel.IsShellTileBound)
            {
                Collection<Uri> ListOfAllowedDomains = new Collection<Uri>();
                foreach (string url in allowedDomains) ListOfAllowedDomains.Add(new Uri(url, UriKind.RelativeOrAbsolute));
                channel.BindToShellTile(ListOfAllowedDomains);
            }
        }
#endif
    }
}
