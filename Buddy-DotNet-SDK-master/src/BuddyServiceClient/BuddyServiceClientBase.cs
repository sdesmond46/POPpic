using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;

namespace BuddyServiceClient
{

    internal class BuddyException : Exception
    {
        public string Method { get; private set; }
        public BuddyError Error { get; private set; }

        public override string Message
        {
            get
            {
                return Error.ToString();
            }
        }
        public BuddyException(BuddyError error, string method)
        {
            Error = error;
            Method = method;
            Debug.WriteLine(method + ": " + error);
        }
    }

    internal class BuddyCallResult<T>
    {
        public BuddyError Error { get; set; }
        public string Message { get; set; }
        public T Result { get; set; }

        public BuddyCallResult()
        {
        }


    }

    internal class BuddyFile
    {
        public Stream Data;

        private byte[] _bytes;
        public byte[] Bytes
        {
            get
            {
                if (Data != null && _bytes == null)
                {
                    _bytes = new byte[Data.Length];
                    Data.Read(_bytes, 0, _bytes.Length);

                }
                return _bytes;
            }
        }
        public string Name;
        public string ContentType = "application/octet-stream";

        public BuddyFile()
        {

        }
    }

    internal enum BuddyError
    {
        None = 0,
        WrongSocketLoginOrPass, SecurityTokenInvalidPleaseRenew, SecurityTokenRenewed, SecurityTokenCouldNotBeRenewed, SecurityFailedBannedDeviceID,
        SecurityFailedBadUserNameOrPassword, SecurityFailedBadUserName, SecurityFailedBadUserPassword, DeviceIDAlreadyInSystem, UserNameAlreadyInUse,
        UserNameAvailble, UserAccountNotFound, UserInvalidAccountSetting, UserAccountErrorSettingMetaValue, UserErrorUpdatingAccount, UserEmailTaken,
        UserEmailAvailable, UserProfileIDEmpty, IdentityValueEmpty, DeviceIDNotFound, DateTimeFormatWasIncorrect, LatLongFormatWasIncorrect,
        GeoLocationCategoryIncorrect, BadGeoLocationName, GeoLocationIDIncorrect, BadParameter, PhotoUploadGenericError, CouldNotFindPhotoTodelete,
        CouldNotDeleteFileGenericError, PhotoAlbumDoesNotExist, AlbumNamesCannotBeBlank, PhotoIDDoesNotExistInContext, dupelocation, invalidflagreason,
        EmptyDeviceURI, EmptyGroupName, EmptyImageURI, EmptyMessageCount, EmptyMessageTitle, EmptyRawMessage, EmptyToastTitle, EmptyToastSubTitle,
        EmptyToastParameter, GroupNameCannotBeEmpty, GroupSecurityCanOnlyBy0or1, GroupAlreadyExists, GroupChatIDEmpty, GroupChatNotFound, GroupOwnerSecurityError,
        ApplicationAPICallDisabledByDeveloper, ServiceErrorNull, ServiceErrorNegativeOne, UnknownServiceError, InternetConnectionError, UserIDMustBeAnInteger, BlobDoesNotExist,
        NoSuchSocialProvider, AccessTokenInvalid, FileLargerThanMaxSize, NoEmailSetForUser,
        PasswordResetInvalidResetCode, PasswordResetNotConfigured, PasswordResetTooManyRequests, UnexpectedServiceError
    }

    internal abstract partial class BuddyServiceClientBase
    {

        protected abstract string ClientName { get; }
        protected abstract string ClientVersion { get; }

        public virtual bool IsLocal
        {
            get
            {
                return false;
            }
        }

        protected BuddyServiceClientBase()
        {
            _syncContext = System.Threading.SynchronizationContext.Current;
        }

        private SynchronizationContext _syncContext;
        internal void CallOnUiThread(SendOrPostCallback callback)
        {
            if (_syncContext != null)
            {
                _syncContext.Post(callback, null);
            }
            else
            {
                callback(null);
            }
        }

        public virtual BuddyCallResult<T> CallMethod<T>(string methodName, IDictionary<string, object> parameters)
        {
            AutoResetEvent waitHandle = new AutoResetEvent(false);
            BuddyCallResult<T> bcr = null;

            CallMethodAsync<T>(methodName, parameters, (r) =>
            {
                bcr = r;
                waitHandle.Set();
            });

            waitHandle.WaitOne();
            return bcr;
        }

        public abstract void CallMethodAsync<T>(string methodName, IDictionary<string, object> parameters, Action<BuddyCallResult<T>> callback);
               
        private Regex IntRegex = new Regex("-?\\d+");

        private static bool ParseBuddyError(string str, out BuddyError err)
        {
            err = BuddyError.None;
            try
            {
                if (!str.StartsWith("{")) {
                    err = (BuddyError)Enum.Parse(typeof(BuddyError), str, true);
                }
                return true;
            }
            catch
            {
                err = BuddyError.None;
                return false;
            }
        }

        protected virtual BuddyError GetBuddyError(string response)
        {
            BuddyError err;
            if (response == "-1") {
                return BuddyError.ServiceErrorNegativeOne;
            }
            else if (response == "null" || response == null)
            {
                return BuddyError.ServiceErrorNull;
            }
            else if (String.IsNullOrEmpty(response))
            {
                return BuddyError.UnknownServiceError;
            }
            else if (!IntRegex.IsMatch(response) && ParseBuddyError(response, out err))
            {
                return err;
            }
            else if (response.StartsWith("Specified argument was out of the range of valid values.\r\n"))
            {
                return BuddyError.BadParameter;
            }
            return BuddyError.None;
        }


       
    }

  

    internal static class BuddyResultCreator
    {
        public static BuddyCallResult<T> Create<T>(T result, BuddyError err)
        {
            return new BuddyCallResult<T>(){Result = result, Error = err};
        }
    }




   




#region gen


internal partial class BuddyServiceClientBase {


public void MetaData_ApplicationMetaDataValue_BatchSum(String BuddyApplicationName, String BuddyApplicationPassword, String ApplicationMetaKeyCollection, String SearchDistanceCollection, String Latitude, String Longitude, String TimeFilter, String ApplicationTag, Action<BuddyCallResult<InternalModels.DataContract_MetaDataBatchSum[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["ApplicationMetaKeyCollection"] = ApplicationMetaKeyCollection;
	parameters["SearchDistanceCollection"] = SearchDistanceCollection;
	parameters["Latitude"] = Latitude;
	parameters["Longitude"] = Longitude;
	parameters["TimeFilter"] = TimeFilter;
	parameters["ApplicationTag"] = ApplicationTag;

	
	CallMethodAsync<InternalModels.DataContract_MetaDataBatchSum[]>("MetaData_ApplicationMetaDataValue_BatchSum", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void MetaData_ApplicationMetaDataValue_SearchData(String BuddyApplicationName, String BuddyApplicationPassword, String SearchDistance, String Latitude, String Longitude, String RecordLimit, String MetaKeySearch, String MetaValueSearch, String TimeFilter, String MetaValueMin, String MetaValueMax, String SearchAsFloat, String SortResultsDirection, String DisableCach, Action<BuddyCallResult<InternalModels.DataContract_SearchAppMetaData[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["SearchDistance"] = SearchDistance;
	parameters["Latitude"] = Latitude;
	parameters["Longitude"] = Longitude;
	parameters["RecordLimit"] = RecordLimit;
	parameters["MetaKeySearch"] = MetaKeySearch;
	parameters["MetaValueSearch"] = MetaValueSearch;
	parameters["TimeFilter"] = TimeFilter;
	parameters["MetaValueMin"] = MetaValueMin;
	parameters["MetaValueMax"] = MetaValueMax;
	parameters["SearchAsFloat"] = SearchAsFloat;
	parameters["SortResultsDirection"] = SortResultsDirection;
	parameters["DisableCach"] = DisableCach;

	
	CallMethodAsync<InternalModels.DataContract_SearchAppMetaData[]>("MetaData_ApplicationMetaDataValue_SearchData", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void MetaData_ApplicationMetaDataValue_SearchNearby(String BuddyApplicationName, String BuddyApplicationPassword, String SearchDistance, String Latitude, String Longitude, String RecordLimit, String MetaKeySearch, String MetaValueSearch, String TimeFilter, String MetaValueMin, String MetaValueMax, String SortResultsDirection, String DisableCach, Action<BuddyCallResult<InternalModels.DataContract_SearchAppMetaData[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["SearchDistance"] = SearchDistance;
	parameters["Latitude"] = Latitude;
	parameters["Longitude"] = Longitude;
	parameters["RecordLimit"] = RecordLimit;
	parameters["MetaKeySearch"] = MetaKeySearch;
	parameters["MetaValueSearch"] = MetaValueSearch;
	parameters["TimeFilter"] = TimeFilter;
	parameters["MetaValueMin"] = MetaValueMin;
	parameters["MetaValueMax"] = MetaValueMax;
	parameters["SortResultsDirection"] = SortResultsDirection;
	parameters["DisableCach"] = DisableCach;

	
	CallMethodAsync<InternalModels.DataContract_SearchAppMetaData[]>("MetaData_ApplicationMetaDataValue_SearchNearby", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void MetaData_UserMetaDataValue_GetAll(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, Action<BuddyCallResult<InternalModels.DataContract_UserMetaData[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;

	
	CallMethodAsync<InternalModels.DataContract_UserMetaData[]>("MetaData_UserMetaDataValue_GetAll", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void MetaData_UserMetaDataValue_Get(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String MetaKey, Action<BuddyCallResult<InternalModels.DataContract_UserMetaData[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["MetaKey"] = MetaKey;

	
	CallMethodAsync<InternalModels.DataContract_UserMetaData[]>("MetaData_UserMetaDataValue_Get", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void MetaData_UserMetaDataValue_Set(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String MetaKey, String MetaValue, String MetaLatitude, String MetaLongitude, String ApplicationTag, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["MetaKey"] = MetaKey;
	parameters["MetaValue"] = MetaValue;
	parameters["MetaLatitude"] = MetaLatitude;
	parameters["MetaLongitude"] = MetaLongitude;
	parameters["ApplicationTag"] = ApplicationTag;

	
	CallMethodAsync<String>("MetaData_UserMetaDataValue_Set", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void MetaData_UserMetaDataValue_BatchSet(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String UserMetaKeyCollection, String UserMetaValueCollection, String MetaLatitude, String MetaLongitude, String ApplicationTag, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["UserMetaKeyCollection"] = UserMetaKeyCollection;
	parameters["UserMetaValueCollection"] = UserMetaValueCollection;
	parameters["MetaLatitude"] = MetaLatitude;
	parameters["MetaLongitude"] = MetaLongitude;
	parameters["ApplicationTag"] = ApplicationTag;

	
	CallMethodAsync<String>("MetaData_UserMetaDataValue_BatchSet", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void MetaData_UserMetaDataValue_Search(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String SearchDistance, String Latitude, String Longitude, String RecordLimit, String MetaKeySearch, String MetaValueSearch, String TimeFilter, String SortValueAsFloat, String SortDirection, String DisableCache, Action<BuddyCallResult<InternalModels.DataContract_SearchUserMetaData[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["SearchDistance"] = SearchDistance;
	parameters["Latitude"] = Latitude;
	parameters["Longitude"] = Longitude;
	parameters["RecordLimit"] = RecordLimit;
	parameters["MetaKeySearch"] = MetaKeySearch;
	parameters["MetaValueSearch"] = MetaValueSearch;
	parameters["TimeFilter"] = TimeFilter;
	parameters["SortValueAsFloat"] = SortValueAsFloat;
	parameters["SortDirection"] = SortDirection;
	parameters["DisableCache"] = DisableCache;

	
	CallMethodAsync<InternalModels.DataContract_SearchUserMetaData[]>("MetaData_UserMetaDataValue_Search", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void MetaData_UserMetaDataValue_Sum(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String MetaKey, String SearchDistance, String Latitude, String Longitude, String TimeFilter, String ApplicationTag, Action<BuddyCallResult<InternalModels.DataContract_MetaDataSum[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["MetaKey"] = MetaKey;
	parameters["SearchDistance"] = SearchDistance;
	parameters["Latitude"] = Latitude;
	parameters["Longitude"] = Longitude;
	parameters["TimeFilter"] = TimeFilter;
	parameters["ApplicationTag"] = ApplicationTag;

	
	CallMethodAsync<InternalModels.DataContract_MetaDataSum[]>("MetaData_UserMetaDataValue_Sum", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void MetaData_UserMetaDataValue_Delete(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String MetaKey, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["MetaKey"] = MetaKey;

	
	CallMethodAsync<String>("MetaData_UserMetaDataValue_Delete", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void MetaData_UserMetaDataValue_DeleteAll(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;

	
	CallMethodAsync<String>("MetaData_UserMetaDataValue_DeleteAll", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void MetaData_UserMetaDataValue_BatchSum(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String UserMetaKeyCollection, String SearchDistanceCollection, String Latitude, String Longitude, String TimeFilter, String ApplicationTag, Action<BuddyCallResult<InternalModels.DataContract_MetaDataBatchSum[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["UserMetaKeyCollection"] = UserMetaKeyCollection;
	parameters["SearchDistanceCollection"] = SearchDistanceCollection;
	parameters["Latitude"] = Latitude;
	parameters["Longitude"] = Longitude;
	parameters["TimeFilter"] = TimeFilter;
	parameters["ApplicationTag"] = ApplicationTag;

	
	CallMethodAsync<InternalModels.DataContract_MetaDataBatchSum[]>("MetaData_UserMetaDataValue_BatchSum", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void PushNotifications_WP_RegisterDevice(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String DeviceURI, String GroupName, Boolean EnableTileMessages, Boolean EnableRawMessages, Boolean EnableToastMessages, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["DeviceURI"] = DeviceURI;
	parameters["GroupName"] = GroupName;
	parameters["EnableTileMessages"] = EnableTileMessages;
	parameters["EnableRawMessages"] = EnableRawMessages;
	parameters["EnableToastMessages"] = EnableToastMessages;

	
	CallMethodAsync<String>("PushNotifications_WP_RegisterDevice", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void PushNotifications_WP_RemoveDevice(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;

	
	CallMethodAsync<String>("PushNotifications_WP_RemoveDevice", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void PushNotifications_WP_SendLiveTile(String BuddyApplicationName, String BuddyApplicationPassword, String UserTokenOrID, String ImageURI, String MessageCount, String MessageTitle, String DeliverAfter, String GroupName, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserTokenOrID"] = UserTokenOrID;
	parameters["ImageURI"] = ImageURI;
	parameters["MessageCount"] = MessageCount;
	parameters["MessageTitle"] = MessageTitle;
	parameters["DeliverAfter"] = DeliverAfter;
	parameters["GroupName"] = GroupName;

	
	CallMethodAsync<String>("PushNotifications_WP_SendLiveTile", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void PushNotifications_WP_SendRawMessage(String BuddyApplicationName, String BuddyApplicationPassword, String UserTokenOrID, String RawMessage, String DeliverAfter, String GroupName, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserTokenOrID"] = UserTokenOrID;
	parameters["RawMessage"] = RawMessage;
	parameters["DeliverAfter"] = DeliverAfter;
	parameters["GroupName"] = GroupName;

	
	CallMethodAsync<String>("PushNotifications_WP_SendRawMessage", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void PushNotifications_WP_SendToastMessage(String BuddyApplicationName, String BuddyApplicationPassword, String UserTokenOrID, String ToastTitle, String ToastSubTitle, String ToastParameter, String DeliverAfter, String GroupName, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserTokenOrID"] = UserTokenOrID;
	parameters["ToastTitle"] = ToastTitle;
	parameters["ToastSubTitle"] = ToastSubTitle;
	parameters["ToastParameter"] = ToastParameter;
	parameters["DeliverAfter"] = DeliverAfter;
	parameters["GroupName"] = GroupName;

	
	CallMethodAsync<String>("PushNotifications_WP_SendToastMessage", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void PushNotifications_WP_GetRegisteredDevices(String BuddyApplicationName, String BuddyApplicationPassword, String GroupName, String PageSize, String CurrentPageNumber, Action<BuddyCallResult<InternalModels.DataContract_WPDeviceList[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["GroupName"] = GroupName;
	parameters["PageSize"] = PageSize;
	parameters["CurrentPageNumber"] = CurrentPageNumber;

	
	CallMethodAsync<InternalModels.DataContract_WPDeviceList[]>("PushNotifications_WP_GetRegisteredDevices", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void PushNotifications_WP_GetGroupNames(String BuddyApplicationName, String BuddyApplicationPassword, Action<BuddyCallResult<InternalModels.DataContract_WPGroupNames[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;

	
	CallMethodAsync<InternalModels.DataContract_WPGroupNames[]>("PushNotifications_WP_GetGroupNames", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void PushNotifications_Android_RegisterDevice(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String GroupName, String RegistrationID, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["GroupName"] = GroupName;
	parameters["RegistrationID"] = RegistrationID;

	
	CallMethodAsync<String>("PushNotifications_Android_RegisterDevice", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void PushNotifications_Android_RemoveDevice(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;

	
	CallMethodAsync<String>("PushNotifications_Android_RemoveDevice", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void PushNotifications_Android_SendRawMessage(String BuddyApplicationName, String BuddyApplicationPassword, String UserTokenOrID, String RawMessage, String DeliverAfter, String GroupName, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserTokenOrID"] = UserTokenOrID;
	parameters["RawMessage"] = RawMessage;
	parameters["DeliverAfter"] = DeliverAfter;
	parameters["GroupName"] = GroupName;

	
	CallMethodAsync<String>("PushNotifications_Android_SendRawMessage", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void PushNotifications_Android_GetRegisteredDevices(String BuddyApplicationName, String BuddyApplicationPassword, String GroupName, String PageSize, String CurrentPageNumber, Action<BuddyCallResult<InternalModels.DataContract_AndroidDeviceList[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["GroupName"] = GroupName;
	parameters["PageSize"] = PageSize;
	parameters["CurrentPageNumber"] = CurrentPageNumber;

	
	CallMethodAsync<InternalModels.DataContract_AndroidDeviceList[]>("PushNotifications_Android_GetRegisteredDevices", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void PushNotifications_Android_GetGroupNames(String BuddyApplicationName, String BuddyApplicationPassword, Action<BuddyCallResult<InternalModels.DataContract_AndroidGroupNames[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;

	
	CallMethodAsync<InternalModels.DataContract_AndroidGroupNames[]>("PushNotifications_Android_GetGroupNames", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void PushNotifications_Apple_RegisterApplicationCertificate(String BuddyApplicationName, String BuddyApplicationPassword, Stream P12CertificateBytes, String CertificatePassword, String ProductionBit, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["P12CertificateBytes"] = new BuddyFile(){Name="P12CertificateBytes", Data=P12CertificateBytes};	parameters["CertificatePassword"] = CertificatePassword;
	parameters["ProductionBit"] = ProductionBit;

	
	CallMethodAsync<String>("PushNotifications_Apple_RegisterApplicationCertificate", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void PushNotifications_Apple_RegisterDevice(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String GroupName, String AppleDeviceToken, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["GroupName"] = GroupName;
	parameters["AppleDeviceToken"] = AppleDeviceToken;

	
	CallMethodAsync<String>("PushNotifications_Apple_RegisterDevice", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void PushNotifications_Apple_GetGroupNames(String BuddyApplicationName, String BuddyApplicationPassword, Action<BuddyCallResult<InternalModels.DataContract_AppleGroupNames[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;

	
	CallMethodAsync<InternalModels.DataContract_AppleGroupNames[]>("PushNotifications_Apple_GetGroupNames", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void PushNotifications_Apple_GetRegisteredDevices(String BuddyApplicationName, String BuddyApplicationPassword, String GroupName, String PageSize, String CurrentPageNumber, Action<BuddyCallResult<InternalModels.DataContract_AppleDeviceList[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["GroupName"] = GroupName;
	parameters["PageSize"] = PageSize;
	parameters["CurrentPageNumber"] = CurrentPageNumber;

	
	CallMethodAsync<InternalModels.DataContract_AppleDeviceList[]>("PushNotifications_Apple_GetRegisteredDevices", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void PushNotifications_Apple_RemoveDevice(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;

	
	CallMethodAsync<String>("PushNotifications_Apple_RemoveDevice", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void PushNotifications_Apple_SendRawMessage(String BuddyApplicationName, String BuddyApplicationPassword, String UserTokenOrID, String DeliverAfter, String GroupName, String AppleMessage, String AppleBadge, String AppleSound, String CustomItems, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserTokenOrID"] = UserTokenOrID;
	parameters["DeliverAfter"] = DeliverAfter;
	parameters["GroupName"] = GroupName;
	parameters["AppleMessage"] = AppleMessage;
	parameters["AppleBadge"] = AppleBadge;
	parameters["AppleSound"] = AppleSound;
	parameters["CustomItems"] = CustomItems;

	
	CallMethodAsync<String>("PushNotifications_Apple_SendRawMessage", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void PushNotifications_Win8_RegisterDevice(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String DeviceURI, String ClientID, String ClientSecret, String GroupName, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["DeviceURI"] = DeviceURI;
	parameters["ClientID"] = ClientID;
	parameters["ClientSecret"] = ClientSecret;
	parameters["GroupName"] = GroupName;

	
	CallMethodAsync<String>("PushNotifications_Win8_RegisterDevice", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void PushNotifications_Win8_GetRegisteredDevices(String BuddyApplicationName, String BuddyApplicationPassword, String GroupName, String PageSize, String CurrentPageNumber, Action<BuddyCallResult<InternalModels.DataContract_Win8DeviceList[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["GroupName"] = GroupName;
	parameters["PageSize"] = PageSize;
	parameters["CurrentPageNumber"] = CurrentPageNumber;

	
	CallMethodAsync<InternalModels.DataContract_Win8DeviceList[]>("PushNotifications_Win8_GetRegisteredDevices", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void PushNotifications_Win8_GetGroupNames(String BuddyApplicationName, String BuddyApplicationPassword, Action<BuddyCallResult<InternalModels.DataContract_Win8GroupNames[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;

	
	CallMethodAsync<InternalModels.DataContract_Win8GroupNames[]>("PushNotifications_Win8_GetGroupNames", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void PushNotifications_Win8_RemoveDevice(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;

	
	CallMethodAsync<String>("PushNotifications_Win8_RemoveDevice", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void PushNotifications_Win8_SendToast(String BuddyApplicationName, String BuddyApplicationPassword, String UserTokenOrID, String XMLMessagePayload, String DeliverAfter, String GroupName, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserTokenOrID"] = UserTokenOrID;
	parameters["XMLMessagePayload"] = XMLMessagePayload;
	parameters["DeliverAfter"] = DeliverAfter;
	parameters["GroupName"] = GroupName;

	
	CallMethodAsync<String>("PushNotifications_Win8_SendToast", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void PushNotifications_Win8_SendBadge(String BuddyApplicationName, String BuddyApplicationPassword, String UserTokenOrID, String XMLMessagePayload, String DeliverAfter, String GroupName, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserTokenOrID"] = UserTokenOrID;
	parameters["XMLMessagePayload"] = XMLMessagePayload;
	parameters["DeliverAfter"] = DeliverAfter;
	parameters["GroupName"] = GroupName;

	
	CallMethodAsync<String>("PushNotifications_Win8_SendBadge", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void PushNotifications_Win8_SendLiveTile(String BuddyApplicationName, String BuddyApplicationPassword, String UserTokenOrID, String XMLMessagePayload, String DeliverAfter, String GroupName, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserTokenOrID"] = UserTokenOrID;
	parameters["XMLMessagePayload"] = XMLMessagePayload;
	parameters["DeliverAfter"] = DeliverAfter;
	parameters["GroupName"] = GroupName;

	
	CallMethodAsync<String>("PushNotifications_Win8_SendLiveTile", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Application_Users_GetEmailList(String BuddyApplicationName, String BuddyApplicationPassword, String FirstRow, String LastRow, Action<BuddyCallResult<InternalModels.DataContract_ApplicationEmail[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["FirstRow"] = FirstRow;
	parameters["LastRow"] = LastRow;

	
	CallMethodAsync<InternalModels.DataContract_ApplicationEmail[]>("Application_Users_GetEmailList", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Application_Users_GetProfileList(String BuddyApplicationName, String BuddyApplicationPassword, String FirstRow, String LastRow, Action<BuddyCallResult<InternalModels.DataContract_ApplicationUserProfile[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["FirstRow"] = FirstRow;
	parameters["LastRow"] = LastRow;

	
	CallMethodAsync<InternalModels.DataContract_ApplicationUserProfile[]>("Application_Users_GetProfileList", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Application_Metrics_GetStats(String BuddyApplicationName, String BuddyApplicationPassword, Action<BuddyCallResult<InternalModels.DataContract_ApplicationStats[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;

	
	CallMethodAsync<InternalModels.DataContract_ApplicationStats[]>("Application_Metrics_GetStats", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void GroupMessages_Membership_JoinGroup(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String GroupChatID, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["GroupChatID"] = GroupChatID;

	
	CallMethodAsync<String>("GroupMessages_Membership_JoinGroup", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void GroupMessages_Message_Get(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String GroupChatID, String FromDateTime, Action<BuddyCallResult<InternalModels.DataContract_GroupMessage[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["GroupChatID"] = GroupChatID;
	parameters["FromDateTime"] = FromDateTime;

	
	CallMethodAsync<InternalModels.DataContract_GroupMessage[]>("GroupMessages_Message_Get", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void GroupMessages_Membership_GetAllGroups(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, Action<BuddyCallResult<InternalModels.DataContract_GroupChatMemberships[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;

	
	CallMethodAsync<InternalModels.DataContract_GroupChatMemberships[]>("GroupMessages_Membership_GetAllGroups", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void GeoLocation_Location_Edit(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String ExistingGeoRecordID, String NewLatitude, String NewLongitude, String NewLocationName, String NewAddress, String NewLocationCity, String NewLocationState, String NewLocationZipPostal, String NewCategoryID, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["ExistingGeoRecordID"] = ExistingGeoRecordID;
	parameters["NewLatitude"] = NewLatitude;
	parameters["NewLongitude"] = NewLongitude;
	parameters["NewLocationName"] = NewLocationName;
	parameters["NewAddress"] = NewAddress;
	parameters["NewLocationCity"] = NewLocationCity;
	parameters["NewLocationState"] = NewLocationState;
	parameters["NewLocationZipPostal"] = NewLocationZipPostal;
	parameters["NewCategoryID"] = NewCategoryID;

	
	CallMethodAsync<String>("GeoLocation_Location_Edit", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void GeoLocation_Location_Flag(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String ExistingGeoRecordID, String FlagReason, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["ExistingGeoRecordID"] = ExistingGeoRecordID;
	parameters["FlagReason"] = FlagReason;

	
	CallMethodAsync<String>("GeoLocation_Location_Flag", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void GeoLocation_Category_GetList(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, Action<BuddyCallResult<InternalModels.DataContract_PlacesCategoryList[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;

	
	CallMethodAsync<InternalModels.DataContract_PlacesCategoryList[]>("GeoLocation_Category_GetList", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void GeoLocation_Location_Add(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String Latitude, String Longitude, String LocationName, String LocationAddress, String LocationCity, String LocationState, String LocationZipPostal, String LocationRegion, String LocationPhone, String LocationFax, String LocationWebsite, String CategoryID, String ApplicationTag, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["Latitude"] = Latitude;
	parameters["Longitude"] = Longitude;
	parameters["LocationName"] = LocationName;
	parameters["LocationAddress"] = LocationAddress;
	parameters["LocationCity"] = LocationCity;
	parameters["LocationState"] = LocationState;
	parameters["LocationZipPostal"] = LocationZipPostal;
	parameters["LocationRegion"] = LocationRegion;
	parameters["LocationPhone"] = LocationPhone;
	parameters["LocationFax"] = LocationFax;
	parameters["LocationWebsite"] = LocationWebsite;
	parameters["CategoryID"] = CategoryID;
	parameters["ApplicationTag"] = ApplicationTag;

	
	CallMethodAsync<String>("GeoLocation_Location_Add", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void GeoLocation_Location_Search(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String SearchDistance, String Latitude, String Longitude, String RecordLimit, String SearchName, String SearchCategoryID, Action<BuddyCallResult<InternalModels.DataContract_SearchPlaces[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["SearchDistance"] = SearchDistance;
	parameters["Latitude"] = Latitude;
	parameters["Longitude"] = Longitude;
	parameters["RecordLimit"] = RecordLimit;
	parameters["SearchName"] = SearchName;
	parameters["SearchCategoryID"] = SearchCategoryID;

	
	CallMethodAsync<InternalModels.DataContract_SearchPlaces[]>("GeoLocation_Location_Search", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void GeoLocation_Location_CustomSearch(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String SearchDistance, String Latitude, String Longitude, String RecordLimit, String SearchName, String SearchCategoryID, String CustomSearchCommand, Action<BuddyCallResult<InternalModels.DataContract_SearchPlaces[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["SearchDistance"] = SearchDistance;
	parameters["Latitude"] = Latitude;
	parameters["Longitude"] = Longitude;
	parameters["RecordLimit"] = RecordLimit;
	parameters["SearchName"] = SearchName;
	parameters["SearchCategoryID"] = SearchCategoryID;
	parameters["CustomSearchCommand"] = CustomSearchCommand;

	
	CallMethodAsync<InternalModels.DataContract_SearchPlaces[]>("GeoLocation_Location_CustomSearch", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void GeoLocation_Category_Submit(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String NewCategoryName, String Comment, String ApplicationTag, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["NewCategoryName"] = NewCategoryName;
	parameters["Comment"] = Comment;
	parameters["ApplicationTag"] = ApplicationTag;

	
	CallMethodAsync<String>("GeoLocation_Category_Submit", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void GeoLocation_Location_SetTag(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String ExistingGeoID, String ApplicationTag, String UserTag, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["ExistingGeoID"] = ExistingGeoID;
	parameters["ApplicationTag"] = ApplicationTag;
	parameters["UserTag"] = UserTag;

	
	CallMethodAsync<String>("GeoLocation_Location_SetTag", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void GeoLocation_Location_GetFromID(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String ExistingGeoID, String Latitude, String Longitude, Action<BuddyCallResult<InternalModels.DataContract_SearchPlaces[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["ExistingGeoID"] = ExistingGeoID;
	parameters["Latitude"] = Latitude;
	parameters["Longitude"] = Longitude;

	
	CallMethodAsync<InternalModels.DataContract_SearchPlaces[]>("GeoLocation_Location_GetFromID", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void StartupData_Location_Search(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String SearchDistance, String Latitude, String Longitude, String RecordLimit, String SearchName, Action<BuddyCallResult<InternalModels.DataContract_SearchStartups[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["SearchDistance"] = SearchDistance;
	parameters["Latitude"] = Latitude;
	parameters["Longitude"] = Longitude;
	parameters["RecordLimit"] = RecordLimit;
	parameters["SearchName"] = SearchName;

	
	CallMethodAsync<InternalModels.DataContract_SearchStartups[]>("StartupData_Location_Search", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void StartupData_Location_GetMetroList(String BuddyApplicationName, String BuddyApplicationPassword, Action<BuddyCallResult<InternalModels.DataContract_MetroList[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;

	
	CallMethodAsync<InternalModels.DataContract_MetroList[]>("StartupData_Location_GetMetroList", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void StartupData_Location_GetFromMetroArea(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String MetroName, String RecordLimit, Action<BuddyCallResult<InternalModels.DataContract_SearchStartups[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["MetroName"] = MetroName;
	parameters["RecordLimit"] = RecordLimit;

	
	CallMethodAsync<InternalModels.DataContract_SearchStartups[]>("StartupData_Location_GetFromMetroArea", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Friends_FriendRequest_Deny(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String FriendProfileID, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["FriendProfileID"] = FriendProfileID;

	
	CallMethodAsync<String>("Friends_FriendRequest_Deny", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Friends_FriendRequest_Add(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String FriendProfileID, String ApplicationTag, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["FriendProfileID"] = FriendProfileID;
	parameters["ApplicationTag"] = ApplicationTag;

	
	CallMethodAsync<String>("Friends_FriendRequest_Add", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Friends_FriendRequest_Accept(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String FriendProfileID, String ApplicationTag, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["FriendProfileID"] = FriendProfileID;
	parameters["ApplicationTag"] = ApplicationTag;

	
	CallMethodAsync<String>("Friends_FriendRequest_Accept", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Friends_FriendRequest_Get(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String FromDateTime, Action<BuddyCallResult<InternalModels.DataContract_FriendRequests[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["FromDateTime"] = FromDateTime;

	
	CallMethodAsync<InternalModels.DataContract_FriendRequests[]>("Friends_FriendRequest_Get", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Friends_Friends_GetList(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String FromDateTime, Action<BuddyCallResult<InternalModels.DataContract_FriendList[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["FromDateTime"] = FromDateTime;

	
	CallMethodAsync<InternalModels.DataContract_FriendList[]>("Friends_Friends_GetList", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Friends_Friends_Search(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String TimeFilter, String SearchDistance, String Latitude, String Longitude, String PageSize, String PageNumber, Action<BuddyCallResult<InternalModels.DataContract_SearchFriends[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["TimeFilter"] = TimeFilter;
	parameters["SearchDistance"] = SearchDistance;
	parameters["Latitude"] = Latitude;
	parameters["Longitude"] = Longitude;
	parameters["PageSize"] = PageSize;
	parameters["PageNumber"] = PageNumber;

	
	CallMethodAsync<InternalModels.DataContract_SearchFriends[]>("Friends_Friends_Search", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Friends_Friends_Remove(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String FriendProfileID, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["FriendProfileID"] = FriendProfileID;

	
	CallMethodAsync<String>("Friends_Friends_Remove", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Friends_Block_Remove(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String BlockedProfileID, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["BlockedProfileID"] = BlockedProfileID;

	
	CallMethodAsync<String>("Friends_Block_Remove", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Friends_Block_Add(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String UserProfileToBlock, String ApplicationTag, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["UserProfileToBlock"] = UserProfileToBlock;
	parameters["ApplicationTag"] = ApplicationTag;

	
	CallMethodAsync<String>("Friends_Block_Add", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Friends_Block_GetList(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, Action<BuddyCallResult<InternalModels.DataContract_BlockedList[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;

	
	CallMethodAsync<InternalModels.DataContract_BlockedList[]>("Friends_Block_GetList", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Friends_FriendRequest_GetSentRequests(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String FromDateTime, Action<BuddyCallResult<InternalModels.DataContract_SentFriendRequests[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["FromDateTime"] = FromDateTime;

	
	CallMethodAsync<InternalModels.DataContract_SentFriendRequests[]>("Friends_FriendRequest_GetSentRequests", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Analytics_DeviceInformation_Add(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String DeviceOSVersion, String DeviceType, String Latitude, String Longitude, String AppName, String AppVersion, String MetaData, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["DeviceOSVersion"] = DeviceOSVersion;
	parameters["DeviceType"] = DeviceType;
	parameters["Latitude"] = Latitude;
	parameters["Longitude"] = Longitude;
	parameters["AppName"] = AppName;
	parameters["AppVersion"] = AppVersion;
	parameters["MetaData"] = MetaData;

	
	CallMethodAsync<String>("Analytics_DeviceInformation_Add", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Analytics_CrashRecords_Add(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String AppVersion, String DeviceOSVersion, String DeviceType, String MethodName, String StackTrace, String MetaData, String Latitude, String Longitude, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["AppVersion"] = AppVersion;
	parameters["DeviceOSVersion"] = DeviceOSVersion;
	parameters["DeviceType"] = DeviceType;
	parameters["MethodName"] = MethodName;
	parameters["StackTrace"] = StackTrace;
	parameters["MetaData"] = MetaData;
	parameters["Latitude"] = Latitude;
	parameters["Longitude"] = Longitude;

	
	CallMethodAsync<String>("Analytics_CrashRecords_Add", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Analytics_Session_Start(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String SessionName, String StartAppTag, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["SessionName"] = SessionName;
	parameters["StartAppTag"] = StartAppTag;

	
	CallMethodAsync<String>("Analytics_Session_Start", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Analytics_Session_End(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String SessionID, String EndAppTag, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["SessionID"] = SessionID;
	parameters["EndAppTag"] = EndAppTag;

	
	CallMethodAsync<String>("Analytics_Session_End", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Analytics_Session_RecordMetric(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String SessionID, String MetricKey, String MetricValue, String AppTag, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["SessionID"] = SessionID;
	parameters["MetricKey"] = MetricKey;
	parameters["MetricValue"] = MetricValue;
	parameters["AppTag"] = AppTag;

	
	CallMethodAsync<String>("Analytics_Session_RecordMetric", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Service_DateTime_Get(String BuddyApplicationName, String BuddyApplicationPassword, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;

	
	CallMethodAsync<String>("Service_DateTime_Get", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Service_Version_Get(String BuddyApplicationName, String BuddyApplicationPassword, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;

	
	CallMethodAsync<String>("Service_Version_Get", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Service_Ping_Get(String BuddyApplicationName, String BuddyApplicationPassword, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;

	
	CallMethodAsync<String>("Service_Ping_Get", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void MetaData_ApplicationMetaDataValue_Set(String BuddyApplicationName, String BuddyApplicationPassword, String SocketMetaKey, String SocketMetaValue, String MetaLatitude, String MetaLongitude, String ApplicationTag, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["SocketMetaKey"] = SocketMetaKey;
	parameters["SocketMetaValue"] = SocketMetaValue;
	parameters["MetaLatitude"] = MetaLatitude;
	parameters["MetaLongitude"] = MetaLongitude;
	parameters["ApplicationTag"] = ApplicationTag;

	
	CallMethodAsync<String>("MetaData_ApplicationMetaDataValue_Set", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void MetaData_ApplicationMetaDataValue_Get(String BuddyApplicationName, String BuddyApplicationPassword, String SocketMetaKey, Action<BuddyCallResult<InternalModels.DataContract_ApplicationMetaData[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["SocketMetaKey"] = SocketMetaKey;

	
	CallMethodAsync<InternalModels.DataContract_ApplicationMetaData[]>("MetaData_ApplicationMetaDataValue_Get", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void MetaData_ApplicationMetaDataValue_GetAll(String BuddyApplicationName, String BuddyApplicationPassword, Action<BuddyCallResult<InternalModels.DataContract_ApplicationMetaData[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;

	
	CallMethodAsync<InternalModels.DataContract_ApplicationMetaData[]>("MetaData_ApplicationMetaDataValue_GetAll", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void MetaData_ApplicationMetaDataValue_Delete(String BuddyApplicationName, String BuddyApplicationPassword, String SocketMetaKey, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["SocketMetaKey"] = SocketMetaKey;

	
	CallMethodAsync<String>("MetaData_ApplicationMetaDataValue_Delete", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void MetaData_ApplicationMetaDataCounter_Increment(String BuddyApplicationName, String BuddyApplicationPassword, String SocketMetaKey, String IncrementValueAmount, String MetaLatitude, String MetaLongitude, String ApplicationTag, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["SocketMetaKey"] = SocketMetaKey;
	parameters["IncrementValueAmount"] = IncrementValueAmount;
	parameters["MetaLatitude"] = MetaLatitude;
	parameters["MetaLongitude"] = MetaLongitude;
	parameters["ApplicationTag"] = ApplicationTag;

	
	CallMethodAsync<String>("MetaData_ApplicationMetaDataCounter_Increment", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void MetaData_ApplicationMetaDataCounter_Decrement(String BuddyApplicationName, String BuddyApplicationPassword, String SocketMetaKey, String DecrementValueAmount, String MetaLatitude, String MetaLongitude, String ApplicationTag, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["SocketMetaKey"] = SocketMetaKey;
	parameters["DecrementValueAmount"] = DecrementValueAmount;
	parameters["MetaLatitude"] = MetaLatitude;
	parameters["MetaLongitude"] = MetaLongitude;
	parameters["ApplicationTag"] = ApplicationTag;

	
	CallMethodAsync<String>("MetaData_ApplicationMetaDataCounter_Decrement", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void MetaData_ApplicationMetaDataValue_Sum(String BuddyApplicationName, String BuddyApplicationPassword, String SocketMetaKey, String SearchDistance, String Latitude, String Longitude, String TimeFilter, String ApplicationTag, Action<BuddyCallResult<InternalModels.DataContract_MetaDataSum[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["SocketMetaKey"] = SocketMetaKey;
	parameters["SearchDistance"] = SearchDistance;
	parameters["Latitude"] = Latitude;
	parameters["Longitude"] = Longitude;
	parameters["TimeFilter"] = TimeFilter;
	parameters["ApplicationTag"] = ApplicationTag;

	
	CallMethodAsync<InternalModels.DataContract_MetaDataSum[]>("MetaData_ApplicationMetaDataValue_Sum", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void MetaData_ApplicationMetaDataValue_DeleteAll(String BuddyApplicationName, String BuddyApplicationPassword, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;

	
	CallMethodAsync<String>("MetaData_ApplicationMetaDataValue_DeleteAll", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Pictures_PhotoAlbum_GetByDateTime(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String UserProfileID, String StartDateTime, Action<BuddyCallResult<InternalModels.DataContract_PhotoList[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["UserProfileID"] = UserProfileID;
	parameters["StartDateTime"] = StartDateTime;

	
	CallMethodAsync<InternalModels.DataContract_PhotoList[]>("Pictures_PhotoAlbum_GetByDateTime", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Pictures_PhotoAlbum_GetList(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String UserProfileID, Action<BuddyCallResult<InternalModels.DataContract_PhotoAlbumList[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["UserProfileID"] = UserProfileID;

	
	CallMethodAsync<InternalModels.DataContract_PhotoAlbumList[]>("Pictures_PhotoAlbum_GetList", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Pictures_PhotoAlbum_Get(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String UserProfileID, String PhotoAlbumID, Action<BuddyCallResult<InternalModels.DataContract_PhotoList[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["UserProfileID"] = UserProfileID;
	parameters["PhotoAlbumID"] = PhotoAlbumID;

	
	CallMethodAsync<InternalModels.DataContract_PhotoList[]>("Pictures_PhotoAlbum_Get", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Pictures_PhotoAlbum_Delete(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String PhotoAlbumID, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["PhotoAlbumID"] = PhotoAlbumID;

	
	CallMethodAsync<String>("Pictures_PhotoAlbum_Delete", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Pictures_PhotoAlbum_Create(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String AlbumName, String PublicAlbumBit, String ApplicationTag, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["AlbumName"] = AlbumName;
	parameters["PublicAlbumBit"] = PublicAlbumBit;
	parameters["ApplicationTag"] = ApplicationTag;

	
	CallMethodAsync<String>("Pictures_PhotoAlbum_Create", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Pictures_PhotoAlbum_GetAllPictures(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String UserProfileID, String SearchFromDateTime, Action<BuddyCallResult<InternalModels.DataContract_PhotoList[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["UserProfileID"] = UserProfileID;
	parameters["SearchFromDateTime"] = SearchFromDateTime;

	
	CallMethodAsync<InternalModels.DataContract_PhotoList[]>("Pictures_PhotoAlbum_GetAllPictures", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Pictures_PhotoAlbum_GetFromAlbumName(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String UserProfileID, String PhotoAlbumName, Action<BuddyCallResult<InternalModels.DataContract_PhotoAlbumList[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["UserProfileID"] = UserProfileID;
	parameters["PhotoAlbumName"] = PhotoAlbumName;

	
	CallMethodAsync<InternalModels.DataContract_PhotoAlbumList[]>("Pictures_PhotoAlbum_GetFromAlbumName", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Pictures_VirtualAlbum_Create(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String AlbumName, String ApplicationTag, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["AlbumName"] = AlbumName;
	parameters["ApplicationTag"] = ApplicationTag;

	
	CallMethodAsync<String>("Pictures_VirtualAlbum_Create", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Pictures_VirtualAlbum_AddPhoto(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String VirtualAlbumID, String ExistingPhotoID, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["VirtualAlbumID"] = VirtualAlbumID;
	parameters["ExistingPhotoID"] = ExistingPhotoID;

	
	CallMethodAsync<String>("Pictures_VirtualAlbum_AddPhoto", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Pictures_VirtualAlbum_Get(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String VirtualPhotoAlbumID, Action<BuddyCallResult<InternalModels.DataContract_VirtualPhotoList[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["VirtualPhotoAlbumID"] = VirtualPhotoAlbumID;

	
	CallMethodAsync<InternalModels.DataContract_VirtualPhotoList[]>("Pictures_VirtualAlbum_Get", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Pictures_VirtualAlbum_DeleteAlbum(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String VirtualAlbumID, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["VirtualAlbumID"] = VirtualAlbumID;

	
	CallMethodAsync<String>("Pictures_VirtualAlbum_DeleteAlbum", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Pictures_VirtualAlbum_AddPhotoBatch(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String VirtualAlbumID, String ExistingPhotoIDBatchString, Action<BuddyCallResult<InternalModels.DataContract_VirtualAlbumBatchAddResults[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["VirtualAlbumID"] = VirtualAlbumID;
	parameters["ExistingPhotoIDBatchString"] = ExistingPhotoIDBatchString;

	
	CallMethodAsync<InternalModels.DataContract_VirtualAlbumBatchAddResults[]>("Pictures_VirtualAlbum_AddPhotoBatch", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Pictures_VirtualAlbum_GetAlbumInformation(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String VirtualAlbumID, Action<BuddyCallResult<InternalModels.DataContract_VirtualPhotoAlbumInformation[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["VirtualAlbumID"] = VirtualAlbumID;

	
	CallMethodAsync<InternalModels.DataContract_VirtualPhotoAlbumInformation[]>("Pictures_VirtualAlbum_GetAlbumInformation", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Pictures_VirtualAlbum_GetMyAlbums(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, Action<BuddyCallResult<InternalModels.DataContract_VirtualPhotoAlbumInformation[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;

	
	CallMethodAsync<InternalModels.DataContract_VirtualPhotoAlbumInformation[]>("Pictures_VirtualAlbum_GetMyAlbums", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Pictures_VirtualAlbum_Update(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String VirtualAlbumID, String NewAlbumName, String NewAppTag, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["VirtualAlbumID"] = VirtualAlbumID;
	parameters["NewAlbumName"] = NewAlbumName;
	parameters["NewAppTag"] = NewAppTag;

	
	CallMethodAsync<String>("Pictures_VirtualAlbum_Update", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Pictures_VirtualAlbum_RemovePhoto(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String VirtualAlbumID, String ExistingPhotoID, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["VirtualAlbumID"] = VirtualAlbumID;
	parameters["ExistingPhotoID"] = ExistingPhotoID;

	
	CallMethodAsync<String>("Pictures_VirtualAlbum_RemovePhoto", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Pictures_VirtualAlbum_UpdatePhoto(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String ExistingPhotoID, String NewPhotoComment, String NewAppTag, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["ExistingPhotoID"] = ExistingPhotoID;
	parameters["NewPhotoComment"] = NewPhotoComment;
	parameters["NewAppTag"] = NewAppTag;

	
	CallMethodAsync<String>("Pictures_VirtualAlbum_UpdatePhoto", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Pictures_Photo_Get(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String UserProfileID, String PhotoID, Action<BuddyCallResult<InternalModels.DataContract_PhotoList[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["UserProfileID"] = UserProfileID;
	parameters["PhotoID"] = PhotoID;

	
	CallMethodAsync<InternalModels.DataContract_PhotoList[]>("Pictures_Photo_Get", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Pictures_Photo_Delete(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String PhotoAlbumPhotoID, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["PhotoAlbumPhotoID"] = PhotoAlbumPhotoID;

	
	CallMethodAsync<String>("Pictures_Photo_Delete", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Pictures_Photo_Add(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, Stream photoStream, String AlbumID, String PhotoComment, String Latitude, String Longitude, String ApplicationTag, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["bytesFullPhotoData"] = new BuddyFile(){Name="bytesFullPhotoData", Data=photoStream};	parameters["AlbumID"] = AlbumID;
	parameters["PhotoComment"] = PhotoComment;
	parameters["Latitude"] = Latitude;
	parameters["Longitude"] = Longitude;
	parameters["ApplicationTag"] = ApplicationTag;

	
	CallMethodAsync<String>("Pictures_Photo_Add", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Pictures_SearchPhotos_Nearby(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String SearchDistance, String Latitude, String Longitude, String RecordLimit, Action<BuddyCallResult<InternalModels.DataContract_PublicPhotoSearch[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["SearchDistance"] = SearchDistance;
	parameters["Latitude"] = Latitude;
	parameters["Longitude"] = Longitude;
	parameters["RecordLimit"] = RecordLimit;

	
	CallMethodAsync<InternalModels.DataContract_PublicPhotoSearch[]>("Pictures_SearchPhotos_Nearby", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Pictures_SearchPhotos_Data(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String Latitude, String Longitude, String RecordLimit, Action<BuddyCallResult<InternalModels.DataContract_PublicPhotoSearch[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["Latitude"] = Latitude;
	parameters["Longitude"] = Longitude;
	parameters["RecordLimit"] = RecordLimit;

	
	CallMethodAsync<InternalModels.DataContract_PublicPhotoSearch[]>("Pictures_SearchPhotos_Data", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Pictures_Photo_AddWithWatermark(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, Stream photoStream, String AlbumID, String PhotoComment, String Latitude, String Longitude, String ApplicationTag, String WatermarkMessage, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["bytesFullPhotoData"] = new BuddyFile(){Name="bytesFullPhotoData", Data=photoStream};	parameters["AlbumID"] = AlbumID;
	parameters["PhotoComment"] = PhotoComment;
	parameters["Latitude"] = Latitude;
	parameters["Longitude"] = Longitude;
	parameters["ApplicationTag"] = ApplicationTag;
	parameters["WatermarkMessage"] = WatermarkMessage;

	
	CallMethodAsync<String>("Pictures_Photo_AddWithWatermark", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Pictures_Photo_SetAppTag(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String PhotoAlbumPhotoID, String ApplicationTag, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["PhotoAlbumPhotoID"] = PhotoAlbumPhotoID;
	parameters["ApplicationTag"] = ApplicationTag;

	
	CallMethodAsync<String>("Pictures_Photo_SetAppTag", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Pictures_ProfilePhoto_GetAll(String BuddyApplicationName, String BuddyApplicationPassword, String UserProfileID, Action<BuddyCallResult<InternalModels.DataContract_ProfilePhotos[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserProfileID"] = UserProfileID;

	
	CallMethodAsync<InternalModels.DataContract_ProfilePhotos[]>("Pictures_ProfilePhoto_GetAll", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Pictures_ProfilePhoto_Delete(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String ProfilePhotoID, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["ProfilePhotoID"] = ProfilePhotoID;

	
	CallMethodAsync<String>("Pictures_ProfilePhoto_Delete", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Pictures_ProfilePhoto_Add(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, Stream photoStream, String ApplicationTag, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["bytesFullPhotoData"] = new BuddyFile(){Name="bytesFullPhotoData", Data=photoStream};	parameters["ApplicationTag"] = ApplicationTag;

	
	CallMethodAsync<String>("Pictures_ProfilePhoto_Add", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Pictures_ProfilePhoto_GetMyList(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, Action<BuddyCallResult<InternalModels.DataContract_ProfilePhotos[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;

	
	CallMethodAsync<InternalModels.DataContract_ProfilePhotos[]>("Pictures_ProfilePhoto_GetMyList", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Pictures_ProfilePhoto_Set(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String ProfilePhotoResource, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["ProfilePhotoResource"] = ProfilePhotoResource;

	
	CallMethodAsync<String>("Pictures_ProfilePhoto_Set", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Messages_Message_Send(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String MessageString, String ToUserID, String ApplicationTag, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["MessageString"] = MessageString;
	parameters["ToUserID"] = ToUserID;
	parameters["ApplicationTag"] = ApplicationTag;

	
	CallMethodAsync<String>("Messages_Message_Send", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Messages_Messages_Get(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String FromDateTime, Action<BuddyCallResult<InternalModels.DataContract_Messages[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["FromDateTime"] = FromDateTime;

	
	CallMethodAsync<InternalModels.DataContract_Messages[]>("Messages_Messages_Get", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Messages_SentMessages_Get(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String FromDateTime, Action<BuddyCallResult<InternalModels.DataContract_MessagesFromMe[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["FromDateTime"] = FromDateTime;

	
	CallMethodAsync<InternalModels.DataContract_MessagesFromMe[]>("Messages_SentMessages_Get", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void GroupMessages_Manage_CreateGroup(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String GroupName, String GroupSecurity, String ApplicationTag, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["GroupName"] = GroupName;
	parameters["GroupSecurity"] = GroupSecurity;
	parameters["ApplicationTag"] = ApplicationTag;

	
	CallMethodAsync<String>("GroupMessages_Manage_CreateGroup", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void GroupMessages_Manage_CheckForGroup(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String GroupName, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["GroupName"] = GroupName;

	
	CallMethodAsync<String>("GroupMessages_Manage_CheckForGroup", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void GroupMessages_Membership_AddNewMember(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String GroupChatID, String UserProfileIDToAdd, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["GroupChatID"] = GroupChatID;
	parameters["UserProfileIDToAdd"] = UserProfileIDToAdd;

	
	CallMethodAsync<String>("GroupMessages_Membership_AddNewMember", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void GroupMessages_Manage_DeleteGroup(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String GroupChatID, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["GroupChatID"] = GroupChatID;

	
	CallMethodAsync<String>("GroupMessages_Manage_DeleteGroup", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void GroupMessages_Membership_RemoveUser(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String UserProfileIDToRemove, String GroupChatID, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["UserProfileIDToRemove"] = UserProfileIDToRemove;
	parameters["GroupChatID"] = GroupChatID;

	
	CallMethodAsync<String>("GroupMessages_Membership_RemoveUser", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void GroupMessages_Membership_DepartGroup(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String GroupChatID, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["GroupChatID"] = GroupChatID;

	
	CallMethodAsync<String>("GroupMessages_Membership_DepartGroup", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void GroupMessages_Membership_GetMyList(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, Action<BuddyCallResult<InternalModels.DataContract_GroupChatMemberships[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;

	
	CallMethodAsync<InternalModels.DataContract_GroupChatMemberships[]>("GroupMessages_Membership_GetMyList", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void GroupMessages_Message_Send(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String GroupChatID, String MessageContent, String Latitude, String Longitude, String ApplicationTag, Action<BuddyCallResult<InternalModels.DataContract_SendGroupMessageResult[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["GroupChatID"] = GroupChatID;
	parameters["MessageContent"] = MessageContent;
	parameters["Latitude"] = Latitude;
	parameters["Longitude"] = Longitude;
	parameters["ApplicationTag"] = ApplicationTag;

	
	CallMethodAsync<InternalModels.DataContract_SendGroupMessageResult[]>("GroupMessages_Message_Send", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Commerce_Store_GetAllItems(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, Action<BuddyCallResult<InternalModels.DataContract_CommerceStoreGetItems[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;

	
	CallMethodAsync<InternalModels.DataContract_CommerceStoreGetItems[]>("Commerce_Store_GetAllItems", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Commerce_Store_GetActiveItems(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, Action<BuddyCallResult<InternalModels.DataContract_CommerceStoreGetItems[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;

	
	CallMethodAsync<InternalModels.DataContract_CommerceStoreGetItems[]>("Commerce_Store_GetActiveItems", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Commerce_Store_GetFreeItems(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, Action<BuddyCallResult<InternalModels.DataContract_CommerceStoreGetItems[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;

	
	CallMethodAsync<InternalModels.DataContract_CommerceStoreGetItems[]>("Commerce_Store_GetFreeItems", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Commerce_Receipt_GetForUserAndTransactionID(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String CustomTransactionID, Action<BuddyCallResult<InternalModels.DataContract_CommerceReceipt[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["CustomTransactionID"] = CustomTransactionID;

	
	CallMethodAsync<InternalModels.DataContract_CommerceReceipt[]>("Commerce_Receipt_GetForUserAndTransactionID", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Commerce_Receipt_Save(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String ReceiptData, String CustomTransactionID, String AppData, String TotalCost, String TotalQuantity, String StoreItemID, String StoreName, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["ReceiptData"] = ReceiptData;
	parameters["CustomTransactionID"] = CustomTransactionID;
	parameters["AppData"] = AppData;
	parameters["TotalCost"] = TotalCost;
	parameters["TotalQuantity"] = TotalQuantity;
	parameters["StoreItemID"] = StoreItemID;
	parameters["StoreName"] = StoreName;

	
	CallMethodAsync<String>("Commerce_Receipt_Save", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Commerce_Receipt_VerifyiOSReceipt(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String AppleItemID, String ReceiptData, String CustomTransactionID, String AppData, String TotalCost, String TotalQuantity, String UseSandbox, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["AppleItemID"] = AppleItemID;
	parameters["ReceiptData"] = ReceiptData;
	parameters["CustomTransactionID"] = CustomTransactionID;
	parameters["AppData"] = AppData;
	parameters["TotalCost"] = TotalCost;
	parameters["TotalQuantity"] = TotalQuantity;
	parameters["UseSandbox"] = UseSandbox;

	
	CallMethodAsync<String>("Commerce_Receipt_VerifyiOSReceipt", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Commerce_Receipt_VerifyAndSaveiOSReceipt(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String AppleItemID, String ReceiptData, String CustomTransactionID, String AppData, String TotalCost, String TotalQuantity, String UseSandbox, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["AppleItemID"] = AppleItemID;
	parameters["ReceiptData"] = ReceiptData;
	parameters["CustomTransactionID"] = CustomTransactionID;
	parameters["AppData"] = AppData;
	parameters["TotalCost"] = TotalCost;
	parameters["TotalQuantity"] = TotalQuantity;
	parameters["UseSandbox"] = UseSandbox;

	
	CallMethodAsync<String>("Commerce_Receipt_VerifyAndSaveiOSReceipt", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Commerce_Receipt_GetForUser(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String FromDateTime, Action<BuddyCallResult<InternalModels.DataContract_CommerceReceipt[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["FromDateTime"] = FromDateTime;

	
	CallMethodAsync<InternalModels.DataContract_CommerceReceipt[]>("Commerce_Receipt_GetForUser", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Game_Score_Add(String BuddyApplicationName, String BuddyApplicationPassword, String UserTokenOrID, String ScoreLatitude, String ScoreLongitude, String ScoreRank, String ScoreValue, String ScoreBoardName, String ApplicationTag, String OneScorePerPlayerBit, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserTokenOrID"] = UserTokenOrID;
	parameters["ScoreLatitude"] = ScoreLatitude;
	parameters["ScoreLongitude"] = ScoreLongitude;
	parameters["ScoreRank"] = ScoreRank;
	parameters["ScoreValue"] = ScoreValue;
	parameters["ScoreBoardName"] = ScoreBoardName;
	parameters["ApplicationTag"] = ApplicationTag;
	parameters["OneScorePerPlayerBit"] = OneScorePerPlayerBit;

	
	CallMethodAsync<String>("Game_Score_Add", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Game_Score_GetScoresForUser(String BuddyApplicationName, String BuddyApplicationPassword, String UserTokenOrID, String RecordLimit, Action<BuddyCallResult<InternalModels.DataContract_GameUserScoreList[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserTokenOrID"] = UserTokenOrID;
	parameters["RecordLimit"] = RecordLimit;

	
	CallMethodAsync<InternalModels.DataContract_GameUserScoreList[]>("Game_Score_GetScoresForUser", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Game_Score_GetBoardHighScores(String BuddyApplicationName, String BuddyApplicationPassword, String ScoreBoardName, String RecordLimit, Action<BuddyCallResult<InternalModels.DataContract_GameBoardScoreList[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["ScoreBoardName"] = ScoreBoardName;
	parameters["RecordLimit"] = RecordLimit;

	
	CallMethodAsync<InternalModels.DataContract_GameBoardScoreList[]>("Game_Score_GetBoardHighScores", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Game_Score_GetBoardLowScores(String BuddyApplicationName, String BuddyApplicationPassword, String ScoreBoardName, String RecordLimit, Action<BuddyCallResult<InternalModels.DataContract_GameBoardScoreList[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["ScoreBoardName"] = ScoreBoardName;
	parameters["RecordLimit"] = RecordLimit;

	
	CallMethodAsync<InternalModels.DataContract_GameBoardScoreList[]>("Game_Score_GetBoardLowScores", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Game_Score_DeleteAllScoresForUser(String BuddyApplicationName, String BuddyApplicationPassword, String UserTokenOrID, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserTokenOrID"] = UserTokenOrID;

	
	CallMethodAsync<String>("Game_Score_DeleteAllScoresForUser", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Game_Score_SearchScores(String BuddyApplicationName, String BuddyApplicationPassword, String UserTokenOrID, String SearchDistance, String SearchLatitude, String SearchLongitude, String RecordLimit, String SearchBoard, String TimeFilter, String MinimumScore, String AppTag, Action<BuddyCallResult<InternalModels.DataContract_GameScoreSearchResults[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserTokenOrID"] = UserTokenOrID;
	parameters["SearchDistance"] = SearchDistance;
	parameters["SearchLatitude"] = SearchLatitude;
	parameters["SearchLongitude"] = SearchLongitude;
	parameters["RecordLimit"] = RecordLimit;
	parameters["SearchBoard"] = SearchBoard;
	parameters["TimeFilter"] = TimeFilter;
	parameters["MinimumScore"] = MinimumScore;
	parameters["AppTag"] = AppTag;

	
	CallMethodAsync<InternalModels.DataContract_GameScoreSearchResults[]>("Game_Score_SearchScores", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Game_Player_Add(String BuddyApplicationName, String BuddyApplicationPassword, String UserTokenOrID, String PlayerName, String PlayerLatitude, String PlayerLongitude, String PlayerRank, String PlayerBoardName, String ApplicationTag, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserTokenOrID"] = UserTokenOrID;
	parameters["PlayerName"] = PlayerName;
	parameters["PlayerLatitude"] = PlayerLatitude;
	parameters["PlayerLongitude"] = PlayerLongitude;
	parameters["PlayerRank"] = PlayerRank;
	parameters["PlayerBoardName"] = PlayerBoardName;
	parameters["ApplicationTag"] = ApplicationTag;

	
	CallMethodAsync<String>("Game_Player_Add", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Game_Player_Update(String BuddyApplicationName, String BuddyApplicationPassword, String UserTokenOrID, String PlayerName, String PlayerLatitude, String PlayerLongitude, String PlayerRank, String PlayerBoardName, String ApplicationTag, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserTokenOrID"] = UserTokenOrID;
	parameters["PlayerName"] = PlayerName;
	parameters["PlayerLatitude"] = PlayerLatitude;
	parameters["PlayerLongitude"] = PlayerLongitude;
	parameters["PlayerRank"] = PlayerRank;
	parameters["PlayerBoardName"] = PlayerBoardName;
	parameters["ApplicationTag"] = ApplicationTag;

	
	CallMethodAsync<String>("Game_Player_Update", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Game_Player_Delete(String BuddyApplicationName, String BuddyApplicationPassword, String UserTokenOrID, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserTokenOrID"] = UserTokenOrID;

	
	CallMethodAsync<String>("Game_Player_Delete", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Game_Player_SearchPlayers(String BuddyApplicationName, String BuddyApplicationPassword, String UserTokenOrID, String RecordLimit, String TimeFilter, String ApplicationTag, String PlayerRank, String BoardName, String SearchDistance, String SearchLatitude, String SearchLongitude, Action<BuddyCallResult<InternalModels.DataContract_GamePlayerSearchResults[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserTokenOrID"] = UserTokenOrID;
	parameters["RecordLimit"] = RecordLimit;
	parameters["TimeFilter"] = TimeFilter;
	parameters["ApplicationTag"] = ApplicationTag;
	parameters["PlayerRank"] = PlayerRank;
	parameters["BoardName"] = BoardName;
	parameters["SearchDistance"] = SearchDistance;
	parameters["SearchLatitude"] = SearchLatitude;
	parameters["SearchLongitude"] = SearchLongitude;

	
	CallMethodAsync<InternalModels.DataContract_GamePlayerSearchResults[]>("Game_Player_SearchPlayers", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Game_Player_GetPlayerInfo(String BuddyApplicationName, String BuddyApplicationPassword, String UserTokenOrID, Action<BuddyCallResult<InternalModels.DataContract_GamePlayerInfo[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserTokenOrID"] = UserTokenOrID;

	
	CallMethodAsync<InternalModels.DataContract_GamePlayerInfo[]>("Game_Player_GetPlayerInfo", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Game_State_Add(String BuddyApplicationName, String BuddyApplicationPassword, String UserTokenOrID, String GameStateKey, String GameStateValue, String ApplicationTag, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserTokenOrID"] = UserTokenOrID;
	parameters["GameStateKey"] = GameStateKey;
	parameters["GameStateValue"] = GameStateValue;
	parameters["ApplicationTag"] = ApplicationTag;

	
	CallMethodAsync<String>("Game_State_Add", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Game_State_Update(String BuddyApplicationName, String BuddyApplicationPassword, String UserTokenOrID, String GameStateKey, String GameStateValue, String ApplicationTag, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserTokenOrID"] = UserTokenOrID;
	parameters["GameStateKey"] = GameStateKey;
	parameters["GameStateValue"] = GameStateValue;
	parameters["ApplicationTag"] = ApplicationTag;

	
	CallMethodAsync<String>("Game_State_Update", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Game_State_Remove(String BuddyApplicationName, String BuddyApplicationPassword, String UserTokenOrID, String GameStateKey, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserTokenOrID"] = UserTokenOrID;
	parameters["GameStateKey"] = GameStateKey;

	
	CallMethodAsync<String>("Game_State_Remove", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Game_State_Get(String BuddyApplicationName, String BuddyApplicationPassword, String UserTokenOrID, String GameStateKey, Action<BuddyCallResult<InternalModels.DataContract_GameStateObject[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserTokenOrID"] = UserTokenOrID;
	parameters["GameStateKey"] = GameStateKey;

	
	CallMethodAsync<InternalModels.DataContract_GameStateObject[]>("Game_State_Get", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void Game_State_GetAll(String BuddyApplicationName, String BuddyApplicationPassword, String UserTokenOrID, Action<BuddyCallResult<InternalModels.DataContract_GameStateObject[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserTokenOrID"] = UserTokenOrID;

	
	CallMethodAsync<InternalModels.DataContract_GameStateObject[]>("Game_State_GetAll", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void UserAccount_Profile_GetFromUserToken(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, Action<BuddyCallResult<InternalModels.DataContract_FullUserProfile[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;

	
	CallMethodAsync<InternalModels.DataContract_FullUserProfile[]>("UserAccount_Profile_GetFromUserToken", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void UserAccount_Profile_GetFromUserID(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String UserIDToFetch, Action<BuddyCallResult<InternalModels.DataContract_PublicUserProfile[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["UserIDToFetch"] = UserIDToFetch;

	
	CallMethodAsync<InternalModels.DataContract_PublicUserProfile[]>("UserAccount_Profile_GetFromUserID", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void UserAccount_Profile_Create(String BuddyApplicationName, String BuddyApplicationPassword, String NewUserName, String UserSuppliedPassword, String NewUserGender, Int32 UserAge, String NewUserEmail, Int32 StatusID, Int32 LocationFuzzEnabledBit, Int32 CelebModeEnabledBit, String ApplicationTag, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["NewUserName"] = NewUserName;
	parameters["UserSuppliedPassword"] = UserSuppliedPassword;
	parameters["NewUserGender"] = NewUserGender;
	parameters["UserAge"] = UserAge;
	parameters["NewUserEmail"] = NewUserEmail;
	parameters["StatusID"] = StatusID;
	parameters["LocationFuzzEnabledBit"] = LocationFuzzEnabledBit;
	parameters["CelebModeEnabledBit"] = CelebModeEnabledBit;
	parameters["ApplicationTag"] = ApplicationTag;

	
	CallMethodAsync<String>("UserAccount_Profile_Create", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void UserAccount_Profile_Update(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String UserName, String UserSuppliedPassword, String UserGender, Int32 UserAge, String UserEmail, Int32 StatusID, Int32 LocationFuzzEnabledBit, Int32 CelebModeEnabledBit, String ApplicationTag, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["UserName"] = UserName;
	parameters["UserSuppliedPassword"] = UserSuppliedPassword;
	parameters["UserGender"] = UserGender;
	parameters["UserAge"] = UserAge;
	parameters["UserEmail"] = UserEmail;
	parameters["StatusID"] = StatusID;
	parameters["LocationFuzzEnabledBit"] = LocationFuzzEnabledBit;
	parameters["CelebModeEnabledBit"] = CelebModeEnabledBit;
	parameters["ApplicationTag"] = ApplicationTag;

	
	CallMethodAsync<String>("UserAccount_Profile_Update", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void UserAccount_Profile_Recover(String BuddyApplicationName, String BuddyApplicationPassword, String UserName, String UserSuppliedPassword, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["username"] = UserName;
	parameters["UserSuppliedPassword"] = UserSuppliedPassword;

	
	CallMethodAsync<String>("UserAccount_Profile_Recover", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void UserAccount_Profile_Search(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String SearchDistance, String Latitude, String Longitude, String RecordLimit, String Gender, String AgeStart, String AgeStop, String StatusID, String TimeFilter, String ApplicationTag, Action<BuddyCallResult<InternalModels.DataContract_SearchPeople[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["SearchDistance"] = SearchDistance;
	parameters["Latitude"] = Latitude;
	parameters["Longitude"] = Longitude;
	parameters["RecordLimit"] = RecordLimit;
	parameters["Gender"] = Gender;
	parameters["AgeStart"] = AgeStart;
	parameters["AgeStop"] = AgeStop;
	parameters["StatusID"] = StatusID;
	parameters["TimeFilter"] = TimeFilter;
	parameters["ApplicationTag"] = ApplicationTag;

	
	CallMethodAsync<InternalModels.DataContract_SearchPeople[]>("UserAccount_Profile_Search", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void UserAccount_Location_Checkin(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String Latitude, String Longitude, String CheckinComment, String ApplicationTag, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["Latitude"] = Latitude;
	parameters["Longitude"] = Longitude;
	parameters["CheckinComment"] = CheckinComment;
	parameters["ApplicationTag"] = ApplicationTag;

	
	CallMethodAsync<String>("UserAccount_Location_Checkin", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void UserAccount_Location_GetHistory(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String FromDateTime, Action<BuddyCallResult<InternalModels.DataContract_UserLocationHistory[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["FromDateTime"] = FromDateTime;

	
	CallMethodAsync<InternalModels.DataContract_UserLocationHistory[]>("UserAccount_Location_GetHistory", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void UserAccount_Defines_GetStatusValues(String BuddyApplicationName, String BuddyApplicationPassword, Action<BuddyCallResult<InternalModels.DataContract_DefinedUserStatusTags[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;

	
	CallMethodAsync<InternalModels.DataContract_DefinedUserStatusTags[]>("UserAccount_Defines_GetStatusValues", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void UserAccount_Profile_CheckUserName(String BuddyApplicationName, String BuddyApplicationPassword, String UserNameToVerify, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserNameToVerify"] = UserNameToVerify;

	
	CallMethodAsync<String>("UserAccount_Profile_CheckUserName", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void UserAccount_Profile_CheckUserEmail(String BuddyApplicationName, String BuddyApplicationPassword, String UserEmailToVerify, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserEmailToVerify"] = UserEmailToVerify;

	
	CallMethodAsync<String>("UserAccount_Profile_CheckUserEmail", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void UserAccount_Profile_GetFromUserName(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String UserNameToFetch, Action<BuddyCallResult<InternalModels.DataContract_PublicUserProfile[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["UserNameToFetch"] = UserNameToFetch;

	
	CallMethodAsync<InternalModels.DataContract_PublicUserProfile[]>("UserAccount_Profile_GetFromUserName", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void UserAccount_Profile_DeleteAccount(String BuddyApplicationName, String BuddyApplicationPassword, String UserProfileID, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserProfileID"] = UserProfileID;

	
	CallMethodAsync<String>("UserAccount_Profile_DeleteAccount", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void UserAccount_Profile_GetUserIDFromUserToken(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;

	
	CallMethodAsync<String>("UserAccount_Profile_GetUserIDFromUserToken", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void UserAccount_Identity_CheckForValues(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String IdentityValue, Action<BuddyCallResult<InternalModels.DataContract_IdentityCheck[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["IdentityValue"] = IdentityValue;

	
	CallMethodAsync<InternalModels.DataContract_IdentityCheck[]>("UserAccount_Identity_CheckForValues", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void UserAccount_Identity_AddNewValue(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String IdentityValue, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["IdentityValue"] = IdentityValue;

	
	CallMethodAsync<String>("UserAccount_Identity_AddNewValue", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void UserAccount_Identity_RemoveValue(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, String IdentityValue, Action<BuddyCallResult<String>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;
	parameters["IdentityValue"] = IdentityValue;

	
	CallMethodAsync<String>("UserAccount_Identity_RemoveValue", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
public void UserAccount_Identity_GetMyList(String BuddyApplicationName, String BuddyApplicationPassword, String UserToken, Action<BuddyCallResult<InternalModels.DataContract_IdentityValueList[]>> callback){
	IDictionary<string, object> parameters = new Dictionary<string, object>();
	parameters["BuddyApplicationName"] = BuddyApplicationName;
	parameters["BuddyApplicationPassword"] = BuddyApplicationPassword;
	parameters["UserToken"] = UserToken;

	
	CallMethodAsync<InternalModels.DataContract_IdentityValueList[]>("UserAccount_Identity_GetMyList", parameters, (bcr) => {
		CallOnUiThread((state) => callback(bcr));

	});
}
}

// for WP7.

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#if PUBLIC_SERIALIZATION
    public
#else
    internal
#endif
 class InternalModels
{

    public class DataContract_MetaDataBatchSum
    {


        public String KeyCount { get; set; }
        public String MetaKey { get; set; }
        public String SearchDistance { get; set; }
        public String TotalValue { get; set; }


    }






    public class DataContract_SearchAppMetaData
    {


        public String DistanceInKilometers { get; set; }
        public String DistanceInMeters { get; set; }
        public String DistanceInMiles { get; set; }
        public String DistanceInYards { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public String MetaKey { get; set; }
        public String MetaLatitude { get; set; }
        public String MetaLongitude { get; set; }
        public String MetaValue { get; set; }


    }






    public class DataContract_UserMetaData
    {


        public DateTime LastUpdateDate { get; set; }
        public String MetaKey { get; set; }
        public String MetaLatitude { get; set; }
        public String MetaLongitude { get; set; }
        public String MetaValue { get; set; }


    }






    public class DataContract_SearchUserMetaData
    {


        public String DistanceInKilometers { get; set; }
        public String DistanceInMeters { get; set; }
        public String DistanceInMiles { get; set; }
        public String DistanceInYards { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public String MetaKey { get; set; }
        public String MetaLatitude { get; set; }
        public String MetaLongitude { get; set; }
        public String MetaValue { get; set; }


    }






    public class DataContract_MetaDataSum
    {


        public String KeyCount { get; set; }
        public String TotalValue { get; set; }


    }






    public class DataContract_WPDeviceList
    {


        public DateTime DeviceModified { get; set; }
        public DateTime DeviceRegistered { get; set; }
        public String DeviceURI { get; set; }
        public String EnableRawMessages { get; set; }
        public String EnableTiles { get; set; }
        public String EnableToastMessages { get; set; }
        public String GroupName { get; set; }
        public String UserID { get; set; }


    }






    public class DataContract_WPGroupNames
    {


        public String DeviceCount { get; set; }
        public String GroupName { get; set; }


    }






    public class DataContract_AndroidDeviceList
    {


        public DateTime DeviceModified { get; set; }
        public DateTime DeviceRegistered { get; set; }
        public String GroupName { get; set; }
        public String RegistrationID { get; set; }
        public String UserID { get; set; }


    }






    public class DataContract_AndroidGroupNames
    {


        public String DeviceCount { get; set; }
        public String GroupName { get; set; }


    }






    public class DataContract_AppleGroupNames
    {


        public String DeviceCount { get; set; }
        public String GroupName { get; set; }


    }






    public class DataContract_AppleDeviceList
    {


        public String APNSDeviceToken { get; set; }
        public DateTime DeviceModified { get; set; }
        public DateTime DeviceRegistered { get; set; }
        public String GroupName { get; set; }
        public String UserID { get; set; }


    }






    public class DataContract_Win8DeviceList
    {


        public String ClientID { get; set; }
        public String ClientSecret { get; set; }
        public DateTime DeviceModified { get; set; }
        public DateTime DeviceRegistered { get; set; }
        public String DeviceURI { get; set; }
        public String GroupName { get; set; }
        public String UserID { get; set; }


    }






    public class DataContract_Win8GroupNames
    {


        public String DeviceCount { get; set; }
        public String GroupName { get; set; }


    }






    public class DataContract_ApplicationEmail
    {


        public String RowNumber { get; set; }
        public String UserEmail { get; set; }


    }






    public class DataContract_ApplicationUserProfile
    {


        public String Age { get; set; }
        public String CelebMode { get; set; }
        public String CreatedDate { get; set; }
        public String LastLoginDate { get; set; }
        public String LocationFuzzing { get; set; }
        public String ProfilePictureUrl { get; set; }
        public String RowNumber { get; set; }
        public String StatusID { get; set; }
        public String UserEmail { get; set; }
        public String UserGender { get; set; }
        public String UserID { get; set; }
        public String UserLatitude { get; set; }
        public String UserLongitude { get; set; }
        public String UserName { get; set; }


    }






    public class DataContract_ApplicationStats
    {


        public String TotalAlbums { get; set; }
        public String TotalAppMetadata { get; set; }
        public String TotalCrashes { get; set; }
        public String TotalDeviceInformation { get; set; }
        public String TotalFriends { get; set; }
        public String TotalGamePlayers { get; set; }
        public String TotalGameScores { get; set; }
        public String TotalMessages { get; set; }
        public String TotalPhotos { get; set; }
        public String TotalPushMessages { get; set; }
        public String TotalUserCheckins { get; set; }
        public String TotalUserMetadata { get; set; }
        public String TotalUsers { get; set; }


    }






    public class DataContract_GroupMessage
    {


        public String AppTag { get; set; }
        public String ChatGroupID { get; set; }
        public String FromUserID { get; set; }
        public String Latitude { get; set; }
        public String Longitude { get; set; }
        public String MessageID { get; set; }
        public String MessageText { get; set; }
        public String SentDateTime { get; set; }


    }






    public class DataContract_GroupChatMemberships
    {


        public String ApplicationTag { get; set; }
        public String ChatGroupID { get; set; }
        public String ChatGroupName { get; set; }
        public String CreatedDateTime { get; set; }
        public String LastMessageApplicationTag { get; set; }
        public String LastMessageDateTime { get; set; }
        public String LastMessageLatitude { get; set; }
        public String LastMessageLongitude { get; set; }
        public String LastMessagePhotoURL { get; set; }
        public String LastMessageText { get; set; }
        public String MemberUserIDList { get; set; }
        public String MembershipCounter { get; set; }
        public String OwnerUserID { get; set; }


    }






    public class DataContract_PlacesCategoryList
    {


        public String CategoryID { get; set; }
        public String CategoryName { get; set; }


    }






    public class DataContract_SearchPlaces
    {


        public String Address { get; set; }
        public String AppTagData { get; set; }
        public String CategoryID { get; set; }
        public String CategoryName { get; set; }
        public String City { get; set; }
        public String CreatedDate { get; set; }
        public String DistanceInKilometers { get; set; }
        public String DistanceInMeters { get; set; }
        public String DistanceInMiles { get; set; }
        public String DistanceInYards { get; set; }
        public String Fax { get; set; }
        public String GeoID { get; set; }
        public String Latitude { get; set; }
        public String Longitude { get; set; }
        public String Name { get; set; }
        public String PostalState { get; set; }
        public String PostalZip { get; set; }
        public String Region { get; set; }
        public String ShortID { get; set; }
        public String Telephone { get; set; }
        public String TouchedDate { get; set; }
        public String UserTagData { get; set; }
        public String WebSite { get; set; }


    }






    public class DataContract_SearchStartups
    {


        public String CenterLat { get; set; }
        public String CenterLong { get; set; }
        public String City { get; set; }
        public String CrunchBaseUrl { get; set; }
        public String CustomData { get; set; }
        public String DistanceInKilometers { get; set; }
        public String DistanceInMeters { get; set; }
        public String DistanceInMiles { get; set; }
        public String DistanceInYards { get; set; }
        public String EmployeeCount { get; set; }
        public String FacebookURL { get; set; }
        public String FundingSource { get; set; }
        public String HomePageURL { get; set; }
        public String Industry { get; set; }
        public String LinkedinURL { get; set; }
        public String LogoURL { get; set; }
        public String MetroLocation { get; set; }
        public String PhoneNumber { get; set; }
        public String StartupID { get; set; }
        public String StartupName { get; set; }
        public String State { get; set; }
        public String StreetAddress { get; set; }
        public String TotalFundingRaised { get; set; }
        public String TwitterURL { get; set; }
        public String ZipPostal { get; set; }


    }






    public class DataContract_MetroList
    {


        public String IconURL { get; set; }
        public String ImageURL { get; set; }
        public String MetroName { get; set; }
        public String StartupCount { get; set; }


    }






    public class DataContract_FriendRequests
    {


        public String Age { get; set; }
        public DateTime CreatedDate { get; set; }
        public String FriendID { get; set; }
        public DateTime LastLoginDate { get; set; }
        public String ProfilePictureUrl { get; set; }
        public String StatusID { get; set; }
        public String UserApplicationTag { get; set; }
        public String UserEmail { get; set; }
        public String UserGender { get; set; }
        public String UserID { get; set; }
        public String UserLatitude { get; set; }
        public String UserLongitude { get; set; }
        public String UserName { get; set; }


    }






    public class DataContract_FriendList
    {


        public String Age { get; set; }
        public DateTime CreatedDate { get; set; }
        public String FriendID { get; set; }
        public DateTime LastLoginDate { get; set; }
        public String ProfilePictureUrl { get; set; }
        public String Status { get; set; }
        public String StatusID { get; set; }
        public String UserApplicationTag { get; set; }
        public String UserEmail { get; set; }
        public String UserGender { get; set; }
        public String UserID { get; set; }
        public String UserLatitude { get; set; }
        public String UserLongitude { get; set; }
        public String UserName { get; set; }


    }






    public class DataContract_SearchFriends
    {


        public String Age { get; set; }
        public String DistanceInKilometers { get; set; }
        public String DistanceInMeters { get; set; }
        public String DistanceInMiles { get; set; }
        public String DistanceInYards { get; set; }
        public String ProfilePictureUrl { get; set; }
        public String RowNumber { get; set; }
        public String StatusID { get; set; }
        public String UserGender { get; set; }
        public String UserID { get; set; }
        public String UserLatitude { get; set; }
        public String UserLongitude { get; set; }
        public String UserName { get; set; }


    }






    public class DataContract_BlockedList
    {


        public String Age { get; set; }
        public String BlockedProfileID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime DateBlocked { get; set; }
        public DateTime LastLoginDate { get; set; }
        public String ProfilePictureUrl { get; set; }
        public String StatusID { get; set; }
        public String UserApplicationTag { get; set; }
        public String UserEmail { get; set; }
        public String UserGender { get; set; }
        public String UserID { get; set; }
        public String UserLatitude { get; set; }
        public String UserLongitude { get; set; }
        public String UserName { get; set; }


    }






    public class DataContract_SentFriendRequests
    {


        public String Age { get; set; }
        public DateTime CreatedDate { get; set; }
        public String FriendID { get; set; }
        public DateTime LastLoginDate { get; set; }
        public String ProfilePictureUrl { get; set; }
        public String Status { get; set; }
        public String StatusID { get; set; }
        public String UserEmail { get; set; }
        public String UserGender { get; set; }
        public String UserID { get; set; }
        public String UserLatitude { get; set; }
        public String UserLongitude { get; set; }
        public String UserName { get; set; }


    }






    public class DataContract_ApplicationMetaData
    {


        public DateTime LastUpdateDate { get; set; }
        public String MetaKey { get; set; }
        public String MetaLatitude { get; set; }
        public String MetaLongitude { get; set; }
        public String MetaValue { get; set; }


    }






    public class DataContract_PhotoList
    {


        public DateTime AddedDateTime { get; set; }
        public String AlbumID { get; set; }
        public String ApplicationTag { get; set; }
        public String FullPhotoURL { get; set; }
        public String Latitude { get; set; }
        public String Longitude { get; set; }
        public String PhotoComment { get; set; }
        public String PhotoID { get; set; }
        public String ShortID { get; set; }
        public String ShortURL { get; set; }
        public String ThumbnailPhotoURL { get; set; }


    }






    public class DataContract_PhotoAlbumList
    {


        public String AlbumID { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime LastUpdatedDateTime { get; set; }
        public String PhotoAlbumName { get; set; }
        public String PhotoAlbumThumbnail { get; set; }
        public String PhotoCount { get; set; }


    }






    public class DataContract_VirtualPhotoList
    {


        public DateTime AddedDateTime { get; set; }
        public String ApplicationTag { get; set; }
        public String FullPhotoURL { get; set; }
        public String Latitude { get; set; }
        public String Longitude { get; set; }
        public String PhotoComment { get; set; }
        public String PhotoID { get; set; }
        public String ThumbnailPhotoURL { get; set; }
        public String UserID { get; set; }
        public String VirtualAlbumID { get; set; }


    }






    public class DataContract_VirtualAlbumBatchAddResults
    {


        public String NewVirtualPhotoID { get; set; }
        public String OriginalPhotoID { get; set; }


    }






    public class DataContract_VirtualPhotoAlbumInformation
    {


        public String ApplicationTag { get; set; }
        public String CreatedDateTime { get; set; }
        public String LastUpdatedDateTime { get; set; }
        public String PhotoAlbumName { get; set; }
        public String PhotoAlbumThumbnail { get; set; }
        public String PhotoCount { get; set; }
        public String UserID { get; set; }
        public String VirtualAlbumID { get; set; }


    }






    public class DataContract_PublicPhotoSearch
    {


        public String ApplicationTag { get; set; }
        public String DistanceInKilometers { get; set; }
        public String DistanceInMeters { get; set; }
        public String DistanceInMiles { get; set; }
        public String DistanceInYards { get; set; }
        public String FullPhotoURL { get; set; }
        public String Latitude { get; set; }
        public String Longitude { get; set; }
        public String PhotoAdded { get; set; }
        public String PhotoAlbumName { get; set; }
        public String PhotoAlbumThumbnail { get; set; }
        public String PhotoID { get; set; }
        public String ShortID { get; set; }
        public String ShortURL { get; set; }
        public String ThumbnailPhotoURL { get; set; }
        public String UserProfileID { get; set; }


    }






    public class DataContract_PictureFilterList
    {


        public String FilterID { get; set; }
        public String FilterName { get; set; }
        public String ParameterList { get; set; }


    }






    public class DataContract_ProfilePhotos
    {


        public DateTime AddedDateTime { get; set; }
        public String FullPhotoURL { get; set; }
        public String Latitude { get; set; }
        public String Longitude { get; set; }
        public String PhotoComment { get; set; }
        public String PhotoID { get; set; }
        public String ShortID { get; set; }
        public String ShortURL { get; set; }
        public String ThumbnailPhotoURL { get; set; }
        public String UserProfileID { get; set; }


    }






    public class DataContract_Messages
    {


        public DateTime DateSent { get; set; }
        public String FromUserID { get; set; }
        public String MessageString { get; set; }
        public String UserName { get; set; }


    }






    public class DataContract_MessagesFromMe
    {


        public DateTime DateSent { get; set; }
        public String MessageString { get; set; }
        public String ToUserID { get; set; }


    }






    public class DataContract_SendGroupMessageResult
    {


        public String MemberUserIDList { get; set; }
        public String SendResult { get; set; }


    }






    public class DataContract_CommerceStoreGetItems
    {


        public String AppData { get; set; }
        public String CustomItemID { get; set; }
        public String ItemAvailableFlag { get; set; }
        public String ItemCost { get; set; }
        public String ItemDateTime { get; set; }
        public String ItemDescription { get; set; }
        public String ItemDownloadUri { get; set; }
        public String ItemFreeFlag { get; set; }
        public String ItemIconUri { get; set; }
        public String ItemName { get; set; }
        public String ItemPreviewUri { get; set; }
        public String StoreItemID { get; set; }


    }






    public class DataContract_CommerceReceipt
    {


        public String AppData { get; set; }
        public String HistoryCustomTransactionID { get; set; }
        public String HistoryDateTime { get; set; }
        public String ItemQuantity { get; set; }
        public String ReceiptData { get; set; }
        public String ReceiptHistoryID { get; set; }
        public String StoreItemID { get; set; }
        public String StoreName { get; set; }
        public String TotalCost { get; set; }
        public String UserID { get; set; }
        public String VerificationResult { get; set; }
        public String VerificationResultData { get; set; }


    }






    public class DataContract_GameUserScoreList
    {


        public String ApplicationTag { get; set; }
        public String ScoreBoardName { get; set; }
        public String ScoreDate { get; set; }
        public String ScoreID { get; set; }
        public String ScoreLatitude { get; set; }
        public String ScoreLongitude { get; set; }
        public String ScoreRank { get; set; }
        public String ScoreValue { get; set; }
        public String UserID { get; set; }
        public String UserName { get; set; }


    }






    public class DataContract_GameBoardScoreList
    {


        public String ApplicationTag { get; set; }
        public String ScoreBoardName { get; set; }
        public String ScoreDate { get; set; }
        public String ScoreID { get; set; }
        public String ScoreLatitude { get; set; }
        public String ScoreLongitude { get; set; }
        public String ScoreRank { get; set; }
        public String ScoreValue { get; set; }
        public String UserID { get; set; }
        public String UserName { get; set; }


    }












    public class DataContract_GameScoreSearchResults
    {


        public String ApplicationTag { get; set; }
        public String ScoreBoardName { get; set; }
        public String ScoreDate { get; set; }
        public String ScoreID { get; set; }
        public String ScoreLatitude { get; set; }
        public String ScoreLongitude { get; set; }
        public String ScoreRank { get; set; }
        public String ScoreValue { get; set; }
        public String UserID { get; set; }
        public String UserName { get; set; }


    }






    public class DataContract_GamePlayerSearchResults
    {


        public String ApplicationTag { get; set; }
        public String DistanceInKilometers { get; set; }
        public String DistanceInMeters { get; set; }
        public String DistanceInMiles { get; set; }
        public String DistanceInYards { get; set; }
        public String PlayerBoardName { get; set; }
        public String PlayerDate { get; set; }
        public String PlayerID { get; set; }
        public String PlayerLatitude { get; set; }
        public String PlayerLongitude { get; set; }
        public String PlayerName { get; set; }
        public String PlayerRank { get; set; }
        public String UserID { get; set; }
        public String UserName { get; set; }


    }






    public class DataContract_GamePlayerInfo
    {


        public String ApplicationTag { get; set; }
        public String PlayerBoardName { get; set; }
        public String PlayerDate { get; set; }
        public String PlayerID { get; set; }
        public String PlayerLatitude { get; set; }
        public String PlayerLongitude { get; set; }
        public String PlayerName { get; set; }
        public String PlayerRank { get; set; }
        public String UserID { get; set; }
        public String UserName { get; set; }


    }






    public class DataContract_GameStateObject
    {


        public String AppTag { get; set; }
        public String StateDateTime { get; set; }
        public String StateID { get; set; }
        public String StateKey { get; set; }
        public String StateValue { get; set; }


    }






    public class DataContract_FullUserProfile
    {


        public String Age { get; set; }
        public String CelebMode { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public String LocationFuzzing { get; set; }
        public String ProfilePictureUrl { get; set; }
        public String StatusID { get; set; }
        public String UserApplicationTag { get; set; }
        public String UserEmail { get; set; }
        public String UserGender { get; set; }
        public String UserID { get; set; }
        public String UserLatitude { get; set; }
        public String UserLongitude { get; set; }
        public String UserName { get; set; }


    }






    public class DataContract_PublicUserProfile
    {


        public String Age { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public String ProfilePictureUrl { get; set; }
        public String StatusID { get; set; }
        public String UserApplicationTag { get; set; }
        public String UserEmail { get; set; }
        public String UserGender { get; set; }
        public String UserID { get; set; }
        public String UserLatitude { get; set; }
        public String UserLongitude { get; set; }
        public String UserName { get; set; }


    }






    public class DataContract_SearchPeople
    {


        public String Age { get; set; }
        public String DistanceInKilometers { get; set; }
        public String DistanceInMeters { get; set; }
        public String DistanceInMiles { get; set; }
        public String DistanceInYards { get; set; }
        public String ProfilePictureUrl { get; set; }
        public String StatusID { get; set; }
        public String UserApplicationTag { get; set; }
        public String UserGender { get; set; }
        public String UserID { get; set; }
        public String UserLatitude { get; set; }
        public String UserLongitude { get; set; }
        public String UserName { get; set; }


    }






    public class DataContract_UserLocationHistory
    {


        public DateTime CreatedDate { get; set; }
        public String Latitude { get; set; }
        public String Longitude { get; set; }
        public String PlaceName { get; set; }


    }


    public class DataContract_Blob
    {
        public String BlobID { get; set; }
        public String FriendlyName { get; set; }
        public String MimeType { get; set; }
        public String FileSize { get; set; }
        public String AppTag { get; set; }
        public String Owner { get; set; }
        public String Latitude { get; set; }
        public String Longitude { get; set; }
        public DateTime UploadDate { get; set; }
        public DateTime LastTouchDate { get; set; }

    }

    public class DataContract_Video
    {
        public String VideoID { get; set; }
        public String FriendlyName { get; set; }
        public String MimeType { get; set; }
        public String FileSize { get; set; }
        public String AppTag { get; set; }
        public String Owner { get; set; }
        public String Latitude { get; set; }
        public String Longitude { get; set; }
        public String UploadDate { get; set; }
        public String LastTouchDate { get; set; }
        public String VideoUrl { get; set; }
    }


    public class DataContract_SocialLoginReply
    {
        public String UserID { get; set; }
        public String UserName { get; set; }
        public String UserToken { get; set; }
        public String IsNew { get; set; }
    }

    public class DataContract_DefinedUserStatusTags
    {


        public String StatusID { get; set; }
        public String StatusTag { get; set; }


    }






    public class DataContract_IdentityCheck
    {


        public String IdentityValue { get; set; }
        public String UserProfileID { get; set; }
        public String ValueFound { get; set; }


    }






    public class DataContract_IdentityValueList
    {


        public String CreatedDateTime { get; set; }
        public String IdentityValue { get; set; }


    }

}





#endregion

}
