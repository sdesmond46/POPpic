using System;
using System.Net;
using BuddyServiceClient;

namespace Buddy
{
    /// <summary>
    /// Represents a single item of metadata. Metadata is used to store custom key/value pairs at the application or user level.
    /// <example>
    /// <code>
    ///     BuddyClient client = new BuddyClient("APPNAME", "APPPASS");
    ///     client.LoginAsync((user, ex) => {
    ///         user.Metadata.GetAsync((item, state) => {
    ///             item.SetAsync(null, "some value");
    ///         }, "some key");
    ///     }, "username", "password");
    /// </code>
    /// </example>
    /// </summary>
    public class MetadataItem : BuddyBase, IComparable<MetadataItem>
    {


        /// <summary>
        /// Gets the key for this item.
        /// </summary>
        public string Key { get; protected set; }

        /// <summary>
        /// Gets the value for this item.
        /// </summary>
        public string Value { get; protected set; }

        /// <summary>
        /// Gets the latitude of this item.
        /// </summary>
        public double Latitude { get; protected set; }

        /// <summary>
        /// Gets the longitude of this item.
        /// </summary>
        public double Longitude { get; protected set; }

        /// <summary>
        /// Gets the last date this item was updated.
        /// </summary>
        public DateTime LastUpdateOn { get; protected set; }

        /// <summary>
        /// Gets a custom application Tag for this item.
        /// </summary>
        public string ApplicationTag { get; protected set; }

        /// <summary>
        /// Gets the latitude of the origin that was used in the metadata search.
        /// </summary>
        public double DistanceOriginLatitude { get; protected set; }

        /// <summary>
        /// Gets the longitude of the origin that was used in the metadata search.
        /// </summary>
        public double DistanceOriginLongitude { get; protected set; }

        /// <summary>
        /// Gets the distance in kilo-meters from the given origin in the Metadata Search method.
        /// </summary>
        public double DistanceInKilometers { get; protected set; }

        /// <summary>
        /// Gets the distance in meters from the given origin in the Metadata Search method.
        /// </summary>
        public double DistanceInMeters { get; protected set; }

        /// <summary>
        /// Gets the distance in miles from the given origin in the Metadata Search method.
        /// </summary>
        public double DistanceInMiles { get; protected set; }

        /// <summary>
        /// Gets the distance in yards from the given origin in the Metadata Search method.
        /// </summary>
        public double DistanceInYards { get; protected set; }

        protected UserMetadata Owner { get; set; }

        protected AppMetadata OwnerApp { get; set; }

        protected string Token { get; set; }

        internal MetadataItem(BuddyClient client, UserMetadata owner, AppMetadata ownerApp, string token, InternalModels.DataContract_SearchUserMetaData d, double originLatitude, double originLongitude)
            : this(client, owner, ownerApp, token, d.MetaKey, d.MetaValue, client.TryParseDouble(d.MetaLatitude), client.TryParseDouble(d.MetaLongitude), d.LastUpdateDate, null)
        {
            this.DistanceInKilometers = this.Client.TryParseDouble (d.DistanceInKilometers);
            this.DistanceInMeters = this.Client.TryParseDouble (d.DistanceInMeters);
            this.DistanceInMiles = this.Client.TryParseDouble (d.DistanceInMiles);
            this.DistanceInYards = this.Client.TryParseDouble (d.DistanceInYards);
        }

        internal MetadataItem(BuddyClient client, UserMetadata owner, AppMetadata ownerApp, string token, InternalModels.DataContract_SearchAppMetaData d, double originLatitude, double originLongitude)
            : this(client, owner, ownerApp, token, d.MetaKey, d.MetaValue, client.TryParseDouble(d.MetaLatitude), client.TryParseDouble(d.MetaLongitude), d.LastUpdateDate, null)
        {
            this.DistanceInKilometers = this.Client.TryParseDouble (d.DistanceInKilometers);
            this.DistanceInMeters = this.Client.TryParseDouble (d.DistanceInMeters);
            this.DistanceInMiles = this.Client.TryParseDouble (d.DistanceInMiles);
            this.DistanceInYards = this.Client.TryParseDouble (d.DistanceInYards);
        }

        internal MetadataItem (BuddyClient client, UserMetadata owner, AppMetadata ownerApp, string token, string key, string value, double latitude, double longitude, DateTime lastUpdateOn, string appTag)
            : base(client)
        {
            if (String.IsNullOrEmpty (key))
                throw new ArgumentException ("Can;t be null or empty.", "key");

            this.Key = key;
            this.Value = value;
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.LastUpdateOn = lastUpdateOn;
            this.ApplicationTag = appTag;
            this.Token = token;
            this.Owner = owner;
            this.OwnerApp = ownerApp;
        }

        public int CompareTo (MetadataItem other)
        {
            if (other.Key == this.Key && other.Value == this.Value)
                return 0;
            else
                return this.Key.CompareTo (other.Key);
        }

        /// <summary>
        /// Updates the value of this metadata item.
        /// </summary>
        /// <param name="callback">The callback to call when this method completes. The first parameter is true if the update was successful, false otherwise.</param>
        /// <param name="value">The new value for this item, can't be null.</param>
        /// <param name="latitude">The optional latitude for this item.</param>
        /// <param name="longitude">The optional longitude for this item.</param>
        /// <param name="appTag">The optional application tag for this item.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of SetAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult SetAsync (Action<bool, BuddyCallbackParams> callback, string value, double latitude = 0.0, double longitude = 0.0, string appTag = "", object state = null)
        {

            SetInternal (value, latitude, longitude, appTag, (bcr) => callback (bcr.Result, new BuddyCallbackParams (bcr.Error == BuddyError.None, bcr.Error == BuddyError.None ? null : new BuddyServiceException (bcr.Error.ToString ()), null, null)));

            return null;
        }

        internal void SetInternal (string value, double latitude, double longitude, string appTag, Action<BuddyCallResult<bool>> callback)
        {
            if (Owner != null) {
                Owner.SetInternal (this.Key, value, latitude, longitude, appTag, callback);
            } else {
                OwnerApp.SetInternal (this.Key, value, latitude, longitude, appTag, callback);
            }
        }


        /// <summary>
        /// Deletes this metadata item.
        /// </summary>
        /// <param name="callback">The callback to call when this method completes. The first parameter is true if the item was deleted, false otherwise.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of DeleteAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult DeleteAsync (Action<bool, BuddyCallbackParams> callback, object state = null)
        {
            DeleteInternal ((bcr) => callback (bcr.Result, new BuddyCallbackParams (bcr.Error == BuddyError.None, bcr.Error == BuddyError.None ? null : new BuddyServiceException (bcr.Error.ToString ()), null, null)));
            return null;
        }

        internal void DeleteInternal (Action<BuddyCallResult<bool>> callback)
        {
            if (Owner != null) {
                Owner.DeleteInternal (this.Key, callback);
            } else {
                OwnerApp.DeleteInternal (this.Key, callback);
            }
            
        }
    }
}
