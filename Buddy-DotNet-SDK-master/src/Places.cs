using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BuddyServiceClient;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Buddy
{
    /// <summary>
    /// Represents an object that can be used to search for physical locations around the user.
    /// <example>
    /// <code>
    ///     BuddyClient client = new BuddyClient("APPNAME", "APPPASS");
    ///     client.LoginAsync((user, state) => {
    ///         user.Places.Find((places, state2) => {  }, 1000000, 0.0, 0.0);
    ///     }, "username", "password");
    /// </code>
    /// </example>
    /// </summary>
    public class Places : BuddyBase
    {


        internal Places (BuddyClient client, AuthenticatedUser user)
            : base(client, user)
        {

        }

     

        /// <summary>
        /// Find a location close to a given latitude and logitude.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is a list of locations that were found.</param>
        /// <param name="searchDistanceInMeters">The radius of the location search.</param>
        /// <param name="latitude">The latitude where the search should start.</param>
        /// <param name="longitude">The longitude where the search should start.</param>
        /// <param name="numberOfResults">Optional number of result to return, defaults to 10.</param>
        /// <param name="searchForName">Optional search string, for example: "Star*" to search for all place that start with the string "Star"</param>
        /// <param name="searchCategoryId">Optional search category ID to narrow down the search with.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of FindAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult FindAsync (Action<List<Place>, BuddyCallbackParams> callback, int searchDistanceInMeters, double latitude, double longitude, int numberOfResults = 10, string searchForName = "", int searchCategoryId = -1, object state = null)
        {
            FindInternal (searchDistanceInMeters, latitude, longitude, numberOfResults, searchForName, searchCategoryId, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void FindInternal (int searchDistanceInMeters, double latitude, double longitude, int numberOfResults, string searchForName, int searchCategoryId, Action<BuddyCallResult<List<Place>>> callback)
        {
            if (searchDistanceInMeters <= 0)
                throw new ArgumentException ("Can't be smaller or equal to zero.", "searchDistanceInMeters");
            if (latitude > 90.0 || latitude < -90.0)
                throw new ArgumentException ("Can't be bigger than 90.0 or smaller than -90.0.", "atLatitude");
            if (longitude > 180.0 || longitude < -180.0)
                throw new ArgumentException ("Can't be bigger than 180.0 or smaller than -180.0.", "atLongitude");
            if (numberOfResults <= 0)
                throw new ArgumentException ("Can't be smaller or equal to zero.", "numberOfResults");
            if (searchForName == null)
                searchForName = "";

            this.Client.Service.GeoLocation_Location_Search (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token, searchDistanceInMeters.ToString (),
                    latitude.ToString (CultureInfo.InvariantCulture), longitude.ToString (CultureInfo.InvariantCulture), numberOfResults.ToString (CultureInfo.InvariantCulture), searchForName, searchCategoryId >= 0 ? searchCategoryId.ToString () : "", (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<List<Place>> (null, bcr.Error));
                    return;
                }
                List<Place> lst = new List<Place> ();
                foreach (var d in result)
                    lst.Add (new Place (this.Client, this.AuthUser, d));
                {
                    callback (BuddyResultCreator.Create (lst, bcr.Error));
                    return; }
                ;
            });
            return;
        }


        /// <summary>
        /// Get all geo-location categories in Buddy.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is a list of geo-location categories mapped to their IDs.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetCategoriesAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetCategoriesAsync (Action<Dictionary<int, string>, BuddyCallbackParams> callback, object state = null)
        {
            GetCategoriesInternal ((bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void GetCategoriesInternal (Action<BuddyCallResult<Dictionary<int, string>>> callback)
        {
            this.Client.Service.GeoLocation_Category_GetList (this.Client.AppName, this.Client.AppPassword, AuthUser.Token, (bcr) =>
            {
                var result = bcr.Result;
                Dictionary<int, string> lst = new Dictionary<int, string> ();
                foreach (var d in result)
                    lst.Add (Int32.Parse (d.CategoryID), d.CategoryName);
                callback (BuddyResultCreator.Create (lst, bcr.Error));
            });
            return;

        }

      
        /// <summary>
        /// Get a Place by it's globally unique identifier. This method can also be used to calculate a distance from a lat/long to a place.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is the Place on success or null if there is no place with the given ID.</param>
        /// <param name="placeId">The ID of the place to retreive.</param>
        /// <param name="latitude">The optional latitude to calcualte a distance to.</param>
        /// <param name="longitude">The optioanl longitude to calculate a distance to.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetAsync (Action<Place, BuddyCallbackParams> callback, int placeId, double latitude = 0.0, double longitude = 0.0, object state = null)
        {
            GetInternal (placeId, latitude, longitude, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void GetInternal (int placeId, double latitude, double longitude, Action<BuddyCallResult<Place>> callback)
        {
            if (latitude > 90.0 || latitude < -90.0)
                throw new ArgumentException ("Can't be bigger than 90.0 or smaller than -90.0.", "atLatitude");
            if (longitude > 180.0 || longitude < -180.0)
                throw new ArgumentException ("Can't be bigger than 180.0 or smaller than -180.0.", "atLongitude");

            this.Client.Service.GeoLocation_Location_GetFromID (this.Client.AppName, this.Client.AppPassword, AuthUser.Token,
                    placeId.ToString (CultureInfo.InvariantCulture), latitude.ToString (CultureInfo.InvariantCulture), longitude.ToString (CultureInfo.InvariantCulture), (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<Place> (null, bcr.Error));
                    return;
                }
                if (result.Length == 0) {
                    callback (BuddyResultCreator.Create<Place> (null, bcr.Error));
                    return;
                } else {
                    callback (BuddyResultCreator.Create (new Place (this.Client, this.AuthUser, result [0]), bcr.Error));
                    return;
                }
            });

            return;
        }
    }
}
