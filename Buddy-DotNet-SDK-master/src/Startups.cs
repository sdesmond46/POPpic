using BuddyServiceClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Buddy
{
    /// <summary>
    /// Represents an object that can be used to search for startups around the user.
    /// <example>
    /// <code>
    ///     BuddyClient client = new BuddyClient("APPNAME", "APPPASS");
    ///     client.LoginAsync((user, state) => {
    ///         user.Startups.FindAsync((startups, state2) => {  }, 1000000, 0.0, 0.0, 10);
    ///     }, "username", "password");
    /// </code>
    /// </example>
    /// </summary>
    public class Startups : BuddyBase
    {
        protected override bool AuthUserRequired {
            get {
                return true;
            }
        }

        internal Startups (BuddyClient client, AuthenticatedUser user)
            : base(client, user)
        {

        }


        /// <summary>
        /// Searches for statups by name within the distance of the specified location. Note: To search for all startups within the distance from the specified location, leave the SearchName parameter empty.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is a list of startups that were found.</param>
        /// <param name="searchDistanceInMeters">The radius of the startup search.</param>
        /// <param name="latitude">The latitude where the search should start.</param>
        /// <param name="longitude">The longitude where the search should start.</param>
        /// <param name="numberOfResults">The number of search results to return.</param>
        /// <param name="searchForName">Optional search string, for example: "Star*" to search for all startups that begin with the string "Star".</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of FindAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult FindAsync (Action<List<Startup>, BuddyCallbackParams> callback, int searchDistanceInMeters, double latitude, double longitude, int numberOfResults, string searchForName = "", object state = null)
        {
            FindInternal (searchDistanceInMeters, latitude, longitude, numberOfResults, searchForName, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void FindInternal (int searchDistanceInMeters, double latitude, double longitude, int numberOfResults, string searchForName, Action<BuddyCallResult<List<Startup>>> callback)
        {
            if (searchDistanceInMeters < 0)
                throw new ArgumentException ("Can't be smaller than zero.", "searchDistanceInMeters");
            if (latitude > 90.0 || latitude < -90.0)
                throw new ArgumentException ("Can't be bigger than 90.0 or smaller than -90.0.", "latitude");
            if (longitude > 180.0 || longitude < -180.0)
                throw new ArgumentException ("Can't be bigger than 180.0 or smaller than -180.0.", "longitude");
            if (numberOfResults < 0)
                throw new ArgumentException ("Can't be smaller than zero.", "numberOfResults");

            this.Client.Service.StartupData_Location_Search (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token, searchDistanceInMeters.ToString (),
                    latitude.ToString (CultureInfo.InvariantCulture), longitude.ToString (CultureInfo.InvariantCulture), numberOfResults.ToString (CultureInfo.InvariantCulture), searchForName, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<List<Startup>> (null, bcr.Error));
                    return;
                }
                List<Startup> lst = new List<Startup> ();
                foreach (var d in result)
                    lst.Add (new Startup (this.Client, this.AuthUser, d));
                {
                    callback (BuddyResultCreator.Create (lst, bcr.Error));
                    return; }
                ;
            });
            return;
        }

      
        /// <summary>
        /// Gets a list of the supported metro areas for statups including the URL to an image for each area returned.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is a .NET List of MetroAreas that were found.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetMetroAreaListAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetMetroAreaListAsync (Action<List<MetroArea>, BuddyCallbackParams> callback, object state = null)
        {
            GetMetroAreaListInternal ((bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void GetMetroAreaListInternal (Action<BuddyCallResult<List<MetroArea>>> callback)
        {
            this.Client.Service.StartupData_Location_GetMetroList (this.Client.AppName, this.Client.AppPassword, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<List<MetroArea>> (null, bcr.Error));
                    return;
                }
                var list = new List<MetroArea> ();
                foreach (var d in result)
                    list.Add (new MetroArea (this.Client, this.AuthUser, d));
                {
                    callback (BuddyResultCreator.Create (list, bcr.Error));
                    return; }
                ;
            });
            return;
        }


        /// <summary>
        /// Get a list of startups in the specified metro area.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is a .NET List of Startups that were found.</param>
        /// <param name="metroName">The name of the metro area within which to search for startups.</param>
        /// <param name="recordLimit">The number of search results to return.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetFromMetroAreaAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetFromMetroAreaAsync (Action<List<Startup>, BuddyCallbackParams> callback, string metroName, int recordLimit, object state = null)
        {
            GetFromMetroAreaInternal (metroName, recordLimit, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void GetFromMetroAreaInternal (string metroName, int recordLimit, Action<BuddyCallResult<List<Startup>>> callback)
        {
            if (string.IsNullOrEmpty (metroName))
                throw new ArgumentNullException ("metroName");
            if (recordLimit < 0)
                throw new ArgumentException ("Can't be smaller than zero.", "recordLimit");

            this.Client.Service.StartupData_Location_GetFromMetroArea (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token, metroName, recordLimit.ToString (), (bcr) =>
            {
                var result = bcr.Result;
                var list = new List<Startup> ();
                foreach (var d in result)
                    list.Add (new Startup (this.Client, this.AuthUser, d));
                callback (BuddyResultCreator.Create (list, bcr.Error));
            });
            return;
        }
    }
}
