using BuddyServiceClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buddy
{
    /// <summary>
    /// Represents a class that can be used to add, retrieve search for and delte Video data in the system.
    /// <example>
    /// <code>
    /// 
    /// </code>
    /// </example>
    /// </summary>
    public class Video : BuddyBase
    {
        protected override bool AuthUserRequired
        {
            get{ return true; }
        }

        /// <summary>
        /// Gets the ID of the video.
        /// </summary>
        public long VideoID { get; protected set; }
        
        /// <summary>
        /// Gets the human friendly name of the video.
        /// </summary>
        public string FriendlyName { get; protected set; }

        /// <summary>
        /// Gets the MimeType of the video.
        /// </summary>
        public string MimeType { get; protected set; }

        /// <summary>
        /// Gets the size of the video in bytes.
        /// </summary>
        public int FileSize { get; protected set; }

        /// <summary>
        /// Gets the optional application tag for the video.
        /// </summary>
        public string AppTag { get; protected set; }

        /// <summary>
        /// Gets the UserID of the user that uploaded this video.
        /// </summary>
        public long Owner { get; protected set; }

        /// <summary>
        /// Gets the latitude where the video was created.
        /// </summary>
        public double Latitude { get; protected set; }

        /// <summary>
        /// Gets the longitude where the video was created.
        /// </summary>
        public double Longitude { get; protected set; }

        /// <summary>
        /// Gets the date the video was uploaded
        /// </summary>
        public DateTime UploadDate { get; protected set; }

        /// <summary>
        /// Gets the date the video was last touched (uploaded or edited)
        /// </summary>
        public DateTime LastTouchDate { get; protected set; }

        /// <summary>
        /// Gets the URL where the video can be reached. To be passed into a media player.
        /// </summary>
        public string VideoUrl {get; set;}

        internal Video(BuddyClient client, AuthenticatedUser user, InternalModels.DataContract_Video video) : base(client, user)
        { 
            this.VideoID = long.Parse(video.VideoID, CultureInfo.InvariantCulture);
            this.FriendlyName = video.FriendlyName;
            this.MimeType = video.MimeType;
            this.FileSize = int.Parse(video.FileSize, CultureInfo.InvariantCulture);
            this.AppTag = video.AppTag;
            this.Owner = long.Parse(video.Owner, CultureInfo.InvariantCulture);
            this.Latitude = double.Parse(video.Latitude, CultureInfo.InvariantCulture);
            this.Longitude = double.Parse(video.Longitude, CultureInfo.InvariantCulture);
            this.UploadDate = DateTime.Parse(video.UploadDate, CultureInfo.InvariantCulture);
            this.LastTouchDate = DateTime.Parse(video.LastTouchDate, CultureInfo.InvariantCulture);
            this.VideoUrl = video.VideoUrl;
        }

        /// <summary>
        /// Edits the information related to this Video.
        /// </summary>
        /// <param name="friendlyName">The new human friendly name for the Video. Leave null or empty to not change.</param>
        /// <param name="appTag">The new AppTag for the video. Leave null or empty to not change.</param>
        /// <returns>A Task&lt;bool&gt; that can be used to monitor progress on this call.</returns>
        public Task<bool> EditInfoAsync(string friendlyName, string appTag)
        {
            var tcs = new TaskCompletionSource<bool>();
            this.EditInfoInternal(friendlyName, appTag, (bcr) =>
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
        /// Delte this Video
        /// </summary>
        /// <returns>A Task&lt;bool&gt; that can be used to monitor progress on this call.</returns>
        public Task<bool> DeleteAsync()
        {
            var tcs = new TaskCompletionSource<bool>();
            this.DeleteInternal((bcr) =>
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

        internal void EditInfoInternal(string friendlyName, string appTag,
            Action<BuddyCallResult<bool>> callback)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("BuddyApplicationName", this.Client.AppName);
            parameters.Add("BuddyApplicationPassword", this.Client.AppPassword);
            parameters.Add("UserToken", this.AuthUser.Token);
            parameters.Add("VideoID", this.VideoID);
            parameters.Add("FriendlyName", friendlyName);
            parameters.Add("AppTag", appTag);

            this.Client.Service.CallMethodAsync<string>("Videos_Video_EditInfo", parameters, (bcr) =>
            {
                this.Client.Service.CallOnUiThread((state) => 
                    callback(BuddyResultCreator.Create(bcr.Result == "1", bcr.Error)));
            });
        }

        internal void DeleteInternal(Action<BuddyCallResult<bool>> callback)
        {
            this.AuthUser.Videos.DeleteInternal(this.VideoID, callback);
        }

        internal void GetInternal(Action<BuddyCallResult<Stream>> callback)
        {
            this.AuthUser.Videos.GetInternal(this.VideoID, callback);
        }
    }
}
