using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BuddyServiceClient;
using System.Globalization;

namespace Buddy
{
    /// <summary>
    /// Represents a single, named location in the Buddy system that's not a user. Locations are related to stores, hotels, parks, etc.
    /// <example>
    /// <code>
    ///     BuddyClient client = new BuddyClient("APPNAME", "APPPASS");
    ///     client.LoginAsync((user, state) => {
    ///         user.Places.Find((places, state2) => {  }, 1000000, 0.0, 0.0);
    ///     }, "username", "password");
    /// </code>
    /// </example>
    /// </summary>
    public class Place : BuddyBase
    {

        protected override bool AuthUserRequired {
            get {
                return true;
            }
        }

      

        /// <summary>
        /// Gets the address of the location.
        /// </summary>
        public string Address { get; protected set; }

        /// <summary>
        /// Gets the custom application tag data for the location.
        /// </summary>
        public string AppTagData { get; protected set; }

        /// <summary>
        /// Gets the category ID of the location (i.e. Hotels).
        /// </summary>
        public int CategoryID { get; protected set; }

        /// <summary>
        /// Gets the category name for the location.
        /// </summary>
        public string CategoryName { get; protected set; }

        /// <summary>
        /// Gets the city for the location.
        /// </summary>
        public string City { get; protected set; }

        /// <summary>
        /// Gets the date the location was created in the system.
        /// </summary>
        public DateTime CreatedDate { get; protected set; }

        /// <summary>
        /// If this user profile was returned from a search, gets the distance in kilometers from the search origin.
        /// </summary>
        public double DistanceInKilometers { get; protected set; }

        /// <summary>
        /// If this user profile was returned from a search, gets the distance in meters from the search origin.
        /// </summary>
        public double DistanceInMeters { get; protected set; }

        /// <summary>
        /// If this user profile was returned from a search, gets the distance in miles from the search origin.
        /// </summary>
        public double DistanceInMiles { get; protected set; }

        /// <summary>
        /// If this user profile was returned from a search, gets the distance in yards from the search origin.
        /// </summary>
        public double DistanceInYards { get; protected set; }

        /// <summary>
        /// Gets the fax number of the location.
        /// </summary>
        public string Fax { get; protected set; }

        /// <summary>
        /// Gets the globaly unique ID of the location.
        /// </summary>
        public int ID { get; protected set; }

        /// <summary>
        /// Gets the latitude of the location.
        /// </summary>
        public double Latitude { get; protected set; }

        /// <summary>
        /// Gets the longitude of the location.
        /// </summary>
        public double Longitude { get; protected set; }

        /// <summary>
        /// Gets the name of the location.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets the postal state of the location.
        /// </summary>
        public string PostalState { get; protected set; }

        /// <summary>
        /// Gets the postal ZIP of the location.
        /// </summary>
        public string PostalZip { get; protected set; }

        /// <summary>
        /// Gets the region of the location.
        /// </summary>
        public string Region { get; protected set; }

        /// <summary>
        /// Gets the ShortID of the location.
        /// </summary>
        public string ShortID { get; protected set; }

        /// <summary>
        /// Gets the telephone number of the location.
        /// </summary>
        public string Telephone { get; protected set; }

        /// <summary>
        /// Gets the last update date of the location.
        /// </summary>
        public DateTime TouchedDate { get; protected set; }

        /// <summary>
        /// Gets the user tag data of the location.
        /// </summary>
        public string UserTagData { get; protected set; }

        /// <summary>
        /// Gets the website of the location.
        /// </summary>
        public string Website { get; protected set; }

        internal Place (BuddyClient client, AuthenticatedUser user, InternalModels.DataContract_SearchPlaces place)
            : base(client, user)
        {


            this.Address = place.Address;
            this.AppTagData = place.AppTagData;
            this.CategoryID = Int32.Parse (place.CategoryID);
            this.CategoryName = place.CategoryName;
            this.City = place.City;
            this.CreatedDate = Convert.ToDateTime (place.CreatedDate, CultureInfo.InvariantCulture);
            this.DistanceInKilometers = Double.Parse (place.DistanceInKilometers, CultureInfo.InvariantCulture);
            this.DistanceInMeters = Double.Parse (place.DistanceInMeters, CultureInfo.InvariantCulture);
            this.DistanceInMiles = Double.Parse (place.DistanceInMiles, CultureInfo.InvariantCulture);
            this.DistanceInYards = Double.Parse (place.DistanceInYards, CultureInfo.InvariantCulture);
            this.Fax = place.Fax;
            this.ID = Int32.Parse (place.GeoID);
            this.Latitude = Double.Parse (place.Latitude, CultureInfo.InvariantCulture);
            this.Longitude = Double.Parse (place.Longitude, CultureInfo.InvariantCulture);
            this.Name = place.Name;
            this.PostalState = place.PostalState;
            this.PostalZip = place.PostalZip;
            this.Region = place.Region;
            this.ShortID = place.ShortID;
            this.Telephone = place.Telephone;
            this.TouchedDate = Convert.ToDateTime (place.TouchedDate, CultureInfo.InvariantCulture);
            this.UserTagData = place.UserTagData;
            this.Website = place.WebSite;
        }

        /// <summary>
        /// Set an application specific tag or a user tag for a place.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true on success, false otherwise.</param>
        /// <param name="appTag">The application level tag to set.</param>
        /// <param name="userTag">The user-level tag to set for this Place.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of SetTagAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult SetTagAsync (Action<bool, BuddyCallbackParams> callback, string appTag, string userTag, object state = null)
        {
            SetTagInternal (appTag, userTag, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void SetTagInternal (string appTag, string userTag, Action<BuddyCallResult<bool>> callback)
        {
            this.Client.Service.GeoLocation_Location_SetTag (this.Client.AppName, this.Client.AppPassword, AuthUser.Token,
                    ID.ToString (), appTag == null ? "" : appTag, userTag == null ? "" : userTag, (bcr) =>
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
    }
}
