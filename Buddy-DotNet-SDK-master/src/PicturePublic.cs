using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using BuddyServiceClient;
using System.Globalization;

namespace Buddy
{
    /// <summary>
    /// Represents a single picture on the Buddy Platform. This is a public view of a picture, can be retrieve either by getting a User's profile pictures or 
    /// by searching for albums.
    /// <example>
    /// <code>
    ///     BuddyClient client = new BuddyClient("APPNAME", "APPPASS");
    ///     
    ///     client.LoginAsync((user, state) => {
    ///         user.GetProfilePhotosAsync((pics, state2) => { });
    ///     }, "username", "password");
    /// </code>
    /// </example>
    /// </summary>
    public class PicturePublic : BuddyBase
    {

     
        /// <summary>
        /// Gets the url of the full picture.
        /// </summary>
        public string FullUrl { get; protected set; }

        /// <summary>
        /// Gets the url of the thumbnail of the picture.
        /// </summary>
        public string ThumbnailUrl { get; protected set; }

        /// <summary>
        /// Gets the latitude of the picture location.
        /// </summary>
        public double Latitude { get; protected set; }

        /// <summary>
        /// Gets the longitude of the picture location.
        /// </summary>
        public double Longitude { get; protected set; }

        /// <summary>
        /// Gets the optional comment of the picture.
        /// </summary>
        public string Comment { get; protected set; }

        /// <summary>
        /// Gets the optional application tag of the picture.
        /// </summary>
        public string AppTag { get; protected set; }

        /// <summary>
        /// Gets the date when this picture was added.
        /// </summary>
        public DateTime AddedOn { get; protected set; }

        /// <summary>
        /// Gets the system-wide ID of the picture.
        /// </summary>
        public int PhotoID { get; protected set; }

        /// <summary>
        /// If this picture was returned as part of an album search, gets the distance in kilometers from the location that was used as the origin of the search.
        /// </summary>
        public double DistanceInKilometers { get; protected set; }

        /// <summary>
        /// If this picture was returned as part of an album search, gets the distance in meters from the location that was used as the origin of the search.
        /// </summary>
        public double DistanceInMeters { get; protected set; }

        /// <summary>
        /// If this picture was returned as part of an album search, gets the distance in miles from the location that was used as the origin of the search.
        /// </summary>
        public double DistanceInMiles { get; protected set; }

        /// <summary>
        /// If this picture was returned as part of an album search, gets the distance in yards from the location that was used as the origin of the search.
        /// </summary>
        public double DistanceInYards { get; protected set; }

        protected User User { get; private set; }
        protected int UserId { get; private set; }

        internal PicturePublic(BuddyClient client, string fullUrl, string thumbnailUrl, double latitude, double longitude, string comment, string appTag,
                       DateTime addedOn, int photoId, User user)
            : base(client, user as AuthenticatedUser)
        {
            this.FullUrl = fullUrl;
            this.ThumbnailUrl = thumbnailUrl;
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Comment = comment;
            this.AppTag = appTag;
            this.AddedOn = addedOn;
            this.PhotoID = photoId;
            this.User = user;

        }

        internal PicturePublic(BuddyClient client, User user, InternalModels.DataContract_PublicPhotoSearch photo, int userId)
            : base(client)
        {
            if (photo == null) throw new ArgumentNullException("photo");

            this.FullUrl = photo.FullPhotoURL;
            this.ThumbnailUrl = photo.ThumbnailPhotoURL;
            this.Latitude = this.Client.TryParseDouble(photo.Latitude);
            this.Longitude = this.Client.TryParseDouble(photo.Longitude);
            this.AddedOn = Convert.ToDateTime(photo.PhotoAdded, CultureInfo.InvariantCulture);
            this.AppTag = photo.ApplicationTag;
            this.PhotoID = Int32.Parse(photo.PhotoID);
            this.User = user;
            this.UserId = userId;

            this.DistanceInKilometers = client.TryParseDouble(photo.DistanceInKilometers);
            this.DistanceInMeters = client.TryParseDouble(photo.DistanceInMeters);
            this.DistanceInMiles = client.TryParseDouble(photo.DistanceInMiles);
            this.DistanceInYards = client.TryParseDouble(photo.DistanceInYards);
        }

        internal PicturePublic(BuddyClient client, InternalModels.DataContract_VirtualPhotoList photo)
            : base(client)
        {
            if (photo == null) throw new ArgumentNullException("photo");


            this.FullUrl = photo.FullPhotoURL;
            this.ThumbnailUrl = photo.ThumbnailPhotoURL;
            this.Latitude = this.Client.TryParseDouble(photo.Latitude);
            this.Longitude = this.Client.TryParseDouble(photo.Longitude);
            this.AddedOn = photo.AddedDateTime;
            this.AppTag = photo.ApplicationTag;
            this.PhotoID = Int32.Parse(photo.PhotoID);
            this.UserId = Int32.Parse(photo.UserID);

        }
    }
}
