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
    public class Videos : BuddyBase
    {
        public Videos(BuddyClient client, AuthenticatedUser user)
            : base(client, user) { }

        protected override bool AuthUserRequired {
            get { return true; }
        }
        /// <summary>
        /// Uploads a Video and returns a Video object representing it.
        /// </summary>
        /// <param name="friendlyName">The human friendly name for the video.</param>
        /// <param name="mimeType">The MIMEType of the video.</param>
        /// <param name="appTag">Optional metadata to store with the Video object. ie: comments on the Video.</param>
        /// <param name="latitude">The latitude of the location where the Video object is being created.</param>
        /// <param name="longitude">The longitude of the location where the Video object is being created.</param>
        /// <param name="videoData">The bytes of the Video to upload.</param>
        /// <returns>A Task&lt;Video&gt; that can be used to monitor progress on this call.</returns>
        public Task<Video> AddAsync(string friendlyName, string mimeType, string appTag, double latitude, double longitude, Stream videoData)
        {
            if (videoData == null || videoData.Length <= 0)
            {
                throw new ArgumentException("videoData");
            }

            var tcs = new TaskCompletionSource<Video>();
            this.AddInternal(friendlyName, mimeType, appTag, latitude, longitude, videoData, (BuddyCallResult<Video> bcr) =>
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
        /// Gets a Video object that represents the given Video.
        /// </summary>
        /// <param name="videoID">The ID of the video to be retrieved.</param>
        /// <returns>A Task&lt;Video&gt;that can be used to monitor progress on this call.</returns>
        public Task<Video> GetInfoAsync(long videoID)
        {
            var tcs = new TaskCompletionSource<Video>();
            this.GetInfoInternal(videoID, (bcr) =>
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
        /// Searched the Videos belonging to the current user using the given criteria.
        /// </summary>
        /// <param name="friendlyName">The string to search the friendlyName by. Use % as a wildcard. Null or empty string to ignore.</param>
        /// <param name="mimeType">The string to search the MIMEType by. Use % as a wildcard. Null or empty string to ignore.</param>
        /// <param name="appTag">The string to search the AppTag by. Use % as a wildcard. Null or empty string to ignore.</param>
        /// <param name="searchDistance">The radius of the startup search. Pass -1 to ignore.</param>
        /// <param name="searchLatitude">The latitude where the search should start.</param>
        /// <param name="searchLongitude">The longitude where the search should start.</param>
        /// <param name="timeFilter">The number of days in the past to search. Pass -1 to ignore.</param>
        /// <param name="recordLimit">The maximum number of results to return. No larger than 500.</param>
        /// <returns>A Task&lt;IEnumerable&lt;Video&gt; &gt;that can be used to monitor progress on this call.</returns>
        public Task<IEnumerable<Video>> SearchMyVideosAsync(string friendlyName, string mimeType, string appTag,
            int searchDistance, double searchLatitude, double searchLongitude, int timeFilter, int recordLimit)
        {
            var tcs = new TaskCompletionSource<IEnumerable<Video>>();
            this.SearchMyVideosInternal(friendlyName, mimeType, appTag, searchDistance, searchLatitude, searchLongitude, timeFilter, recordLimit, (bcr) =>
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
        /// Searches the Videos belonging to all users using the given criteria.
        /// </summary>
        /// <param name="friendlyName">The string to search the friendlyName by. Use % as a wildcard. Null or empty string to ignore.</param>
        /// <param name="mimeType">The string to search the MIMEType by. Use % as a wildcard. Null or empty string to ignore.</param>
        /// <param name="appTag">The string to search the AppTag by. Use % as a wildcard. Null or empty string to ignore.</param>
        /// <param name="searchDistance">The radius of the startup search. Pass -1 to ignore.</param>
        /// <param name="searchLatitude">The latitude where the search should start.</param>
        /// <param name="searchLongitude">The longitude where the search should start.</param>
        /// <param name="timeFilter">The number of days in the past to search. Pass -1 to ignore.</param>
        /// <param name="recordLimit">The maximum number of results to return. No larger than 500.</param>
        /// <returns>A Task&lt;IEnumerable&lt;Video&gt; &gt;that can be used to monitor progress on this call.</returns>
        public Task<IEnumerable<Video>> SearchVideosAsync(string friendlyName, string mimeType, string appTag,
            int searchDistance, double searchLatitude, double searchLongitude, int timeFilter, int recordLimit)
        {
            var tcs = new TaskCompletionSource<IEnumerable<Video>>();
            this.SearchVideosInternal(friendlyName, mimeType, appTag, searchDistance, searchLatitude, searchLongitude, timeFilter, recordLimit, (bcr) =>
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
        /// Gets a list of the videos belonging to either the given User or to all users of the App.
        /// </summary>
        /// <param name="userID">The UserID for which to return Videos. Pass -1 for all users.</param>
        /// <param name="recordLimit">The maximum number of results to return. No larger than 500.</param>
        /// <returns>A Task&lt;IEnumerable&lt;Video&gt; &gt;that can be used to monitor progress on this call.</returns>
        public Task<IEnumerable<Video>> GetVideoListAsync(long userID, int recordLimit)
        {
            var tcs = new TaskCompletionSource<IEnumerable<Video>>();
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
        /// Gets a list of the videos belonging to the current User.
        /// </summary>
        /// <param name="recordLimit">The maximum number of results to return. No larger than 500.</param>
        /// <returns>A Task&lt;IEnumerable&lt;Video&gt; &gt;that can be used to monitor progress on this call.</returns>
        public Task<IEnumerable<Video>> GetMyVideoListAsync(int recordLimit)
        {
            var tcs = new TaskCompletionSource<IEnumerable<Video>>();
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

        internal void AddInternal(string friendlyName, string mimeType, string appTag, double latitude, double longitude, Stream videoData, Action<BuddyCallResult<long>> callback)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("BuddyApplicationName", this.Client.AppName);
            parameters.Add("BuddyApplicationPassword", this.Client.AppPassword);
            parameters.Add("UserToken", this.AuthUser.Token);
            parameters.Add("FriendlyName", friendlyName);
            parameters.Add("AppTag", appTag);
            parameters.Add("Latitude", latitude);
            parameters.Add("Longitude", longitude);
            parameters.Add("VideoData", new BuddyFile
            {
                Data = videoData,
                ContentType = mimeType,
                Name = "VideoData"
            });

            this.Client.Service.CallMethodAsync<string>("Videos_Video_AddVideo", parameters, (bcr) =>
            {
                long result = -1;
                if (bcr.Result != null)
                {
                    result = long.Parse(bcr.Result);
                }
                callback(BuddyResultCreator.Create(result, bcr.Error));
            });
        }

        internal void AddInternal(string friendlyName, string mimeType, string appTag, double latitude, double longitude, Stream videoData, Action<BuddyCallResult<Video>> callback)
        {
            AddInternal(friendlyName, mimeType, appTag, latitude, longitude, videoData, (bcr) =>
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
                        callback(BuddyResultCreator.Create<Video>(null, bcr.Error));
                    }
                });
        }

        internal void DeleteInternal(long videoID, Action<BuddyCallResult<bool>> callback)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("BuddyApplicationName", this.Client.AppName);
            parameters.Add("BuddyApplicationPassword", this.Client.AppPassword);
            parameters.Add("UserToken", this.AuthUser.Token);
            parameters.Add("VideoID", videoID);

            this.Client.Service.CallMethodAsync<string>("Videos_Video_DeleteVideo", parameters, (bcr) =>
            {
                bool result = false;
                if (bcr.Result != null)
                {
                    result = bcr.Result == "1";
                }
                callback(BuddyResultCreator.Create<bool>(result, bcr.Error));
            });
        }

        internal void GetInfoInternal(long VideoID, Action<BuddyServiceClient.BuddyCallResult<Video>> callback)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("BuddyApplicationName", this.Client.AppName);
            parameters.Add("BuddyApplicationPassword", this.Client.AppPassword);
            parameters.Add("UserToken", this.AuthUser.Token);
            parameters.Add("VideoID", VideoID);

            this.Client.Service.CallMethodAsync<InternalModels.DataContract_Video[]>("Videos_Video_GetVideoInfo", parameters, (bcr) =>
            {
                Video result = null;
                if (bcr.Result != null)
                {
                    result = new Video(this.Client, this.AuthUser, bcr.Result[0]);
                }
                callback(BuddyResultCreator.Create((Video)result, bcr.Error));
            });
        }

        internal void SearchVideosInternal(string friendlyName, string mimeType, string appTag,
            int searchDistance, double searchLatitude, double searchLongitude, int timeFilter, int recordLimit, Action<BuddyCallResult<IEnumerable<Video>>> callback)
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

            this.Client.Service.CallMethodAsync<InternalModels.DataContract_Video[]>("Videos_Video_SearchVideos", parameters, (bcr) =>
            {
                InternalModels.DataContract_Video[] result = null;
                if (bcr.Result != null)
                {
                    result = bcr.Result;
                }
                var lst = new List<Video>();
                foreach (var vid in result) { lst.Add(new Video(this.Client, this.AuthUser, vid)); }
                callback(BuddyServiceClient.BuddyResultCreator.Create((IEnumerable<Video>)lst, bcr.Error));
            });
        }

        internal void SearchMyVideosInternal(string friendlyName, string mimeType, string appTag,
            int searchDistance, double searchLatitude, double searchLongitude, int timeFilter, int recordLimit, Action<BuddyCallResult<IEnumerable<Video>>> callback)
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

            this.Client.Service.CallMethodAsync<InternalModels.DataContract_Video[]>("Videos_Video_SearchMyVideos", parameters, (bcr) =>
            {
                InternalModels.DataContract_Video[] result = null;
                if (bcr.Result != null)
                {
                    result = bcr.Result;
                }
                var lst = new List<Video>();
                foreach (var vid in result) { lst.Add(new Video(this.Client, this.AuthUser, vid)); }
                callback(BuddyServiceClient.BuddyResultCreator.Create((IEnumerable<Video>)lst, bcr.Error));
            });
        }

        internal void GetListInternal(long userID, int recordLimit, Action<BuddyCallResult<IEnumerable<Video>>> callback)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("BuddyApplicationName", this.Client.AppName);
            parameters.Add("BuddyApplicationPassword", this.Client.AppPassword);
            parameters.Add("UserToken", this.AuthUser.Token);
            parameters.Add("UserID", userID);
            parameters.Add("RecordLimit", recordLimit);

            this.Client.Service.CallMethodAsync<InternalModels.DataContract_Video[]>("Videos_Video_GetVideoList", parameters, (bcr) =>
            {
                InternalModels.DataContract_Video[] result = null;
                if (bcr.Result != null)
                {
                    result = bcr.Result;
                }
                var lst = new List<Video>();
                foreach (var vid in result) { lst.Add(new Video(this.Client, this.AuthUser, vid)); }
                callback(BuddyServiceClient.BuddyResultCreator.Create((IEnumerable<Video>)lst, bcr.Error));
            });
        }

        internal void GetMyListInternal(int recordLimit, Action<BuddyCallResult<IEnumerable<Video>>> callback)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("BuddyApplicationName", this.Client.AppName);
            parameters.Add("BuddyApplicationPassword", this.Client.AppPassword);
            parameters.Add("UserToken", this.AuthUser.Token);
            parameters.Add("RecordLimit", recordLimit);

            this.Client.Service.CallMethodAsync<InternalModels.DataContract_Video[]>("Videos_Video_GetMyVideoList", parameters, (bcr) =>
            {
                InternalModels.DataContract_Video[] result = null;
                if (bcr.Result != null)
                {
                    result = bcr.Result;
                }
                var lst = new List<Video>();
                foreach (var vid in result) { lst.Add(new Video(this.Client, this.AuthUser, vid)); }
                callback(BuddyServiceClient.BuddyResultCreator.Create((IEnumerable<Video>)lst, bcr.Error));
            });
        }

        internal void GetInternal(long videoID, Action<BuddyCallResult<Stream>> callback)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("BuddyApplicationName", this.Client.AppName);
            parameters.Add("BuddyApplicationPassword", this.Client.AppPassword);
            parameters.Add("UserToken", this.AuthUser.Token);
            parameters.Add("VideoID", videoID);

            this.Client.Service.CallMethodAsync<HttpWebResponse>("Videos_Video_GetVideo", parameters, (bcr) =>
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
