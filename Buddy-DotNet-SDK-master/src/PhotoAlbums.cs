using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using BuddyServiceClient;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Buddy
{
    /// <summary>
    /// Represents a object that can be used to interact with an AuthenticatedUser's photo albums.
    /// <example>
    /// <code>
    ///     BuddyClient client = new BuddyClient("APPNAME", "APPPASS");
    ///     
    ///     client.LoginAsync((user, state) => {
    ///         user.PhotoAlbums.GetAllAsync((albums, state2) => { });
    ///     }, "username", "password");
    /// </code>
    /// </example>
    /// </summary>
    public class PhotoAlbums : BuddyBase
    {


        internal PhotoAlbums (BuddyClient client, AuthenticatedUser user)
            : base(client, user)
        {

        }

     

        /// <summary>
        /// This method is used create a new album. The album will be owned by this user. Multiple albums can be created with the same name. Note that this method internally does two web-service calls, and the IAsyncResult object
        /// returned is only valid for the first one.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is the new photo album that was created, null if an error occured.</param>
        /// <param name="name">The name of the new album.</param>
        /// <param name="isPublic">Make the album publicly visible to other users.</param>
        /// <param name="appTag">Optionally add a custom application tag for this user.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of CreateAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult CreateAsync (Action<PhotoAlbum, BuddyCallbackParams> callback, string name, bool isPublic = false, string appTag = "", object state = null)
        {
            CreateInternal (name, isPublic, appTag, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void CreateInternal (string name, bool isPublic, string appTag, Action<BuddyCallResult<PhotoAlbum>> callback)
        {
            this.Client.Service.Pictures_PhotoAlbum_Create (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token,
                        name, isPublic ? "1" : "0", appTag, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<PhotoAlbum> (null, bcr.Error));
                    return;
                }
                GetInternal (Int32.Parse (result), callback);


            });
            return;
        }

        /// <summary>
        /// Get a photo album by ID. This album doesn't need to be owned by this user.
        /// </summary>
        /// <param name="albumId">The ID of the album.</param>
        /// <param name="callback">The async callback to call on success or error. The first parameter is the photo album if successful, null otherwise.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetAsync (Action<PhotoAlbum, BuddyCallbackParams> callback, int albumId, object state = null)
        {
            GetInternal (albumId, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void GetInternal (int albumId, Action<BuddyCallResult<PhotoAlbum>> callback)
        {
            this.Client.Service.Pictures_PhotoAlbum_Get (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token,
                        this.AuthUser.ID.ToString (), albumId.ToString (), (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<PhotoAlbum> (null, bcr.Error));
                    return;
                }
                PhotoAlbum album = new PhotoAlbum (this.Client, this.AuthUser, albumId);
                foreach (var d in result)
                    album.pictures.Add (new Picture (this.Client, this.AuthUser, d));
                {
                    callback (BuddyResultCreator.Create (album, bcr.Error));
                    return; }
                ;
            });
            return;
        }



        /// <summary>
        /// Get a photo album by its name. Note that there can be more than one album with the same name. This method will only return the first one.
        /// Call PhotoAlbums.All to get all the albums.
        /// </summary>
        /// <param name="albumName">The name of the albul to retrieve. Can't be null or empty.</param>
        /// <param name="callback">The async callback to call on success or error. The first parameter is the PhotoAlbum if found, null otherwise.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetAsync (Action<PhotoAlbum, BuddyCallbackParams> callback, string albumName, object state = null)
        {
            GetInternal (albumName, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void GetInternal (string albumName, Action<BuddyCallResult<PhotoAlbum>> callback)
        {
            if (String.IsNullOrEmpty (albumName))
                throw new ArgumentNullException ("albumName");

            this.Client.Service.Pictures_PhotoAlbum_GetFromAlbumName (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token,
                        this.AuthUser.ID.ToString (), albumName.ToString (), (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<PhotoAlbum> (null, bcr.Error));
                    return;
                }
                if (result.Length == 0) {
                    callback (BuddyResultCreator.Create<PhotoAlbum> (null, bcr.Error));
                    return;
                }
                ;

                GetInternal (Int32.Parse (result [0].AlbumID), callback);


            });
            return;
        }

     

        /// <summary>
        /// Return all photo albums for this user. Note that this can be an expensive operation since all the Picture data is retrieved as well.
        /// </summary>
        /// <param name="afterDate">Optionally return all albums created after a date.</param>
        /// <param name="callback">The async callback to call on success or error. The first parameter is a list of photo albums that this user owns.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetAllAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetAllAsync (Action<List<PhotoAlbum>, BuddyCallbackParams> callback, DateTime afterDate = default(DateTime), object state = null)
        {
            GetAllInternal (afterDate, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void GetAllInternal (DateTime afterDate, Action<BuddyCallResult<List<PhotoAlbum>>> callback)
        {
            this.Client.Service.Pictures_PhotoAlbum_GetAllPictures (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token,
                        this.AuthUser.ID.ToString (), afterDate == DateTime.MinValue ? "1/1/1950" : afterDate.ToString (CultureInfo.InvariantCulture), (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<List<PhotoAlbum>> (null, bcr.Error));
                    return;
                }
                Dictionary<string, PhotoAlbum> dict = new Dictionary<string, PhotoAlbum> ();
                foreach (var d in result) {
                    if (!dict.ContainsKey (d.AlbumID))
                        dict.Add (d.AlbumID, new PhotoAlbum (this.Client, this.AuthUser, Int32.Parse (d.AlbumID)));
                    dict [d.AlbumID].pictures.Add (new Picture (this.Client, d.FullPhotoURL, d.ThumbnailPhotoURL,
                                    this.Client.TryParseDouble (d.Latitude), this.Client.TryParseDouble (d.Longitude), d.PhotoComment,
                                    d.ApplicationTag, d.AddedDateTime, Int32.Parse (d.PhotoID), this.AuthUser));
                }
                ;

                {
                    callback (BuddyResultCreator.Create (dict.Values.ToList (), bcr.Error));
                    return; }
                ;
            });
            return;
        }

#if AWAIT_SUPPORTED


        /// <summary>
        /// This method is used create a new album. The album will be owned by this user. Multiple albums can be created with the same name. Note that this method internally does two web-service calls, and the IAsyncResult object
        /// returned is only valid for the first one.
        /// </summary>
        /// <param name="name">The name of the new album.</param>
        /// <param name="isPublic">Make the album publicly visible to other users.</param>
        /// <param name="appTag">Optionally add a custom application tag for this user.</param>
        /// <returns>A Task&lt;PhotoAlbum&gt;that can be used to monitor progress on this call.</returns>
        public System.Threading.Tasks.Task<PhotoAlbum> CreateAsync( string name, bool isPublic = false, string appTag = "")
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<PhotoAlbum>();
            CreateInternal(name, isPublic, appTag, (bcr) =>
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
        /// Get a photo album by ID. This album doesn't need to be owned by this user.
        /// </summary>
        /// <param name="albumId">The ID of the album.</param>
        /// <returns>A Task&lt;PhotoAlbum&gt;that can be used to monitor progress on this call.</returns>
        public System.Threading.Tasks.Task<PhotoAlbum> GetAsync( int albumId)
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<PhotoAlbum>();
            GetInternal(albumId, (bcr) =>
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
        /// Get a photo album by its name. Note that there can be more than one album with the same name. This method will only return the first one.
        /// Call PhotoAlbums.All to get all the albums.
        /// </summary>
        /// <param name="albumName">The name of the albul to retrieve. Can't be null or empty.</param>
        /// <returns>A Task&lt;PhotoAlbum&gt;that can be used to monitor progress on this call.</returns>
        public System.Threading.Tasks.Task<PhotoAlbum> GetAsync( string albumName)
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<PhotoAlbum>();
            GetInternal(albumName, (bcr) =>
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
        /// Return all photo albums for this user. Note that this can be an expensive operation since all the Picture data is retrieved as well.
        /// </summary>
        /// <param name="afterDate">Optionally return all albums created after a date.</param>
        /// <returns>A Task&lt;IEnumerable&lt;PhotoAlbum&gt; &gt;that can be used to monitor progress on this call.</returns>
        public System.Threading.Tasks.Task<IEnumerable<PhotoAlbum>> GetAllAsync( System.DateTime afterDate = default(DateTime))
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<IEnumerable<PhotoAlbum>>();
            GetAllInternal(afterDate, (bcr) =>
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
