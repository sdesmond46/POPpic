using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using BuddyServiceClient;
using System.Globalization;

namespace Buddy
{
    /// <summary>
    /// Represents a single virtual album. Unlike normal photoalbums any user may add existing photos to a virtual album. 
    /// Only the owner of the virtual album can delete the album however.
    /// </summary>
    public class VirtualAlbum : BuddyBase
    {
        internal List<PicturePublic> pictures;

        /// <summary>
        /// Gets a readonly collection of pictures in this album. Use the AddPicture method to add more pictures to the album or the Picture.Delete method
        /// to remove them.
        /// </summary>
        public ReadOnlyCollection<PicturePublic> Pictures { get; set; }

        /// <summary>
        /// Gets the globally unique ID of the virtual album.
        /// </summary>
        public int ID { get; protected set; }

        /// <summary>
        /// Gets the name of the virtual album.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets the thumbnail for the virtual album.
        /// </summary>
        public string ThumbnailUrl { get; protected set; }

        /// <summary>
        /// Gets the user ID of the owner of this virtual album.
        /// </summary>
        public int OwnerUserId { get; protected set; }

        /// <summary>
        /// Gets the optional application tag for this virtual album.
        /// </summary>
        public string ApplicationTag { get; protected set; }

        /// <summary>
        /// Gets the date this virtual album was created.
        /// </summary>
        public DateTime CreatedOn { get; protected set; }

        /// <summary>
        /// Gets the date this virtual album was last updated.
        /// </summary>
        public DateTime LastUpdated { get; protected set; }

        internal VirtualAlbum(BuddyClient client, AuthenticatedUser user, InternalModels.DataContract_VirtualPhotoAlbumInformation info)
            : base(client, user)
        {
            this.pictures = new List<PicturePublic> ();
            this.Pictures = new ReadOnlyCollection<PicturePublic> (this.pictures);

            this.ID = Int32.Parse (info.VirtualAlbumID);
            this.ApplicationTag = info.ApplicationTag;
            this.CreatedOn = Convert.ToDateTime (info.CreatedDateTime, CultureInfo.InvariantCulture);
            this.LastUpdated = Convert.ToDateTime (info.LastUpdatedDateTime, CultureInfo.InvariantCulture);
            this.Name = info.PhotoAlbumName;
            this.OwnerUserId = Int32.Parse (info.UserID);
            this.ThumbnailUrl = info.PhotoAlbumThumbnail;
        }

        internal void AddPictures(IEnumerable<InternalModels.DataContract_VirtualPhotoList> virtualPhotoList)
        {
            foreach (var d in virtualPhotoList)
                pictures.Add (new PicturePublic (this.Client, d));
        }

      

        /// <summary>
        /// Delete this virtual album.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true on success, false otherwise.</param>
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
            this.Client.Service.Pictures_VirtualAlbum_DeleteAlbum (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token,
                        this.ID.ToString (), (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None && bcr.Error != BuddyError.ServiceErrorNegativeOne) {
                    callback (BuddyResultCreator.Create (default(bool), bcr.Error));
                    return;
                }
                callback (BuddyResultCreator.Create (result == "1", BuddyError.None));
            });
            return;
        }


        /// <summary>
        /// Add an existing (uploaded) photo to a virtual album. This photo can be either private or public (either PicturePublic and Picture will work).
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true on success, false otherwise.</param>
        /// <param name="picture">The picture to add to the virtual albums. Either PicturePublic or Picture works.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of AddPictureAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult AddPictureAsync (Action<long, BuddyCallbackParams> callback, PicturePublic picture, object state = null)
        {
            AddPictureInternal (picture, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void AddPictureInternal (PicturePublic picture, Action<BuddyCallResult<long>> callback)
        {
            if (picture == null)
                throw new ArgumentNullException ("picture");

            this.Client.Service.Pictures_VirtualAlbum_AddPhoto (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token, this.ID.ToString (),
                        picture.PhotoID.ToString (), (bcr) =>
            {
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create ((long)-1, bcr.Error));
                    return;
                }
                {
                    callback (BuddyResultCreator.Create (long.Parse(bcr.Result), bcr.Error));
                    return; }
                ;
            });
            return;
        }


        /// <summary>
        /// Add a list of pictures to this virtual album.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true on success, false otherwise.</param>
        /// <param name="pictures">The list of pictures to add to this photo album. Either PicturePublic or Picture works.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of AddPictureBatchAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult AddPictureBatchAsync (Action<bool, BuddyCallbackParams> callback, List<PicturePublic> pictures, object state = null)
        {
            AddPictureBatchInternal (pictures, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void AddPictureBatchInternal (List<PicturePublic> pictures, Action<BuddyCallResult<bool>> callback)
        {
            if (pictures == null || pictures.Count == 0)
                throw new ArgumentException ("Can't be null or empty.", "pictures");

            string batch = "";
            foreach (var p in pictures)
                batch += p.PhotoID + ";";
            batch = batch.Substring (0, batch.Length - 1);

            this.Client.Service.Pictures_VirtualAlbum_AddPhotoBatch (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token, this.ID.ToString (),
                        batch, (bcr) =>
            {
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create (default(bool), bcr.Error));
                    return;
                }
                {
                    callback (BuddyResultCreator.Create (true, bcr.Error));
                    return; }
                ;
            });
            return;
        }

        /// <summary>
        /// Remove a picture from this virtual album.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true on success, false otherwise.</param>
        /// <param name="picture">The picture to remove from the album. Either PicturePublic or Picture works.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of RemovePictureAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult RemovePictureAsync (Action<bool, BuddyCallbackParams> callback, PicturePublic picture, object state = null)
        {
            RemovePictureInternal (picture, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void RemovePictureInternal (PicturePublic picture, Action<BuddyCallResult<bool>> callback)
        {
            if (picture == null)
                throw new ArgumentNullException ("picture");

            this.Client.Service.Pictures_VirtualAlbum_RemovePhoto (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token, this.ID.ToString (),
                        picture.PhotoID.ToString (), (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None && bcr.Error != BuddyError.ServiceErrorNegativeOne) {
                    callback (BuddyResultCreator.Create (default(bool), bcr.Error));
                    return;
                }
                callback (BuddyResultCreator.Create (result == "1", BuddyError.None));
            });
            return;
        }

      

        /// <summary>
        /// Update this virtul albums name and app.tag
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true on success, false otherwise.</param>
        /// <param name="newName">The new name for the album.</param>
        /// <param name="newAppTag">An optional new application tag for the album.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of UpdateAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult UpdateAsync (Action<bool, BuddyCallbackParams> callback, string newName, string newAppTag = "", object state = null)
        {
            UpdateInternal (newName, newAppTag, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void UpdateInternal (string newName, string newAppTag, Action<BuddyCallResult<bool>> callback)
        {
            if (String.IsNullOrEmpty (newName))
                throw new ArgumentException ("Can't be null or empty.", "newName");

            this.Client.Service.Pictures_VirtualAlbum_Update (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token, this.ID.ToString (),
                        newName, newAppTag == null ? "" : newAppTag, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None && bcr.Error != BuddyError.ServiceErrorNegativeOne) {
                    callback (BuddyResultCreator.Create (default(bool), bcr.Error));
                    return;
                }
                callback (BuddyResultCreator.Create (result == "1", BuddyError.None));
            });
            return;
        }

        /// <summary>
        /// Update virtual album picture comment or app.tag.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true on success, false otherwise.</param>
        /// <param name="picture">The picture to be updated, either PicturePublic or Picture works.</param>
        /// <param name="newComment">The new comment to set for the picture.</param>
        /// <param name="newAppTag">An optional new application tag for the picture.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of UpdatePictureAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult UpdatePictureAsync (Action<bool, BuddyCallbackParams> callback, PicturePublic picture, string newComment, string newAppTag = "", object state = null)
        {
            UpdatePictureInternal (picture, newComment, newAppTag, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void UpdatePictureInternal (PicturePublic picture, string newComment, string newAppTag, Action<BuddyCallResult<bool>> callback)
        {
            if (picture == null)
                throw new ArgumentNullException ("picture");

            this.Client.Service.Pictures_VirtualAlbum_UpdatePhoto (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token, picture.PhotoID.ToString (),
                        newComment == null ? "" : newComment, newAppTag == null ? "" : newAppTag, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None && bcr.Error != BuddyError.ServiceErrorNegativeOne) {
                    callback (BuddyResultCreator.Create (default(bool), bcr.Error));
                    return;
                }
                callback (BuddyResultCreator.Create (result == "1", BuddyError.None));
            });
            return;
        }
    }
}
