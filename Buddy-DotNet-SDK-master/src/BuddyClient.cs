using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
using BuddyServiceClient;
using System.Reflection;

#if WINDOWS_PHONE
using System.Net;
#else


#endif
using System.Xml.Linq;
using System.Threading.Tasks;

namespace Buddy
{
    /// <summary>
    /// Represents the main class and entry point to the Buddy platform. Use this class to interact with the platform, create and login users and modify general
    /// application level properties like Devices and Metadata.
    /// <example>
    /// <code>
    ///     BuddyClient client = new BuddyClient("APPNAME", "APPPASS");
    ///     client.PingAsync(null);
    /// </code>
    /// </example>
    /// </summary>
    public class BuddyClient
    {


        /// <summary>
        /// Gets the BuddyServiceClient interface
        /// </summary>
        internal BuddyServiceClientBase Service { get; private set; }

        /// <summary>
        /// Gets the application name for this client.
        /// </summary>
        public string AppName { get; protected set; }

        /// <summary>
        /// Gets the application password for this client.
        /// </summary>
        public string AppPassword { get; protected set; }

        /// <summary>
        /// Gets the optional string that describes the version of the app you are building. This string is used when uploading
        /// device information to buddy or submitting crash reports. It will default to 1.0.
        /// </summary>
        public string AppVersion { get; protected set; }

        /// <summary>
        /// Gets an object that can be used to manipulate application-level metadata. Metadata is used to store custom values on the platform.
        /// </summary>
        public AppMetadata Metadata { get; protected set; }

        /// <summary>
        /// Gets an object that can be used to record device information about this client or upload crashes.
        /// </summary>
        public Device Device { get; protected set; }

        /// <summary>
        /// Gets an object that can be used to retrieve high score rankings or search for game boards in this application.
        /// </summary>
        public GameBoards GameBoards { get; protected set; }

        /// <summary>
        /// Gets an object that can be used to retrieve sounds.
        /// </summary>
        public Sounds Sounds { get; protected set; }

        private bool recordDeviceInfo = true;
        private const string WebServiceUrl = "https://webservice.buddyplatform.com";
        /// <summary>
        /// Initializes a new instance of the BuddyClient class. To get an application username and password, go to http://buddy.com, create a new
        /// developer account and create a new application.
        /// </summary>
        /// <param name="appName">The name of the application to use with this client. Can't be null or empty.</param>
        /// <param name="appPassword">The password of the application to use with this client. Can't be null or empty.</param>
        /// <param name="appVersion">Optional string that describes the version of the app you are building. This string will then be used when uploading
        /// device information to buddy or submitting crash reports.</param>
        /// <param name="autoRecordDeviceInfo">If true automatically records the current device profile with the Buddy Service (device type, os version, etc.). Note that this
        /// only works for Windows Phone clients.</param>
        public BuddyClient(string appName, string appPassword, string appVersion = "1.0", bool autoRecordDeviceInfo = true)
        {

#if WINDOWS_PHONE
             var  versionAttrs = Assembly.GetExecutingAssembly().GetCustomAttributes(false);
#else
            var versionAttrs = typeof(BuddyClient).GetTypeInfo().Assembly.GetCustomAttributes();
#endif
            var attr = versionAttrs.OfType<AssemblyFileVersionAttribute>().First();

            var sdkVersion = "Version=" + attr.Version;

            this.Service = new BuddyServiceClientHttp(WebServiceUrl, sdkVersion);

            if (String.IsNullOrEmpty(appName))
                throw new ArgumentException("Can't be null or empty.", "appName");
            if (String.IsNullOrEmpty(appPassword))
                throw new ArgumentException("Can't be null or empty.", "appPassword");

            this.AppName = appName;
            this.AppPassword = appPassword;
            this.AppVersion = String.IsNullOrEmpty(appVersion) ? "1.0" : appVersion;
            this.Metadata = new AppMetadata(this);
            this.Device = new Device(this);
            this.GameBoards = new GameBoards(this);
            this.Sounds = new Sounds(this);

            this.recordDeviceInfo = autoRecordDeviceInfo;
        }

      

        /// <summary>
        /// Ping the service.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is a string "Pong" if this method was successful.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of PingAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult PingAsync(Action<string, BuddyCallbackParams> callback, object state = null)
        {
            PingInternal((bcr) =>
            {
                if (callback == null)
                    return;
                callback(bcr.Result, new BuddyCallbackParams(bcr.Error));
            });
            return null;
        }

        internal void PingInternal(Action<BuddyCallResult<string>> callback)
        {
            this.Service.Service_Ping_Get(this.AppName, this.AppPassword, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None)
                {
                    callback(BuddyResultCreator.Create<string>(null, bcr.Error));
                    return;
                }
                {
                    callback(BuddyResultCreator.Create(result, bcr.Error));
                    return;
                }
                ;
            });
            return;

        }


        /// <summary>
        /// Get the current Buddy web-service date/time.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is the datetime of the Buddy web-service.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetServiceTimeAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetServiceTimeAsync(Action<DateTime, BuddyCallbackParams> callback, object state = null)
        {
            GetServiceTimeInternal((bcr) =>
            {
                if (callback == null)
                    return;
                callback(bcr.Result, new BuddyCallbackParams(bcr.Error));
            });
            return null;
        }

        internal void GetServiceTimeInternal(Action<BuddyCallResult<DateTime>> callback)
        {
            this.Service.Service_DateTime_Get(this.AppName, this.AppPassword, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None)
                {
                    callback(BuddyResultCreator.Create(default(DateTime), bcr.Error));
                    return;
                }
                {
                    callback(BuddyResultCreator.Create(Convert.ToDateTime(result, CultureInfo.InvariantCulture), bcr.Error));
                    return;
                }
                ;
            });
            return;

        }


        /// <summary>
        /// Get the current version of the service that is being used by this SDK.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is the version of the service.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetServiceVersionAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetServiceVersionAsync(Action<string, BuddyCallbackParams> callback, object state = null)
        {
            GetServiceVersionInternal((bcr) =>
            {
                if (callback == null)
                    return;
                callback(bcr.Result, new BuddyCallbackParams(bcr.Error));
            });
            return null;
        }

        internal void GetServiceVersionInternal(Action<BuddyCallResult<string>> callback)
        {
            this.Service.Service_Version_Get(this.AppName, this.AppPassword, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None)
                {
                    callback(BuddyResultCreator.Create<string>(null, bcr.Error));
                    return;
                }
                {
                    callback(BuddyResultCreator.Create(result, bcr.Error));
                    return;
                }
                ;
            });
            return;

        }

        /// <summary>
        /// Gets a list of emails for all registered users for this app.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is the list of emails.</param>
        /// <param name="fromRow">Used for paging, retrieve only records starting fromRow.</param>
        /// <param name="pageSize">Used for paginig, specify page size.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetUserEmailsAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetUserEmailsAsync(Action<List<string>, BuddyCallbackParams> callback, int fromRow, int pageSize = 10, object state = null)
        {
            GetUserEmailsInternal(fromRow, pageSize, (bcr) =>
            {
                if (callback == null)
                    return;
                callback(bcr.Result, new BuddyCallbackParams(bcr.Error));
            });
            return null;
        }

        internal void GetUserEmailsInternal(int fromRow, int pageSize, Action<BuddyCallResult<List<string>>> callback)
        {
            this.Service.Application_Users_GetEmailList(this.AppName, this.AppPassword, fromRow.ToString(),
                    (fromRow + pageSize).ToString(), (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None)
                {
                    callback(BuddyResultCreator.Create<List<string>>(null, bcr.Error));
                    return;
                }
                List<string> emails = new List<string>();
                foreach (var d in result)
                    emails.Add(d.UserEmail);
                {
                    callback(BuddyResultCreator.Create(emails, bcr.Error));
                    return;
                }
                ;
            });
            return;

        }

       

        /// <summary>
        /// Gets a list of all user profiles for this app.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is the list of user profiles.</param>
        /// <param name="fromRow">Used for paging, retrieve only records starting fromRow.</param>
        /// <param name="pageSize">Used for paginig, specify page size.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetUserProfilesAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetUserProfilesAsync(Action<List<User>, BuddyCallbackParams> callback, int fromRow, int pageSize = 10, object state = null)
        {
            GetUserProfilesInternal(fromRow, pageSize, (bcr) =>
            {
                if (callback == null)
                    return;
                callback(bcr.Result, new BuddyCallbackParams(bcr.Error));
            });
            return null;
        }

        internal void GetUserProfilesInternal(int fromRow, int pageSize, Action<BuddyCallResult<List<User>>> callback)
        {
            this.Service.Application_Users_GetProfileList(this.AppName, this.AppPassword, fromRow.ToString(),
                    (fromRow + pageSize).ToString(), (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None)
                {
                    callback(BuddyResultCreator.Create<List<User>>(null, bcr.Error));
                    return;
                }
                var userProfiles = new List<User>();
                foreach (var userProfile in result)
                {
                    userProfiles.Add(new User(this, userProfile));
                }
                {
                    callback(BuddyResultCreator.Create(userProfiles, bcr.Error));
                    return;
                }
                ;
            });
            return;

        }

        /// <summary>
        /// This method will return a list of statistics for the application covering items such as total users, photos, etc. 
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is the list of application stats.</param>   
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetApplicationStatisticsAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetApplicationStatisticsAsync(Action<List<ApplicationStatistics>, BuddyCallbackParams> callback, object state = null)
        {
            GetApplicationStatisticsInternal((bcr) =>
            {
                if (callback == null)
                    return;
                callback(bcr.Result, new BuddyCallbackParams(bcr.Error));
            });
            return null;
        }

        internal void GetApplicationStatisticsInternal(Action<BuddyCallResult<List<ApplicationStatistics>>> callback)
        {
            this.Service.Application_Metrics_GetStats(this.AppName, this.AppPassword, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None)
                {
                    callback(BuddyResultCreator.Create<List<ApplicationStatistics>>(null, bcr.Error));
                    return;
                }
                List<ApplicationStatistics> stats = new List<ApplicationStatistics>();
                foreach (var d in result)
                    stats.Add(new ApplicationStatistics(d, this));
                {
                    callback(BuddyResultCreator.Create(stats, bcr.Error));
                    return;
                }
                ;
            });
            return;

        }

        /// <summary>
        /// Request a reset password email for the given user.
        /// </summary>
        /// <param name="userName">The username of the user to create a reset password request for.</param>
        /// <returns>A Task&lt;bool&gt; that can be used to monitor progress on this call.</returns>
        public Task<bool> RequestPasswordResetAsnyc(string userName)
        {
            var tcs = new TaskCompletionSource<bool>();

            this.RequestPasswordResetInternal(userName, (bcr) =>
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

        internal void RequestPasswordResetInternal(string userName, Action<BuddyCallResult<bool>> callback)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("BuddyApplicationName", this.AppName);
            parameters.Add("BuddyApplicationPassword", this.AppPassword);
            parameters.Add("UserName", userName);

            this.Service.CallMethodAsync<string>("UserAccount_Profile_RequestPasswordReset", parameters, (bcr) =>
                {
                    this.Service.CallOnUiThread((state) =>
                        callback(BuddyResultCreator.Create(bcr.Result == "1", bcr.Error)));
                });
        }

        /// <summary>
        /// Reset the password for the given user using the given reset code.
        /// </summary>
        /// <param name="userName">The username of the user to change the password for.</param>
        /// <param name="resetCode">The reset code for this user as seen in the users email.</param>
        /// <param name="newPassword">The new password to set for the given user.</param>
        /// <returns>A Task&lt;bool&gt; that can be used to monitor progress on this call.</returns>
        public Task<bool> ResetPasswordAsync(string userName, string resetCode, string newPassword)
        {
            var tcs = new TaskCompletionSource<bool>();

            this.ResetPasswordInternal(userName, resetCode, newPassword, (bcr) =>
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

        internal void ResetPasswordInternal(string userName, string resetCode, string newPassword, Action<BuddyCallResult<bool>> callback)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("BuddyApplicationName", this.AppName);
            parameters.Add("BuddyApplicationPassword", this.AppPassword);
            parameters.Add("UserName", userName);
            parameters.Add("ResetCode", resetCode);
            parameters.Add("NewPassword", newPassword);

            this.Service.CallMethodAsync<string>("UserAccount_Profile_ResetPassword", parameters, (bcr) =>
                {
                    this.Service.CallOnUiThread((state) =>
                        callback(BuddyResultCreator.Create(bcr.Result == "1", bcr.Error)));
                });
        }

        /// <summary>
        /// Login an existing user with their secret token. Each user is assigned a token on creation, you can store it instead of a
        /// username/password combination.
        /// </summary>
        /// <param name="token">The private token of the user to login.</param>
        /// <param name="callback">The async callback to call on success or error. The first parameter is a the authenticated user if the login was successful.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of LoginAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult LoginAsync(Action<AuthenticatedUser, BuddyCallbackParams> callback, string token, object state = null)
        {
            LoginInternal(token, (bcr) =>
            {
                if (callback == null)
                    return;
                callback(bcr.Result, new BuddyCallbackParams(bcr.Error));
            });
            return null;
        }

        internal void LoginInternal(string token, Action<BuddyCallResult<AuthenticatedUser>> callback)
        {
            if (String.IsNullOrEmpty("token"))
                throw new ArgumentException("Can't be null or empty.", "token");

            this.Service.UserAccount_Profile_GetFromUserToken(this.AppName, this.AppPassword, token, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None)
                {
                    callback(BuddyResultCreator.Create<AuthenticatedUser>(null, bcr.Error));
                    return;
                }
                if (result.Length == 0)
                {
                    callback(BuddyResultCreator.Create<AuthenticatedUser>(null, bcr.Error));
                    return;
                }
                ;

                var user = new AuthenticatedUser(token, result[0], this);

                this.RecordDeviceInfo(user);

                {
                    callback(BuddyResultCreator.Create(new AuthenticatedUser(token, result[0], this), bcr.Error));
                    return;
                }
                ;
            });
            return;


        }


        public Task<SocialAuthenticatedUser> SocialLoginAsync(string providerName, string providerUserId, string accessToken) 
        {
            var tcs = new TaskCompletionSource<SocialAuthenticatedUser>();

            this.SocialLoginInternal(providerName, providerUserId, accessToken, (bcr) =>
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

        internal void SocialLoginInternal(string providerName, string providerUserId, string accessToken, Action<BuddyServiceClient.BuddyCallResult<SocialAuthenticatedUser>> callback)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("BuddyApplicationName", this.AppName);
            parameters.Add("BuddyApplicationPassword", this.AppPassword);
            parameters.Add("ProviderName", providerName);
            parameters.Add("ProviderUserId", providerUserId);
            parameters.Add("AccessToken", accessToken);

            this.Service.CallMethodAsync<InternalModels.DataContract_SocialLoginReply[]>("UserAccount_Profile_SocialLogin", parameters, (bcr) =>
                {
                    SocialAuthenticatedUser usr = null;
                    if (bcr.Result != null)
                    {
                        this.LoginInternal(bcr.Result.First().UserToken, (bdr) =>
                            {
                                usr = new SocialAuthenticatedUser(this, bdr.Result, bool.Parse(bcr.Result.First().IsNew));
                                callback(BuddyServiceClient.BuddyResultCreator.Create(usr, bcr.Error));
                            });
                    }
                    else
                    {
                        callback(BuddyServiceClient.BuddyResultCreator.Create(usr, bcr.Error));
                    }

                });
        }

        private void RecordDeviceInfo(AuthenticatedUser user)
        {
            if (this.recordDeviceInfo)
            {
                var osVersion = this.GetOSVersion();
                var deviceName = this.GetDeviceName();
                var applicationId = this.GetApplicationId();

                this.Device.RecordInformationInternal(osVersion, deviceName, user, this.AppVersion, 0, 0, applicationId, (bcr) =>
                {
                    this.recordDeviceInfo = bcr.Error == BuddyError.None;
                });


            }
        }

        private string GetOSVersion()
        {
#if WINDOWS_PHONE
            return System.Environment.OSVersion.Version.ToString();
#else
            try
            {
                // .NET
                var osVersionProperty = typeof(Environment).GetRuntimeProperty("OSVersion");
                object osVersion = osVersionProperty.GetValue(null, null);
                var versionStringProperty = osVersion.GetType().GetRuntimeProperty("VersionString");
                var versionString = versionStringProperty.GetValue(osVersion, null);
                return (string)versionString;
            }
            catch
            {
            }

            return "DeviceOSVersion not found";
#endif
        }

        private string GetDeviceName()
        {
#if WINDOWS_PHONE
            return Microsoft.Phone.Info.DeviceStatus.DeviceName;
#else
            return "DeviceType not found";
#endif
        }

        private string GetApplicationId()
        {

#if WINDOWS_PHONE
            var xml = XElement.Load("WMAppManifest.xml");
            var prodId = (from app in xml.Descendants("App")
                            select app.Attribute("ProductID").Value).FirstOrDefault();
            if (string.IsNullOrEmpty(prodId)) return string.Empty;
            return new Guid(prodId).ToString();
#else
            try
            {
                // .NET
                var assemblyFullName = typeof(System.Diagnostics.Debug).GetTypeInfo().Assembly.FullName;
                var process = Type.GetType("System.Diagnostics.Process, " + assemblyFullName);
                var getCurrentProcessMethod = process.GetRuntimeMethod("GetCurrentProcess", new Type[0]);
                var currentProcess = getCurrentProcessMethod.Invoke(null, null);
                var processNameProperty = currentProcess.GetType().GetRuntimeProperty("ProcessName");
                var processName = processNameProperty.GetValue(currentProcess, null);
                return (string)processName;
            }
            catch
            {
            }

            try
            {
                // Windows Store
                var loadMethod = typeof(XDocument).GetRuntimeMethod("Load", new Type[] {
                    typeof(string),
                    typeof(LoadOptions)
                });
                var xDocument = loadMethod.Invoke(null, new object[] {
                    "AppxManifest.xml",
                    LoadOptions.None
                });

                var xNamespace = XNamespace.Get("http://schemas.microsoft.com/appx/2010/manifest");

                var identityElement = ((XDocument)xDocument).Descendants(xNamespace + "Identity").First();

                return identityElement.Attribute("Name").Value;
            }
            catch
            {
            }

            return "ApplicationId not found";
#endif
        }



        

        /// <summary>
        /// Login an existing user with their username and password. Note that this method internally does two web-service calls, and the IAsyncResult object
        /// returned is only valid for the first one.
        /// </summary>
        /// <param name="username">The username of the user. Can't be null or empty.</param>
        /// <param name="password">The password of the user. Can't be null.</param>
        /// <param name="callback">The async callback to call on success or error. The first parameter is an authenticated user if the Login was successful.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of LoginAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult LoginAsync(Action<AuthenticatedUser, BuddyCallbackParams> callback, string username, string password, object state = null)
        {
            LoginInternal(username, password, (bcr) =>
            {
                if (callback == null)
                    return;
                callback(bcr.Result, new BuddyCallbackParams(bcr.Error));
            });
            return null;
        }


        internal void LoginInternal(string username, string password, Action<BuddyCallResult<AuthenticatedUser>> callback)
        {
            if (String.IsNullOrEmpty("username"))
                throw new ArgumentException("Can't be null or empty.", "username");
            if (password == null)
                throw new ArgumentNullException("password");

            this.Service.UserAccount_Profile_Recover(this.AppName, this.AppPassword, username, password, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None)
                {
                    callback(BuddyResultCreator.Create<AuthenticatedUser>(null, bcr.Error));
                    return;
                }
                LoginInternal(result, callback);

            });
            return;

        }

       

        /// <summary>
        /// Check if another user with the same email already exists in the system.
        /// </summary>
        /// <param name="email">The email to check for, can't be null or empty.</param>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true if the email exists in the system, false otherwise.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of CheckIfEmailExistsAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult CheckIfEmailExistsAsync(Action<bool, BuddyCallbackParams> callback, string email, object state = null)
        {
            CheckIfEmailExistsInternal(email, (bcr) =>
            {
                if (callback == null)
                    return;
                callback(bcr.Result, new BuddyCallbackParams(bcr.Error));
            });
            return null;
        }

        internal void CheckIfEmailExistsInternal(string email, Action<BuddyCallResult<bool>> callback)
        {
            if (String.IsNullOrEmpty(email))
                throw new ArgumentException("Can't be null or empty.", "email");

            this.Service.UserAccount_Profile_CheckUserEmail(this.AppName, this.AppPassword, email, (bcr) =>
            {

                callback(BuddyResultCreator.Create(bcr.Error == BuddyError.UserEmailTaken, BuddyError.None));
            });
            return;

        }

      

        /// <summary>
        /// Check if another user with the same name already exists in the system.
        /// </summary>
        /// <param name="username">The name to check for, can't be null or empty.</param>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true if the name exists in the system, false otherwise.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of CheckIfUsernameExistsAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult CheckIfUsernameExistsAsync(Action<bool, BuddyCallbackParams> callback, string username, object state = null)
        {
            CheckIfUsernameExistsInternal(username, (bcr) =>
            {
                if (callback == null)
                    return;
                callback(bcr.Result, new BuddyCallbackParams(bcr.Error));
            });
            return null;
        }

        internal void CheckIfUsernameExistsInternal(string username, Action<BuddyCallResult<bool>> callback)
        {
            if (String.IsNullOrEmpty(username))
                throw new ArgumentException("Can't be null or empty.", "username");

            this.Service.UserAccount_Profile_CheckUserName(this.AppName, this.AppPassword, username, (bcr) =>
            {
                if (bcr.Error != BuddyError.None && bcr.Error != BuddyError.UserNameAvailble && bcr.Error != BuddyError.UserNameAlreadyInUse)
                {
                    callback(BuddyResultCreator.Create(default(bool), bcr.Error));
                    return;
                }
                callback(BuddyResultCreator.Create(bcr.Error == BuddyError.UserNameAlreadyInUse, BuddyError.None));

            });
            return;

        }


        /// <summary>
        /// Create a new Buddy user. Note that this method internally does two web-service calls, and the IAsyncResult object
        /// returned is only valid for the first one.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is an AuthenticatedUser object is returned.</param>
        /// <param name="name">The name of the new user. Can't be null or empty.</param>
        /// <param name="password">The password of the new user. Can't be null.</param>
        /// <param name="gender">An optional gender for the user.</param>
        /// <param name="age">An optional age for the user.</param>
        /// <param name="email">An optional email for the user.</param>
        /// <param name="status">An optional status for the user.</param>
        /// <param name="fuzzLocation">Optionally set location fuzzing for this user. When enabled user location is randomized in searches.</param>
        /// <param name="celebrityMode">Optionally set the celebrity mode for this user. When enabled this user will be absent from all searches.</param>
        /// <param name="appTag">An optional custom tag for this user.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of CreateUserAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult CreateUserAsync(Action<AuthenticatedUser, BuddyCallbackParams> callback, string name, string password, UserGender gender = UserGender.Any, int age = 0,
                            string email = "", UserStatus status = UserStatus.Any, bool fuzzLocation = false, bool celebrityMode = false, string appTag = "", object state = null)
        {
            CreateUserInternal(name, password, gender, age, email, status, fuzzLocation, celebrityMode, appTag, (bcr) =>
            {
                if (callback == null)
                    return;
                callback(bcr.Result, new BuddyCallbackParams(bcr.Error));
            });
            return null;
        }

        internal void CreateUserInternal(string name, string password, UserGender gender, int age,
                    string email, UserStatus status, bool fuzzLocation, bool celebrityMode, string appTag, Action<BuddyCallResult<AuthenticatedUser>> callback)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentException("Can't be null or empty.", "name");
            if (password == null)
                throw new ArgumentNullException("password");
            if (age < 0)
                throw new ArgumentException("Can't be less than 0.", "age");

            this.Service.UserAccount_Profile_Create(this.AppName, this.AppPassword, name, password,
                            gender.ToString().ToLowerInvariant(), age, email == null ? "" : email, (int)status, fuzzLocation ? 1 : 0,
                            celebrityMode ? 1 : 0, appTag, (bcr) =>
            {
                if (bcr.Error != BuddyError.None)
                {
                    callback(BuddyResultCreator.Create<AuthenticatedUser>(null, bcr.Error));
                    return;
                }
                this.LoginInternal(bcr.Result, callback);
            });
            return;

        }


        //
        // Analytics_Session methods
        //

      

        /// <summary>
        /// Starts an analytics session
        /// </summary>
        /// <param name="callback">The callback to call upon success or error.  The first parameter is an identifier for the session.</param>
        /// <param name="user">The user that is starting this session</param>
        /// <param name="sessionName">The name of the session</param>
        /// <param name="appTag">An optional custom tag to include with the session.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns></returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of StartSessionAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult StartSessionAsync(Action<int, BuddyCallbackParams> callback, AuthenticatedUser user, string sessionName, string appTag = null, object state = null)
        {

            StartSessionInternal(user, sessionName, appTag, (bcr) =>
            {
                if (callback == null)
                    return;
                callback(bcr.Result, new BuddyCallbackParams(bcr.Error));
            });
            return null;
        }

        internal void StartSessionInternal(AuthenticatedUser user, string sessionName, string appTag, Action<BuddyCallResult<int>> callback)
        {
            if (user == null || user.Token == null)
                throw new ArgumentNullException("user", "An AuthenticatedUser value is required.");
            if (String.IsNullOrEmpty(sessionName))
                throw new ArgumentException("sessionNae", "sessionName must not be null or empty.");

            this.Service.Analytics_Session_Start(this.AppName, this.AppPassword, user.Token, sessionName, appTag, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None)
                {
                    callback(BuddyResultCreator.Create(default(int), bcr.Error));
                    return;
                }
                {
                    callback(BuddyResultCreator.Create(Int32.Parse(result), bcr.Error));
                    return;
                }
                ;
            });
            return;

        }

     

        /// <summary>
        /// Ends an analytics session
        /// </summary>
        /// <param name="callback">The callback to call upon success or error.  The first parameter a boolean which is true upon success.</param>
        /// <param name="user">The user that is starting this session</param>
        /// <param name="sessionId">The id of the session, returned from StartSessionAsync.</param>
        /// <param name="appTag">An optional custom tag to include with the session.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns></returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of EndSessionAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult EndSessionAsync(Action<bool, BuddyCallbackParams> callback, AuthenticatedUser user, int sessionId, string appTag = null, object state = null)
        {

            EndSessionInternal(user, sessionId, appTag, (bcr) =>
            {
                if (callback == null)
                    return;
                callback(bcr.Result, new BuddyCallbackParams(bcr.Error));
            });
            return null;
        }

        internal void EndSessionInternal(AuthenticatedUser user, int sessionId, string appTag, Action<BuddyCallResult<bool>> callback)
        {
            if (user == null || user.Token == null)
                throw new ArgumentNullException("An AuthenticatedUser value is required for parmaeter user");


            this.Service.Analytics_Session_End(this.AppName, this.AppPassword, user.Token, sessionId.ToString(), appTag, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None && bcr.Error != BuddyError.ServiceErrorNegativeOne)
                {
                    callback(BuddyResultCreator.Create(default(bool), bcr.Error));
                    return;
                }
                {
                    callback(BuddyResultCreator.Create(result == "1", bcr.Error));
                    return;
                }
                ;
            });
            return;

        }


        /// <summary>
        /// Records a session metric value
        /// </summary>
        /// <param name="callback">The callback to call upon success or error.  The first parameter a boolean which is true upon success.</param>
        /// <param name="user">The user that is starting this session</param>
        /// <param name="sessionId">The id of the session, returned from StartSessionAsync.</param>
        /// <param name="metricKey">A custom key describing the metric.</param>
        /// <param name="metricValue">The value to set.</param>
        /// <param name="appTag">An optional custom tag to include with the metric.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns></returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of RecordSessionMetricAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult RecordSessionMetricAsync(Action<bool, BuddyCallbackParams> callback, AuthenticatedUser user, int sessionId, string metricKey, string metricValue, string appTag = null, object state = null)
        {

            RecordSessionMetricInternal(user, sessionId, metricKey, metricValue, appTag, (bcr) =>
            {
                if (callback == null)
                    return;
                callback(bcr.Result, new BuddyCallbackParams(bcr.Error));
            });
            return null;
        }

        internal void RecordSessionMetricInternal(AuthenticatedUser user, int sessionId, string metricKey, string metricValue, string appTag, Action<BuddyCallResult<bool>> callback)
        {
            if (user == null || user.Token == null)
                throw new ArgumentNullException("user", "An AuthenticatedUser value is required for parmaeter user");
            if (String.IsNullOrEmpty(metricKey))
                throw new ArgumentException("metricKey", "metricKey must not be null or empty.");
            if (metricValue == null)
                throw new ArgumentNullException("metricValue", "metrickValue must not be null.");

            this.Service.Analytics_Session_RecordMetric(this.AppName, this.AppPassword, user.Token, sessionId.ToString(), metricKey, metricValue, appTag, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None)
                {
                    callback(BuddyResultCreator.Create(default(bool), bcr.Error));
                    return;
                }
                {
                    callback(BuddyResultCreator.Create(result == "1", bcr.Error));
                    return;
                }
                ;
            });
            return;

        }

        internal double TryParseDouble(string value)
        {
            double result;
            if (value == "")
            {
                return 0;
            }
            Double.TryParse(value, out result);
            if (value == "")
            {
                return result;
            }
            else
                return Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }


   #if AWAIT_SUPPORTED

      /// <summary>
        /// Check if another user with the same name already exists in the system.
        /// </summary>
        /// <param name="username">The name to check for, can't be null or empty.</param>
        /// <returns>A Task&lt;Boolean&gt;that can be used to monitor progress on this call.</returns>
        public System.Threading.Tasks.Task<Boolean> CheckIfUsernameExistsAsync( string username)
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<Boolean>();
            CheckIfUsernameExistsInternal(username, (bcr) =>
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
        /// Create a new Buddy user. Note that this method internally does two web-service calls, and the IAsyncResult object
        /// returned is only valid for the first one.
        /// </summary>
        /// <param name="name">The name of the new user. Can't be null or empty.</param>
        /// <param name="password">The password of the new user. Can't be null.</param>
        /// <param name="gender">An optional gender for the user.</param>
        /// <param name="age">An optional age for the user.</param>
        /// <param name="email">An optional email for the user.</param>
        /// <param name="status">An optional status for the user.</param>
        /// <param name="fuzzLocation">Optionally set location fuzzing for this user. When enabled user location is randomized in searches.</param>
        /// <param name="celebrityMode">Optionally set the celebrity mode for this user. When enabled this user will be absent from all searches.</param>
        /// <param name="appTag">An optional custom tag for this user.</param>
        /// <returns>A Task&lt;AuthenticatedUser&gt;that can be used to monitor progress on this call.</returns>
        public System.Threading.Tasks.Task<AuthenticatedUser> CreateUserAsync( string name, string password, Buddy.UserGender gender = UserGender.Any, int age = 0, string email = "", Buddy.UserStatus status = UserStatus.Any, bool fuzzLocation = false, bool celebrityMode = false, string appTag = "")
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<AuthenticatedUser>();
            CreateUserInternal(name, password, gender, age, email, status, fuzzLocation, celebrityMode, appTag, (bcr) =>
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
        /// Starts an analytics session
        /// </summary>
        /// <param name="user">The user that is starting this session</param>
        /// <param name="sessionName">The name of the session</param>
        /// <param name="appTag">An optional custom tag to include with the session.</param>
        /// <returns></returns>
        public System.Threading.Tasks.Task<Int32> StartSessionAsync( Buddy.AuthenticatedUser user, string sessionName, string appTag = null)
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<Int32>();
            StartSessionInternal(user, sessionName, appTag, (bcr) =>
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
        /// Ends an analytics session
        /// </summary>
        /// <param name="user">The user that is starting this session</param>
        /// <param name="sessionId">The id of the session, returned from StartSessionAsync.</param>
        /// <param name="appTag">An optional custom tag to include with the session.</param>
        /// <returns></returns>
        public System.Threading.Tasks.Task<Boolean> EndSessionAsync( Buddy.AuthenticatedUser user, int sessionId, string appTag = null)
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<Boolean>();
            EndSessionInternal(user, sessionId, appTag, (bcr) =>
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
        /// Records a session metric value
        /// </summary>
        /// <param name="user">The user that is starting this session</param>
        /// <param name="sessionId">The id of the session, returned from StartSessionAsync.</param>
        /// <param name="metricKey">A custom key describing the metric.</param>
        /// <param name="metricValue">The value to set.</param>
        /// <param name="appTag">An optional custom tag to include with the metric.</param>
        /// <returns></returns>
        public System.Threading.Tasks.Task<Boolean> RecordSessionMetricAsync( Buddy.AuthenticatedUser user, int sessionId, string metricKey, string metricValue, string appTag = null)
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<Boolean>();
            RecordSessionMetricInternal(user, sessionId, metricKey, metricValue, appTag, (bcr) =>
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
        /// Ping the service.
        /// </summary>
        /// <returns>A Task&lt;String&gt;that can be used to monitor progress on this call.</returns>
        public System.Threading.Tasks.Task<String> PingAsync()
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<String>();
            PingInternal((bcr) =>
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
        /// Get the current Buddy web-service date/time.
        /// </summary>
        /// <returns>A Task&lt;DateTime&gt;that can be used to monitor progress on this call.</returns>
        public System.Threading.Tasks.Task<DateTime> GetServiceTimeAsync()
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<DateTime>();
            GetServiceTimeInternal((bcr) =>
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
        /// Get the current version of the service that is being used by this SDK.
        /// </summary>
        /// <returns>A Task&lt;String&gt;that can be used to monitor progress on this call.</returns>
        public System.Threading.Tasks.Task<String> GetServiceVersionAsync()
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<String>();
            GetServiceVersionInternal((bcr) =>
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
        /// Gets a list of emails for all registered users for this app.
        /// </summary>
        /// <param name="fromRow">Used for paging, retrieve only records starting fromRow.</param>
        /// <param name="pageSize">Used for paginig, specify page size.</param>
        /// <returns>A Task&lt;IEnumerable&lt;String&gt; &gt;that can be used to monitor progress on this call.</returns>
        public System.Threading.Tasks.Task<IEnumerable<String>> GetUserEmailsAsync( int fromRow, int pageSize = 10)
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<IEnumerable<String>>();
            GetUserEmailsInternal(fromRow, pageSize, (bcr) =>
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
        /// Gets a list of all user profiles for this app.
        /// </summary>
        /// <param name="fromRow">Used for paging, retrieve only records starting fromRow.</param>
        /// <param name="pageSize">Used for paginig, specify page size.</param>
        /// <returns>A Task&lt;IEnumerable&lt;User&gt; &gt;that can be used to monitor progress on this call.</returns>
        public System.Threading.Tasks.Task<IEnumerable<User>> GetUserProfilesAsync( int fromRow, int pageSize = 10)
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<IEnumerable<User>>();
            GetUserProfilesInternal(fromRow, pageSize, (bcr) =>
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
        /// This method will return a list of statistics for the application covering items such as total users, photos, etc. 
        /// </summary>
        /// <returns>A Task&lt;IEnumerable&lt;ApplicationStatistics&gt; &gt;that can be used to monitor progress on this call.</returns>
        public System.Threading.Tasks.Task<IEnumerable<ApplicationStatistics>> GetApplicationStatisticsAsync()
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<IEnumerable<ApplicationStatistics>>();
            GetApplicationStatisticsInternal((bcr) =>
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
        /// Login an existing user with their secret token. Each user is assigned a token on creation, you can store it instead of a
        /// username/password combination.
        /// </summary>
        /// <param name="token">The private token of the user to login.</param>
        /// <returns>A Task&lt;AuthenticatedUser&gt;that can be used to monitor progress on this call.</returns>
        public System.Threading.Tasks.Task<AuthenticatedUser> LoginAsync( string token)
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<AuthenticatedUser>();
            LoginInternal(token, (bcr) =>
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
        /// Login an existing user with their username and password. Note that this method internally does two web-service calls, and the IAsyncResult object
        /// returned is only valid for the first one.
        /// </summary>
        /// <param name="username">The username of the user. Can't be null or empty.</param>
        /// <param name="password">The password of the user. Can't be null.</param>
        /// <returns>A Task&lt;AuthenticatedUser&gt;that can be used to monitor progress on this call.</returns>
        public System.Threading.Tasks.Task<AuthenticatedUser> LoginAsync( string username, string password)
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<AuthenticatedUser>();
            LoginInternal(username, password, (bcr) =>
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
        /// Check if another user with the same email already exists in the system.
        /// </summary>
        /// <param name="email">The email to check for, can't be null or empty.</param>
        /// <returns>A Task&lt;Boolean&gt;that can be used to monitor progress on this call.</returns>
        public System.Threading.Tasks.Task<Boolean> CheckIfEmailExistsAsync( string email)
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<Boolean>();
            CheckIfEmailExistsInternal(email, (bcr) =>
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

   

   

    /// <summary>
    /// 
    /// <example>
    /// <code>
    ///    
    /// </code>
    /// </example>
    /// </summary>
    public class ApplicationStatistics
    {
        /// <summary>
        /// 
        /// </summary>
        public string TotalUsers { get; protected set; }

        /// <summary>
        /// This is the combined total of all profile photos and photo album photos for the application
        /// </summary>
        public string TotalPhotos { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public string TotalUserCheckins { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public string TotalUserMetadata { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public string TotalAppMetadata { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public string TotalFriends { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public string TotalAlbums { get; protected set; }


        /// <summary>
        /// 
        /// </summary>
        public string TotalCrashes { get; protected set; }


        /// <summary>
        /// 
        /// </summary>
        public string TotalMessages { get; protected set; }


        /// <summary>
        /// This is the combined total of all push notifications sent for all platforms supported 
        /// </summary>
        public string TotalPushMessages { get; protected set; }


        /// <summary>
        /// 
        /// </summary>
        public string TotalGamePlayers { get; protected set; }


        /// <summary>
        /// 
        /// </summary>
        public string TotalGameScores { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public string TotalDeviceInformation { get; protected set; }

        internal ApplicationStatistics(InternalModels.DataContract_ApplicationStats appStats, BuddyClient client)
        {
            if (client == null)
                throw new ArgumentNullException("client");

            this.TotalUsers = appStats.TotalUsers;

            this.TotalDeviceInformation = appStats.TotalDeviceInformation;

            this.TotalCrashes = appStats.TotalCrashes;

            this.TotalAppMetadata = appStats.TotalAppMetadata;

            this.TotalAlbums = appStats.TotalAlbums;

            this.TotalFriends = appStats.TotalFriends;

            this.TotalGamePlayers = appStats.TotalGamePlayers;

            this.TotalGameScores = appStats.TotalGameScores;

            this.TotalMessages = appStats.TotalMessages;

            this.TotalPhotos = appStats.TotalPhotos;

            this.TotalPushMessages = appStats.TotalPushMessages;

            this.TotalUserCheckins = appStats.TotalUserCheckins;

            this.TotalUserMetadata = appStats.TotalUserMetadata;
        }


    }

#if NET40
    public static class ReflectionExtensions
    {
        public static Type GetTypeInfo(this Type type)
        {
            return type;
        }

        public static Attribute[] GetCustomAttributes(this Assembly a)
        {
            var attrs = a.GetCustomAttributes(true);
            if (attrs == null)
            {
                return null;
            }
            Attribute[] attributes = new Attribute[attrs.Length];
            Array.Copy(attrs, attributes, attrs.Length);
            return attributes;
        }

        public static PropertyInfo GetRuntimeProperty(this Type type, string name)
        {
            return type.GetProperty(name);
        }
        public static MethodInfo GetRuntimeMethod(this Type type, string name, Type[] parameterTypes = null)
        {
            return type.GetMethod(name, parameterTypes);
        }
    }
#endif

}

