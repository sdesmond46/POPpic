using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using BuddyServiceClient;

namespace Buddy
{
    /// <summary>
    /// Represents an object that can be used to record device analytics, like device types and app crashes.
    /// <example>
    /// <code>
    ///     BuddyClient client = new BuddyClient("APPNAME", "APPPASS");
    ///     
    ///     // On WinPhone 7
    ///     client.RecordInformationAsync((r, state) => { }, System.Environment.OSVersion.Version.ToString(), DeviceExtendedProperties.GetValue("DeviceName").ToString())
    ///     
    ///     // On WinPhone 7 app.xaml.cs global exception handler
    ///     private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
    ///     {
    ///         BuddyClient client = new BuddyClient("APPNAME", "APPPASS");
    ///         client.Device.RecordCrashAsync((r, state) => { }, e.ExceptionObject.Message, e.ExceptionObject.StackTrace);
    ///         e.Handled = true;
    ///     }   
    /// </code>
    /// </example>
    /// </summary>
    public class Device : BuddyBase
    {
        protected override bool AuthUserRequired {
            get {
                return true;
            }
        }

        internal Device (BuddyClient client)
            : base(client)
        {

        }

       
        /// <summary>
        /// Record runtine device type information. This info will be uploaded to the Buddy service and can later be used for analytics purposes.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true on success, false otherwise.</param>
        /// <param name="osVersion">The OS version of the device runnign this code. On some .NET platforms you can use System.Environment.OSVersion to get this information.</param>
        /// <param name="deviceType">The type of device running this app. On Windows Phone 7 for example you can use DeviceExtendedProperties to retrieve this information.</param>
        /// <param name="user">The user that's registering this device information.</param>
        /// <param name="appVersion">The optional version of this application.</param>
        /// <param name="latitude">The optional latitude where this report was submitted.</param>
        /// <param name="longitude">The optional longiture where this report was submitted.</param>
        /// <param name="metadata">An optional application specific metadata string to include with the report.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of RecordInformationAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult RecordInformationAsync (Action<bool, BuddyCallbackParams> callback, string osVersion, string deviceType, AuthenticatedUser user, string appVersion = "1.0", double latitude = 0.0, double longitude = 0.0, string metadata = "", object state = null)
        {
            RecordInformationInternal (osVersion, deviceType, user, appVersion, latitude, longitude, metadata, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void RecordInformationInternal (string osVersion, string deviceType, AuthenticatedUser user, string appVersion, double latitude, double longitude, string metadata, Action<BuddyCallResult<bool>> callback)
        {
            if (osVersion == null)
                throw new ArgumentNullException ("osVersion", "Can't be null.");
            if (deviceType == null)
                throw new ArgumentNullException ("deviceType", "Can't be null.");
            if (user == null || user.Token == null)
                throw new ArgumentNullException ("user", "An AuthenticatedUser value is required.");
            if (latitude > 90.0 || latitude < -90.0)
                throw new ArgumentException ("Can't be bigger than 90.0 or smaller than -90.0.", "atLatitude");
            if (longitude > 180.0 || longitude < -180.0)
                throw new ArgumentException ("Can't be bigger than 180.0 or smaller than -180.0.", "atLongitude");


            this.Client.Service.Analytics_DeviceInformation_Add (this.Client.AppName, this.Client.AppPassword,
                    user.Token, osVersion, deviceType, latitude.ToString (CultureInfo.InvariantCulture), longitude.ToString (CultureInfo.InvariantCulture), this.Client.AppName, appVersion, metadata, (bcr) =>
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
        /// Record runtime crash information for this app. This could be exceptions, errors or your own custom crash information.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true on success, false otherwise.</param>
        /// <param name="methodName">The method name or location where the error happend. This could also be a descriptive string of the error.</param>
        /// <param name="osVersion">The OS version of the device runnign this code. On some .NET platforms you can use System.Environment.OSVersion to get this information.</param>
        /// <param name="deviceType">The type of device running this app. On Windows Phone 7 for example you can use DeviceExtendedProperties to retrieve this information.</param>
        /// <param name="user">The user that's registering this device information.</param>
        /// <param name="stackTrace">The optional stack trace of where the error happened.</param>
        /// <param name="appVersion">The optional version of this application.</param>
        /// <param name="latitude">The optional latitude where this report was submitted.</param>
        /// <param name="longitude">The optional longiture where this report was submitted.</param>
        /// <param name="metadata">An optional application specific metadata string to include with the report.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of RecordCrashAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult RecordCrashAsync (Action<bool, BuddyCallbackParams> callback, string methodName, string osVersion, string deviceType, AuthenticatedUser user, string stackTrace = "", string appVersion = "1.0", double latitude = 0.0, double longitude = 0.0, string metadata = "", object state = null)
        {
            RecordCrashInternal (methodName, osVersion, deviceType,  user, stackTrace, appVersion,latitude, longitude, metadata, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void RecordCrashInternal (string methodName, string osVersion, string deviceType,  AuthenticatedUser user, string stackTrace, string appVersion, double latitude, double longitude, string metadata, Action<BuddyCallResult<bool>> callback)
        {
            if (String.IsNullOrEmpty (methodName))
                throw new ArgumentException ("Can't be null or empty.", "methodName");

            if (osVersion == null)
                throw new ArgumentNullException ("osVersion", "Can't be null.");
            if (deviceType == null)
                throw new ArgumentNullException ("deviceType", "Can't be null.");
            if (user == null || user.Token == null)
                throw new ArgumentNullException ("user", "An AuthenticatedUser value is required.");
            if (latitude > 90.0 || latitude < -90.0)
                throw new ArgumentException ("Can't be bigger than 90.0 or smaller than -90.0.", "atLatitude");
            if (longitude > 180.0 || longitude < -180.0)
                throw new ArgumentException ("Can't be bigger than 180.0 or smaller than -180.0.", "atLongitude");


            this.Client.Service.Analytics_CrashRecords_Add (this.Client.AppName, this.Client.AppPassword,
                   user.Token, appVersion, osVersion, deviceType, methodName, stackTrace, metadata, latitude.ToString (CultureInfo.InvariantCulture), longitude.ToString (CultureInfo.InvariantCulture), (bcr) =>
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

#if AWAIT_SUPPORTED

        /// <summary>
        /// Record runtine device type information. This info will be uploaded to the Buddy service and can later be used for analytics purposes.
        /// </summary>
        /// <param name="osVersion">The OS version of the device runnign this code. On some .NET platforms you can use System.Environment.OSVersion to get this information.</param>
        /// <param name="deviceType">The type of device running this app. On Windows Phone 7 for example you can use DeviceExtendedProperties to retrieve this information.</param>
        /// <param name="user">The user that's registering this device information.</param>
        /// <param name="appVersion">The optional version of this application.</param>
        /// <param name="latitude">The optional latitude where this report was submitted.</param>
        /// <param name="longitude">The optional longiture where this report was submitted.</param>
        /// <param name="metadata">An optional application specific metadata string to include with the report.</param>
        /// <returns>A Task&lt;Boolean&gt;that can be used to monitor progress on this call.</returns>
        public  System.Threading.Tasks.Task<Boolean> RecordInformationAsync(string osVersion, string deviceType, Buddy.AuthenticatedUser user, string appVersion = "1.0", double latitude = 0, double longitude = 0, string metadata = "")
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<Boolean>();
            RecordInformationInternal(osVersion, deviceType, user, appVersion, latitude, longitude, metadata, (bcr) =>
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
        /// Record runtime crash information for this app. This could be exceptions, errors or your own custom crash information.
        /// </summary>
        /// <param name="methodName">The method name or location where the error happend. This could also be a descriptive string of the error.</param>
        /// <param name="osVersion">The OS version of the device runnign this code. On some .NET platforms you can use System.Environment.OSVersion to get this information.</param>
        /// <param name="deviceType">The type of device running this app. On Windows Phone 7 for example you can use DeviceExtendedProperties to retrieve this information.</param>
        /// <param name="user">The user that's registering this device information.</param>
        /// <param name="stackTrace">The optional stack trace of where the error happened.</param>
        /// <param name="appVersion">The optional version of this application.</param>
        /// <param name="latitude">The optional latitude where this report was submitted.</param>
        /// <param name="longitude">The optional longiture where this report was submitted.</param>
        /// <param name="metadata">An optional application specific metadata string to include with the report.</param>
        /// <returns>A Task&lt;Boolean&gt;that can be used to monitor progress on this call.</returns>
        public System.Threading.Tasks.Task<Boolean> RecordCrashAsync( string methodName, string osVersion, string deviceType, Buddy.AuthenticatedUser user, string stackTrace = "", string appVersion = "1.0", double latitude = 0, double longitude = 0, string metadata = "")
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<Boolean>();
            RecordCrashInternal(methodName, osVersion, deviceType, user, stackTrace, appVersion, latitude, longitude, metadata, (bcr) =>
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
