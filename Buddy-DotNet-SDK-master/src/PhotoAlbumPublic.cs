using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BuddyServiceClient;

namespace Buddy
{
    /// <summary>
    /// Represents a public photo album. Public albums are returned from album searches.
    /// <example>
    /// <code>
    ///     BuddyClient client = new BuddyClient("APPNAME", "APPPASS");
    ///     
    ///     client.LoginAsync((user, state) => {
    ///         user.SearchForAlbumsAsync((result, state2) => {});
    ///     }, "username", "password");
    /// </code>
    /// </example>
    /// </summary>
    public class PhotoAlbumPublic : BuddyBase
    {

        protected override bool AuthUserRequired
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the user ID of the user that owns this album.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets the name of the album
        /// </summary>
        public string AlbumName { get; protected set; }

        internal List<PicturePublic> pictures;

        /// <summary>
        /// Gets a list of pictures in this album.
        /// </summary>
        public ReadOnlyCollection<PicturePublic> Pictures { get; set; }

        internal PhotoAlbumPublic(BuddyClient client, AuthenticatedUser user, string albumName)
            : base(client, user)
        {
            this.AlbumName = albumName;
            this.UserId = user.ID;
            this.pictures = new List<PicturePublic>();
            this.Pictures = new ReadOnlyCollection<PicturePublic>(this.pictures);
        }

        internal PhotoAlbumPublic(BuddyClient client, int userId, string albumName): base(client)
        {

            this.AlbumName = albumName;
            this.UserId = userId;
            this.pictures = new List<PicturePublic>();
            this.Pictures = new ReadOnlyCollection<PicturePublic>(this.pictures);
        }
    }
}
