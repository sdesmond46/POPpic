using BuddyServiceClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Buddy
{
    /// <summary>
    /// Represents a class that can be used to add, retrieve or delete Blobs.
    /// <example>
    /// <code>
    ///     
    /// </code>
    /// </example>
    /// </summary>
    public class Blobs : BuddyBase
    {
        public Blobs(BuddyClient client, AuthenticatedUser user)
            : base(client, user) { }

        protected override bool AuthUserRequired{
            get{ return true; }
        }

        /// <summary>
        /// Uploads a Blob and returns a Blob object representing it.
        /// </summary>
        /// <param name="friendlyName">The human friendly name for the blob.</param>
        /// <param name="mimeType">The MimeType of the blob.</param>
        /// <param name="appTag">Optional metadata to store with the Blob object. ie: comments on the Blob.</param>
        /// <param name="latitude">The latitude of the location where the Blob object is being created.</param>
        /// <param name="longitude">The longitude of the location where the Blob object is being created.</param>
        /// <param name="blobData">The Stream of the Blob to upload.</param>
        /// <returns>A Task&lt;Blob&gt;that can be used to monitor progress on this call.</returns>
        public Task<Blob> AddAsync(string friendlyName, string mimeType, string appTag, double latitude, double longitude, Stream blobData)
        {
            if (blobData == null || blobData.Length <= 0)
            {
                throw new ArgumentNullException("blobData");
            }

            var tcs = new TaskCompletionSource<Blob>();
            this.AddInternal(friendlyName, mimeType, appTag, latitude, longitude, blobData, (BuddyCallResult<Blob> bcr) =>
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
        /// Gets the bytes for the given Blob.
        /// </summary>
        /// <param name="blobID">The ID of the Blob to be retrieved.</param>
        /// <returns>A Task&lt;Stream&gt;that can be used to monitor progress on this call.</returns>
        public Task<Stream> GetAsync(long blobID)
        {
            var tcs = new TaskCompletionSource<Stream>();
            this.GetInternal(blobID, (bcr) =>
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
        /// Gets a Blob object that represents the given Blob.
        /// </summary>
        /// <param name="blobID">The ID of the blob to be retrieved.</param>
        /// <returns>A Task&lt;Blob&gt;that can be used to monitor progress on this call.</returns>
        public Task<Blob> GetInfoAsync(long blobID)
        {
            var tcs = new TaskCompletionSource<Blob>();
            this.GetInfoInternal(blobID, (bcr) =>
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
        /// Searches the Blobs belonging to the current user using the given criteria.
        /// </summary>
        /// <param name="friendlyName">The string to search the friendlyName by. Use % as a wildcard. Null or empty string to ignore.</param>
        /// <param name="mimeType">The string to search the MIMEType by. Use % as a wildcard. Null or empty string to ignore.</param>
        /// <param name="appTag">The string to search the AppTag by. Use % as a wildcard. Null or empty string to ignore.</param>
        /// <param name="searchDistance">The radius of the startup search. Pass -1 to ignore.</param>
        /// <param name="searchLatitude">The latitude where the search should start.</param>
        /// <param name="searchLongitude">The longitude where the search should start.</param>
        /// <param name="timeFilter">The number of days in the past to search. Pass -1 to ignore.</param>
        /// <param name="recordLimit">The maximum number of results to return. No larger than 500.</param>
        /// <returns>A Task&lt;IEnumerable&lt;Blob&gt; &gt;that can be used to monitor progress on this call.</returns>
        public Task<IEnumerable<Blob>> SearchMyBlobsAsync(string friendlyName, string mimeType, string appTag,
            int searchDistance, double searchLatitude, double searchLongitude, int timeFilter, int recordLimit)
        {
            var tcs = new TaskCompletionSource<IEnumerable<Blob>>();
            this.SearchMyBlobsInternal(friendlyName, mimeType, appTag, searchDistance, searchLatitude, searchLongitude, timeFilter, recordLimit, (bcr) =>
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
        /// Searches the Blobs belonging to all users using the given criteria.
        /// </summary>
        /// <param name="friendlyName">The string to search the friendlyName by. Use % as a wildcard. Null or empty string to ignore.</param>
        /// <param name="mimeType">The string to search the MIMEType by. Use % as a wildcard. Null or empty string to ignore.</param>
        /// <param name="appTag">The string to search the AppTag by. Use % as a wildcard. Null or empty string to ignore.</param>
        /// <param name="searchDistance">The radius of the startup search. Pass -1 to ignore.</param>
        /// <param name="searchLatitude">The latitude where the search should start.</param>
        /// <param name="searchLongitude">The longitude where the search should start.</param>
        /// <param name="timeFilter">The number of days in the past to search. Pass -1 to ignore</param>
        /// <param name="recordLimit">The maximum number of results to return. No larger than 500.</param>
        /// <returns>A Task&lt;IEnumerable&lt;Blob&gt; &gt;that can be used to monitor progress on this call.</returns>
        public Task<IEnumerable<Blob>> SearchBlobsAsync(string friendlyName, string mimeType, string appTag,
            int searchDistance, double searchLatitude, double searchLongitude, int timeFilter, int recordLimit)
        {
            var tcs = new TaskCompletionSource<IEnumerable<Blob>>();
            this.SearchBlobsInternal(friendlyName, mimeType, appTag, searchDistance, searchLatitude, searchLongitude, timeFilter, recordLimit, (bcr) =>
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
        /// Gets a list of the blobs belonging to either the given User or to all users of the App.
        /// </summary>
        /// <param name="userID">The UserID for which to return Blobs. Pass -1 for all users.</param>
        /// <param name="recordLimit">The maximum number of results to return. No larger than 500.</param>
        /// <returns>A Task&lt;IEnumerable&lt;Blob&gt; &gt;that can be used to monitor progress on this call.</returns>
        public Task<IEnumerable<Blob>> GetBlobListAsync(long userID, int recordLimit)
        {
            var tcs = new TaskCompletionSource<IEnumerable<Blob>>();
            this.GetListInternal(userID, recordLimit, (bcr) =>
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
        /// Gets a list of the blobs belonging to the current User.
        /// </summary>
        /// <param name="recordLimit">The maximum number of results to return. No larger than 500.</param>
        /// <returns>A Task&lt;IEnumerable&lt;Blob&gt; &gt;that can be used to monitor progress on this call.</returns>
        public Task<IEnumerable<Blob>> GetMyBlobListAsync(int recordLimit)
        {
            var tcs = new TaskCompletionSource<IEnumerable<Blob>>();
            this.GetMyListInternal(recordLimit, (bcr) =>
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

        internal void AddInternal(string friendlyName, string mimeType, string appTag, double latitude, double longitude, Stream blobData, Action<BuddyCallResult<long>> callback)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("BuddyApplicationName", this.Client.AppName);
            parameters.Add("BuddyApplicationPassword", this.Client.AppPassword);
            parameters.Add("UserToken", this.AuthUser.Token);
            parameters.Add("FriendlyName", friendlyName);
            parameters.Add("AppTag", appTag);
            parameters.Add("Latitude", latitude);
            parameters.Add("Longitude", longitude);
            parameters.Add("BlobData", new BuddyFile { Data = blobData, 
                                                        ContentType = mimeType,
                                                        Name = "BlobData"});

            this.Client.Service.CallMethodAsync<string>("Blobs_Blob_AddBlob", parameters, (bcr) =>
            {
                long result = -1;
                if (bcr.Result != null)
                {
                    result = long.Parse(bcr.Result);
                }
                callback(BuddyResultCreator.Create(result, bcr.Error));
            });
        }

        internal void AddInternal(string friendlyName, string mimeType, string appTag, double latitude, double longitude, Stream blobData, Action<BuddyCallResult<Blob>> callback)
        {
            AddInternal(friendlyName, mimeType, appTag, latitude, longitude, blobData, (bcr) =>
            {
                if (bcr.Error == BuddyError.None)
                {
                    this.GetInfoInternal(bcr.Result, (bdr) =>
                    {
                        callback(bdr);
                    });
                }
                else
                {
                    callback(BuddyResultCreator.Create<Blob>(null, bcr.Error));
                }
            });
        }

        internal void DeleteInternal(long blobID, Action<BuddyCallResult<bool>> callback)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("BuddyApplicationName", this.Client.AppName);
            parameters.Add("BuddyApplicationPassword", this.Client.AppPassword);
            parameters.Add("UserToken", this.AuthUser.Token);
            parameters.Add("BlobID", blobID);

            this.Client.Service.CallMethodAsync<string>("Blobs_Blob_DeleteBlob", parameters, (bcr) =>
            {
                bool result = false;
                if (bcr.Result != null)
                {
                    result = bcr.Result == "1";
                }
                callback(BuddyResultCreator.Create<bool>(result, bcr.Error));
            });
        }

        internal void GetInfoInternal(long blobID, Action<BuddyServiceClient.BuddyCallResult<Blob>> callback)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("BuddyApplicationName", this.Client.AppName);
            parameters.Add("BuddyApplicationPassword", this.Client.AppPassword);
            parameters.Add("UserToken", this.AuthUser.Token);
            parameters.Add("BlobID", blobID);

            this.Client.Service.CallMethodAsync<InternalModels.DataContract_Blob[]>("Blobs_Blob_GetBlobInfo", parameters, (bcr) =>
            {
                Blob result = null;
                if (bcr.Result != null)
                {
                    result = new Blob(this.Client, this.AuthUser, bcr.Result.First());
                }
                callback(BuddyResultCreator.Create((Blob)result, bcr.Error));
            });
        }

        internal void SearchBlobsInternal(string friendlyName, string mimeType, string appTag,
            int searchDistance, double searchLatitude, double searchLongitude, int timeFilter, int recordLimit, Action<BuddyCallResult<IEnumerable<Blob>>> callback)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("BuddyApplicationName", this.Client.AppName);
            parameters.Add("BuddyApplicationPassword", this.Client.AppPassword);
            parameters.Add("UserToken", this.AuthUser.Token);
            parameters.Add("FriendlyName", friendlyName);
            parameters.Add("MimeType", mimeType);
            parameters.Add("AppTag", appTag);
            parameters.Add("SearchDistance", searchDistance);
            parameters.Add("SearchLatitude", searchLatitude);
            parameters.Add("SearchLongitude", searchLongitude);
            parameters.Add("TimeFilter", timeFilter);
            parameters.Add("RecordLimit", recordLimit);

            this.Client.Service.CallMethodAsync<InternalModels.DataContract_Blob[]>("Blobs_Blob_SearchBlobs", parameters, (bcr) =>
            {
                List<Blob> result = new List<Blob>();
                if (bcr.Result != null)
                {
                    foreach (InternalModels.DataContract_Blob b in bcr.Result)
                    {
                        result.Add(new Blob(this.Client, this.AuthUser, b));
                    }
                }
                callback(BuddyServiceClient.BuddyResultCreator.Create((IEnumerable<Blob>)result, bcr.Error));
            });
        }

        internal void SearchMyBlobsInternal(string friendlyName, string mimeType, string appTag,
            int searchDistance, double searchLatitude, double searchLongitude, int timeFilter, int recordLimit, Action<BuddyCallResult<IEnumerable<Blob>>> callback)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("BuddyApplicationName", this.Client.AppName);
            parameters.Add("BuddyApplicationPassword", this.Client.AppPassword);
            parameters.Add("UserToken", this.AuthUser.Token);
            parameters.Add("FriendlyName", friendlyName);
            parameters.Add("MimeType", mimeType);
            parameters.Add("AppTag", appTag);
            parameters.Add("SearchDistance", searchDistance);
            parameters.Add("SearchLatitude", searchLatitude);
            parameters.Add("SearchLongitude", searchLongitude);
            parameters.Add("TimeFilter", timeFilter);
            parameters.Add("RecordLimit", recordLimit);

            this.Client.Service.CallMethodAsync<InternalModels.DataContract_Blob[]>("Blobs_Blob_SearchMyBlobs", parameters, (bcr) =>
            {
                List<Blob> result = new List<Blob>();
                if (bcr.Result != null)
                {
                    foreach (var b in bcr.Result)
                    {
                        result.Add(new Blob(this.Client, this.AuthUser, b));
                    }
                }
                callback(BuddyServiceClient.BuddyResultCreator.Create((IEnumerable<Blob>)result, bcr.Error));
            });
        }

        internal void GetListInternal(long userID, int recordLimit, Action<BuddyCallResult<IEnumerable<Blob>>> callback)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("BuddyApplicationName", this.Client.AppName);
            parameters.Add("BuddyApplicationPassword", this.Client.AppPassword);
            parameters.Add("UserToken", this.AuthUser.Token);
            parameters.Add("UserID", userID);
            parameters.Add("RecordLimit", recordLimit);

            this.Client.Service.CallMethodAsync<InternalModels.DataContract_Blob[]>("Blobs_Blob_GetBlobList", parameters, (bcr) =>
            {
                List<Blob> result = new List<Blob>();
                var brd = bcr.Result;
                if (brd != null)
                {
                    foreach (var b in brd)
                    {
                        var blob = new Blob(this.Client, this.AuthUser, b);
                        result.Add(blob);
                    }
                }
                callback(BuddyServiceClient.BuddyResultCreator.Create((IEnumerable<Blob>)result, bcr.Error));
            });
        }

        internal void GetMyListInternal(int recordLimit, Action<BuddyCallResult<IEnumerable<Blob>>> callback)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("BuddyApplicationName", this.Client.AppName);
            parameters.Add("BuddyApplicationPassword", this.Client.AppPassword);
            parameters.Add("UserToken", this.AuthUser.Token);
            parameters.Add("RecordLimit", recordLimit);

            this.Client.Service.CallMethodAsync<InternalModels.DataContract_Blob[]>("Blobs_Blob_GetMyBlobList", parameters, (bcr) =>
            {
                List<Blob> result = new List<Blob>();
                if (bcr.Result != null)
                {
                    foreach (var b in bcr.Result)
                    {
                        result.Add(new Blob(this.Client, this.AuthUser, b));
                    }
                }
                callback(BuddyServiceClient.BuddyResultCreator.Create((IEnumerable<Blob>)result, bcr.Error));
            });
        }

        internal void GetInternal(long blobID, Action<BuddyCallResult<Stream>> callback)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("BuddyApplicationName", this.Client.AppName);
            parameters.Add("BuddyApplicationPassword", this.Client.AppPassword);
            parameters.Add("UserToken", this.AuthUser.Token);
            parameters.Add("BlobID", blobID);

            this.Client.Service.CallMethodAsync<HttpWebResponse>("Blobs_Blob_GetBlob", parameters, (bcr) =>
            {
                Stream result = null;
                if (bcr.Result != null)
                {
                    result = bcr.Result.GetResponseStream();
                }
                callback(BuddyServiceClient.BuddyResultCreator.Create(result, bcr.Error));
            });
        }
    }
}
