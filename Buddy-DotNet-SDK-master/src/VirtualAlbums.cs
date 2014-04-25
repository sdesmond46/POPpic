using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using BuddyServiceClient;

namespace Buddy
{
    /// <summary>
    /// Represents a class that can be used to interact with virtual albums. Unlike normal photoalbums any user may add existing photos to a virtual album. 
    /// Only the owner of the virtual album can delete the album however.
    /// </summary>
    public class VirtualAlbums : BuddyBase
    {


        internal VirtualAlbums (BuddyClient client, AuthenticatedUser user)
            : base(client, user)
        {

        }

        /// <summary>
        /// Create a new virtual album. Note that this method internally does two web-service calls, and the IAsyncResult object
        /// returned is only valid for the first one.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is the new album.</param>
        /// <param name="name">The name of the new virtual album.</param>
        /// <param name="appTag">An optional application tag for the album.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of CreateAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult CreateAsync (Action<VirtualAlbum, BuddyCallbackParams> callback, string name, string appTag = "", object state = null)
        {
            CreateInternal (name, appTag, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void CreateInternal (string name, string appTag, Action<BuddyCallResult<VirtualAlbum>> callback)
        {
            this.Client.Service.Pictures_VirtualAlbum_Create (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token,
                        name, appTag, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<VirtualAlbum> (null, bcr.Error));
                    return;
                }


                GetInternal (Int32.Parse (result), callback);


            });
            return;
        }

     
        /// <summary>
        /// Get a virtual album by its globally unique identifier. All the album photos will be retreived as well. Note that this method internally does two web-service calls, and the IAsyncResult object
        /// returned is only valid for the first one.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is the album.</param>
        /// <param name="albumId">The ID of the virtual album to retrieve.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetAsync (Action<VirtualAlbum, BuddyCallbackParams> callback, int albumId, object state = null)
        {
            GetInternal (albumId, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void GetInternal (int albumId, Action<BuddyCallResult<VirtualAlbum>> callback)
        {
            this.Client.Service.Pictures_VirtualAlbum_GetAlbumInformation (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token, albumId.ToString (), (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<VirtualAlbum> (null, bcr.Error));
                    return;
                }
                if (result.Length == 0) {
                    callback (BuddyResultCreator.Create<VirtualAlbum> (null, bcr.Error));
                    return;
                }
                ;



                GetPicturesInternal (albumId, result [0], callback);


            });
            return;
        }

        internal void GetPicturesInternal(int albumId, InternalModels.DataContract_VirtualPhotoAlbumInformation info, Action<BuddyCallResult<VirtualAlbum>> callback)
        {
            this.Client.Service.Pictures_VirtualAlbum_Get (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token, albumId.ToString (), (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<VirtualAlbum> (null, bcr.Error));
                    return;
                }
                VirtualAlbum album = new VirtualAlbum (this.Client, this.AuthUser, info);
                album.AddPictures (result);
                {
                    callback (BuddyResultCreator.Create (album, bcr.Error));
                    return; }
                ;
            });
            return;
        }


        /// <summary>
        /// Get the IDs of all the virtual albums that this user owns.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is a list of album IDs that this user owns.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetMyAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetMyAsync (Action<List<int>, BuddyCallbackParams> callback, object state = null)
        {
            GetMyInternal ((bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void GetMyInternal (Action<BuddyCallResult<List<int>>> callback)
        {
            this.Client.Service.Pictures_VirtualAlbum_GetMyAlbums (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<List<int>> (null, bcr.Error));
                    return;
                }
                List<int> albums = new List<int> ();
                foreach (var d in result)
                    albums.Add (Int32.Parse (d.VirtualAlbumID));
                {
                    callback (BuddyResultCreator.Create (albums, bcr.Error));
                    return; }
                ;
            });
            return;
        }
    }
}
