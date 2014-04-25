using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BuddyServiceClient;
using System.Globalization;
using System.IO;

namespace Buddy
{
    /// <summary>
    /// Represent a single Buddy photo album. Albums are collections of photo that can be manipulated by their owner (the user that created the album). Albums
    /// can be public in which case other users can see them (but no modify).
    /// <example>
    /// <code>
    ///     BuddyClient client = new BuddyClient("APPNAME", "APPPASS");
    ///     
    ///     client.LoginAsync((user, state) => {
    ///         user.PhotoAlbums["test album"].AddPicture(pictureBineryData);
    ///     }, "username", "password");
    /// </code>
    /// </example>
    /// </summary>
    public class PhotoAlbum : PhotoAlbumPublic
    {
        internal new List<Picture> pictures;

       

        /// <summary>
        /// Gets a readonly collection of pictures in this album. Use the AddPicture method to add more pictures to the album or the Picture.Delete method
        /// to remove them.
        /// </summary>
        public new ReadOnlyCollection<Picture> Pictures { get; set; }

        /// <summary>
        /// Gets the global unique album ID.
        /// </summary>
        public int AlbumId { get; protected set; }

        internal PhotoAlbum (BuddyClient client, AuthenticatedUser user, int albumId)
            : base(client, user, null)
        {
            this.AlbumId = albumId;
            this.pictures = new List<Picture> ();
            this.Pictures = new ReadOnlyCollection<Picture> (this.pictures);
        }

        /// <summary>
        /// Delete this photo album.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true if successful, false otherwise.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of DeleteAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult DeleteAsync (Action<bool, BuddyCallbackParams> callback, object state = null)
        {
            DeleteInternal ((bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void DeleteInternal (Action<BuddyCallResult<bool>> callback)
        {
            this.Client.Service.Pictures_PhotoAlbum_Delete (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token,
                        this.AlbumId.ToString (), (bcr) =>
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
        /// Add a new picture to this album. Note that this method internally does two web-service calls, and the IAsyncResult object
        /// returned is only valid for the first one.
        /// </summary>
        /// <param name="blob">The image byte array of the picture.</param>
        /// <param name="comment">An optional comment for this picture.</param>
        /// <param name="latitude">An optional latitude for the picture.</param>
        /// <param name="longitude">An optional longitude for the picture.</param>
        /// <param name="appTag">An optional application tag.</param>
        /// <param name="callback">The async callback to call on success or error. The first parameter is the new picture that was added or null on error.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of AddPictureAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult AddPictureAsync (Action<Picture, BuddyCallbackParams> callback, byte[] blob, string comment = "", double latitude = 0.0, double longitude = 0.0, string appTag = "", object state = null)
        {
            AddPictureInternal (new MemoryStream(blob), comment, latitude, longitude, appTag, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void AddPictureInternal (Stream blob, string comment, double latitude, double longitude, string appTag, Action<BuddyCallResult<Picture>> callback)
        {
            if (blob == null || blob.Length == 0)
                throw new ArgumentException ("Can't be null or empty.", "blob");

            this.Client.Service.Pictures_Photo_Add (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token, blob, this.AlbumId.ToString (),
                        comment, latitude.ToString (CultureInfo.InvariantCulture), longitude.ToString (CultureInfo.InvariantCulture), appTag, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<Picture> (null, bcr.Error));
                    return;
                }
                this.AuthUser.GetPictureInternal (Int32.Parse (result), callback);


            });
            return;
        }


      

        /// <summary>
        /// Add a new picture to this album. Note that this method internally does two web-service calls, and the IAsyncResult object
        /// returned is only valid for the first one.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is the new picture that was added or null on error.</param>
        /// <param name="blob">The image byte array of the picture.</param>
        /// <param name="comment">An optional comment for this picture.</param>
        /// <param name="latitude">An optional latitude for the picture.</param>
        /// <param name="longitude">An optional longitude for the picture.</param>
        /// <param name="appTag">An optional application tag.</param>
        /// <param name="watermarkmessage">An optional message to watermark the image with.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of AddPictureWithWatermarkAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult AddPictureWithWatermarkAsync (Action<Picture, BuddyCallbackParams> callback, byte[] blob, string comment = "", double latitude = 0.0, double longitude = 0.0, string appTag = "", string watermarkmessage = "", object state = null)
        {
            AddPictureWithWatermarkInternal (new MemoryStream(blob), comment, latitude, longitude, appTag, watermarkmessage, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void AddPictureWithWatermarkInternal (Stream photoStream, string comment, double latitude, double longitude, string appTag, string watermarkmessage, Action<BuddyCallResult<Picture>> callback)
        {
            if (photoStream == null || photoStream.Length == 0)
                throw new ArgumentException ("Can't be null or empty.", "blob");

            this.Client.Service.Pictures_Photo_AddWithWatermark (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token, photoStream, this.AlbumId.ToString (), comment, latitude.ToString (CultureInfo.InvariantCulture), longitude.ToString (CultureInfo.InvariantCulture), appTag, watermarkmessage, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<Picture> (null, bcr.Error));
                    return;
                }
                this.AuthUser.GetPictureInternal (Int32.Parse (result), callback);

                {
                    callback (BuddyResultCreator.Create<Picture> (null, bcr.Error));
                    return; }
                ;
            });
            return;
        }

#if AWAIT_SUPPORTED
        [Obsolete("Please use the overload of AddPictureWithWatermark that takes a Stream instead of a byte[]")]
        public  System.Threading.Tasks.Task<Buddy.Picture> AddPictureWithWatermark(byte[] blob, string comment = "", double latitude = 0, double longitude = 0, string appTag = "", string watermarkmessage = "") {
           return this.AddPictureWithWatermarkAsync(new MemoryStream(blob), comment, latitude, longitude, appTag, watermarkmessage);
        }
         [Obsolete("Please use the overload of AddPicture that takes a Stream instead of a byte[]")]
         public System.Threading.Tasks.Task<Buddy.Picture> AddPicture(byte[] blob, string comment = "", double latitude = 0, double longitude = 0, string appTag = "") {
            return this.AddPictureAsync(new MemoryStream(blob), comment, latitude, longitude, appTag);
        }


         /// <summary>
         /// Delete this photo album.
         /// </summary>
         /// <returns>A Task&lt;Boolean&gt;that can be used to monitor progress on this call.</returns>
         public System.Threading.Tasks.Task<Boolean> DeleteAsync()
         {
             var tcs = new System.Threading.Tasks.TaskCompletionSource<Boolean>();
             DeleteInternal((bcr) =>
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
         /// Add a new picture to this album. Note that this method internally does two web-service calls, and the IAsyncResult object
         /// returned is only valid for the first one.
         /// </summary>
         /// <param name="photoStream">A stream containing the photo's contents..</param>
         /// <param name="comment">An optional comment for this picture.</param>
         /// <param name="latitude">An optional latitude for the picture.</param>
         /// <param name="longitude">An optional longitude for the picture.</param>
         /// <param name="appTag">An optional application tag.</param>
         /// <returns>A Task&lt;Picture&gt;that can be used to monitor progress on this call.</returns>
         public System.Threading.Tasks.Task<Picture> AddPictureAsync(Stream photoStream, string comment = "", double latitude = 0, double longitude = 0, string appTag = "")
         {
             var tcs = new System.Threading.Tasks.TaskCompletionSource<Picture>();
             AddPictureInternal(photoStream, comment, latitude, longitude, appTag, (bcr) =>
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
         /// Add a new picture to this album. Note that this method internally does two web-service calls, and the IAsyncResult object
         /// returned is only valid for the first one.
         /// </summary>
         /// <param name="photoStream">A stream containing the photo's contents..</param>
         /// <param name="comment">An optional comment for this picture.</param>
         /// <param name="latitude">An optional latitude for the picture.</param>
         /// <param name="longitude">An optional longitude for the picture.</param>
         /// <param name="appTag">An optional application tag.</param>
         /// <param name="watermarkmessage">An optional message to watermark the image with.</param>
         /// <returns>A Task&lt;Picture&gt;that can be used to monitor progress on this call.</returns>
         public System.Threading.Tasks.Task<Picture> AddPictureWithWatermarkAsync(Stream photoStream, string comment = "", double latitude = 0, double longitude = 0, string appTag = "", string watermarkmessage = "")
         {
             var tcs = new System.Threading.Tasks.TaskCompletionSource<Picture>();
             AddPictureWithWatermarkInternal(photoStream, comment, latitude, longitude, appTag, watermarkmessage, (bcr) =>
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
