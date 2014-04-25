using BuddyServiceClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Buddy
{
    public class Blob : BuddyBase
    {
        protected override bool AuthUserRequired
        {
            get{ return true; }
        }

        /// <summary>
        /// Gets the ID of the Blob.
        /// </summary>
        public long BlobID { get; protected set; }

        /// <summary>
        /// Gets the human friendly name of the Blob.
        /// </summary>
        public string FriendlyName { get; protected set; }

        /// <summary>
        /// Gets the MIMEType of the Blob.
        /// </summary>
        public string MimeType { get; protected set;}

        /// <summary>
        /// Gets the size of the Blob in bytes.
        /// </summary>
        public int FileSize { get; protected set; }

        /// <summary>
        /// Gets the optional application tag for the Blob.
        /// </summary>
        public string AppTag { get; protected set; }

        /// <summary>
        /// Gets the UserID of the user that uploaded this Blob.
        /// </summary>
        public long Owner { get; protected set; }
        
        /// <summary>
        /// Gets the latitude where the Blob was created.
        /// </summary>
        public double Latitude { get; protected set; }

        /// <summary>
        /// Gets the longitude where the Blob was created.
        /// </summary>
        public double Longitude { get; protected set; }

        /// <summary>
        /// Gets the date the Blob was uploaded.
        /// </summary>
        public DateTime UploadDate { get; protected set; }

        /// <summary>
        /// Gets the date the Blob was last touched (uploaded or edited)
        /// </summary>
        public DateTime LastTouchDate { get; protected set; }

        internal Blob(BuddyClient client, AuthenticatedUser user, InternalModels.DataContract_Blob blob) : base(client, user)
        {
            this.BlobID = long.Parse(blob.BlobID, CultureInfo.InvariantCulture);
            this.FriendlyName = blob.FriendlyName;
            this.MimeType = blob.MimeType;
            this.FileSize = int.Parse(blob.FileSize, CultureInfo.InvariantCulture);
            this.AppTag = blob.AppTag;
            this.Owner = long.Parse(blob.Owner, CultureInfo.InvariantCulture);
            this.Latitude = double.Parse(blob.Latitude, CultureInfo.InvariantCulture);
            this.Longitude = double.Parse(blob.Longitude, CultureInfo.InvariantCulture);
            this.UploadDate = blob.UploadDate;
            this.LastTouchDate = blob.LastTouchDate;
        }

        /// <summary>
        /// Edits the information related to this Blob.
        /// </summary>
        /// <param name="friendlyName">The new human friendly name for the Blob. Leave null or empty to not change.</param>
        /// <param name="appTag">The new AppTag for the Blob. Leave null or empty to not change.</param>
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
        /// Gets a Stream of this Blob.
        /// </summary>
        /// <returns>A Task&lt;Stream&gt;that can be used to monitor progress on this call.</returns>
        public Task<Stream> GetAsync()
        {
            return this.AuthUser.Blobs.GetAsync(this.BlobID);
        }

        /// <summary>
        /// Delete this Blob.
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

        internal void  EditInfoInternal(string friendlyName, string appTag,
            Action<BuddyCallResult<bool>> callback)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("BuddyApplicationName", this.Client.AppName);
            parameters.Add("BuddyApplicationPassword", this.Client.AppPassword);
            parameters.Add("UserToken", this.AuthUser.Token);
            parameters.Add("BlobID", this.BlobID);
            parameters.Add("FriendlyName", friendlyName);
            parameters.Add("AppTag", appTag);

            this.Client.Service.CallMethodAsync<string>("Blobs_Blob_EditInfo", parameters, (bcr) =>
            {
                this.Client.Service.CallOnUiThread((state) => 
                    callback(BuddyResultCreator.Create(bcr.Result == "1", bcr.Error)));
            });
        }

        internal void DeleteInternal(Action<BuddyCallResult<bool>> callback)
        {
            this.AuthUser.Blobs.DeleteInternal(this.BlobID, callback);
        }

        internal void GetInternal(Action<BuddyCallResult<Stream>> callback)
        {
            this.AuthUser.Blobs.GetInternal(this.BlobID, callback);
        }
    }
}
