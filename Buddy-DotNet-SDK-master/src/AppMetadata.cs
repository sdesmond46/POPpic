using System;
using System.Linq;
using System.Collections.Generic;
using BuddyServiceClient;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Buddy
{
    /// <summary>
    /// Represents a collection of application level metadata items. You can access this class through the BuddyClient object.
    /// <example>
    /// <code>
    ///     BuddyClient client = new BuddyClient("APPNAME", "APPPASS");
    ///     client.Metadata.SetAsync(null, "test key", "test value");
    ///     
    ///     // Search for items within a thousand meters of a lat:0.0 and long:0.0.
    ///     client.Metadata.FindAsync(null, 1000, 0.0, 0.0);
    /// </code>
    /// </example>
    /// </summary>
    public class AppMetadata : BuddyBase
    {

        internal AppMetadata (BuddyClient client)
            : base(client)
        {
        }


        /// <summary>
        /// Get all the metadata items for this application. Note that this can be a very expensive method, try to retrieve specific items if possible
        /// or do a search.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is a list of all of the application metadata items.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetAllAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetAllAsync (Action<Dictionary<string, MetadataItem>, BuddyCallbackParams> callback, object state = null)
        {
            GetAllInternal ((bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void GetAllInternal (Action<BuddyCallResult<Dictionary<string, MetadataItem>>> callback)
        {
            this.Client.Service.MetaData_ApplicationMetaDataValue_GetAll (this.Client.AppName, this.Client.AppPassword, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<Dictionary<string, MetadataItem>> (null, bcr.Error));
                    return;
                }
                Dictionary<string, MetadataItem> dict = new Dictionary<string, MetadataItem> ();
                foreach (var d in result)
                    dict.Add (d.MetaKey, new MetadataItem (this.Client, null, this, null, d.MetaKey, d.MetaValue,
                    this.Client.TryParseDouble (d.MetaLatitude), this.Client.TryParseDouble (d.MetaLongitude), d.LastUpdateDate, null));
                {
                    callback (BuddyResultCreator.Create (dict, bcr.Error));
                    return; }
                ;
            });
            return;

        }

        /// <summary>
        /// Get a metadata item with a key. The key can't be null or an empty string.
        /// </summary>
        /// <param name="key">The key to use to reference the metadata item.</param>
        /// <param name="callback">The async callback to call on success or error. The first parameter is the metadata item or null if it doesn't exist.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetAsync (Action<MetadataItem, BuddyCallbackParams> callback, string key, object state = null)
        {
            GetInternal (key, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void GetInternal (string key, Action<BuddyCallResult<MetadataItem>> callback)
        {
            if (String.IsNullOrEmpty (key))
                throw new ArgumentException ("Can't be null or empty.", "key");

            this.Client.Service.MetaData_ApplicationMetaDataValue_Get (this.Client.AppName, this.Client.AppPassword, key, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<MetadataItem> (null, bcr.Error));
                    return;
                }
                if (result.Length == 0) {
                    callback (BuddyResultCreator.Create<MetadataItem> (null, bcr.Error));
                    return;
                }
                ;

                InternalModels.DataContract_ApplicationMetaData d = result[0];
                {
                    callback (BuddyResultCreator.Create (new MetadataItem (this.Client, null, this, null, d.MetaKey, d.MetaValue,
                       this.Client.TryParseDouble (d.MetaLatitude), this.Client.TryParseDouble (d.MetaLongitude), d.LastUpdateDate, null), bcr.Error));
                    return;
                }
                ;
            });
            return;

        }

       
        /// <summary>
        /// Set a metadata item value for a key. You can additional add latitude and longitude coordinate to record the location
        /// from where this item was set, or tag the item with a custom tag. 
        /// The item doesn't have to exist to be set, this method acts as an Add method in cases where the item doesn't exist.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true if the item was set, false otherwise..</param>
        /// <param name="key">The key of the metadata item, can't be null or empty.</param>
        /// <param name="value">The value of the metadata item, can't be null.</param>
        /// <param name="latitude">The optional latitude of the metadata item.</param>
        /// <param name="longitude">The optional longitude of the metadata item.</param>
        /// <param name="appTag">The optional application tag for this item.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of SetAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult SetAsync (Action<bool, BuddyCallbackParams> callback, string key, string value, double latitude = 0.0, double longitude = 0.0, string appTag = "", object state = null)
        {
            SetInternal (key, value, latitude, longitude, appTag, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void SetInternal (string key, string value, double latitude, double longitude, string appTag, Action<BuddyCallResult<bool>> callback)
        {
            if (String.IsNullOrEmpty (key))
                throw new ArgumentException ("Can't be null or empty.", "key");
            if (value == null)
                throw new ArgumentNullException ("value");
            if (appTag == null)
                appTag = "";

            this.Client.Service.MetaData_ApplicationMetaDataValue_Set (this.Client.AppName, this.Client.AppPassword, key, value, latitude.ToString (CultureInfo.InvariantCulture),
                    longitude.ToString (CultureInfo.InvariantCulture), appTag, (bcr) =>
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
        /// Delete a metadata item referenced by key.
        /// </summary>
        /// <param name="key">A valid key of a metadata item. The key can't be null or mpety.</param>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true if the item was deleted, false otherwise (i.e. doesn't exist).</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of DeleteAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult DeleteAsync (Action<bool, BuddyCallbackParams> callback, string key, object state = null)
        {
            DeleteInternal (key, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void DeleteInternal (string key, Action<BuddyCallResult<bool>> callback)
        {
            if (String.IsNullOrEmpty (key))
                throw new ArgumentException ("Can't be null or empty.", "key");

            this.Client.Service.MetaData_ApplicationMetaDataValue_Delete (this.Client.AppName, this.Client.AppPassword, key, (bcr) =>
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
        /// Delete all application metadata. There is no way to recover from this operation, be careful when you call it.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true if all metadata was deleted, false otherwise.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of DeleteAllAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult DeleteAllAsync (Action<bool, BuddyCallbackParams> callback, object state = null)
        {
            DeleteAllInternal ((bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void DeleteAllInternal (Action<BuddyCallResult<bool>> callback)
        {
            this.Client.Service.MetaData_ApplicationMetaDataValue_DeleteAll (this.Client.AppName, this.Client.AppPassword, (bcr) =>
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
        /// Search for metadata items in this application. Note that this method will only find app-level metadata items.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is a dictionary of metadata keys mapped to items.</param>
        /// <param name="searchDistanceMeters">The distance in meters from the latitude and longitude to search in. To ignore this distance pass in 40075000 (the circumferance of the earth).</param>
        /// <param name="latitude">The latitude from where the saerch will start.</param>
        /// <param name="longitude">The longitude from where the saerch will start.</param>
        /// <param name="numberOfResults">Optionally limit the number of returned metadata items.</param>
        /// <param name="withKey">Optionally search for items with a specific key. The value of this parameter is treated as a wildcard.</param>
        /// <param name="withValue">Optionally search for items with a specific value. The value of this parameter is treated as a wildcard.</param>
        /// <param name="updatedMinutesAgo">Optionally return only items that were updated some minutes ago.</param>
        /// <param name="valueMin">Optionally search for metadata item values that are bigger than this number.</param>
        /// <param name="valueMax">Optionally search for metadata item values that are smaller than this number.</param>
        /// <param name="searchAsFloat">Optionally treat all metadata values as floats. Useful for min/max searches.</param>
        /// <param name="sortAscending">Optionally sort the results ascending.</param>
        /// <param name="disableCache">Optionally disable cache searches.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of FindAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult FindAsync (Action<Dictionary<string, MetadataItem>, BuddyCallbackParams> callback, int searchDistanceMeters, double latitude, double longitude, int numberOfResults = 10,
                    string withKey = "", string withValue = "", int updatedMinutesAgo = -1, double valueMin = 0, double valueMax = 100, bool searchAsFloat = false,
                    bool sortAscending = false, bool disableCache = false, object state = null)
        {
            FindInternal (searchDistanceMeters, latitude, longitude, numberOfResults, withKey, withValue,
                updatedMinutesAgo, valueMin, valueMax, searchAsFloat, sortAscending, disableCache, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void FindInternal (int searchDistanceMeters, double latitude, double longitude, int numberOfResults,
                    string withKey, string withValue, int updatedMinutesAgo, double valueMin, double valueMax, bool searchAsFloat, bool sortAscending, bool disableCache, Action<BuddyCallResult<Dictionary<string, MetadataItem>>> callback)
        {
            this.Client.Service.MetaData_ApplicationMetaDataValue_SearchData (this.Client.AppName, this.Client.AppPassword, searchDistanceMeters.ToString (),
                    latitude.ToString (CultureInfo.InvariantCulture), longitude.ToString (CultureInfo.InvariantCulture), numberOfResults.ToString (), withKey, withValue, updatedMinutesAgo.ToString (),
                    valueMin.ToString (CultureInfo.InvariantCulture), valueMax.ToString (CultureInfo.InvariantCulture), searchAsFloat ? "1" : "", sortAscending ? "asc" : "desc", disableCache ? "true" : "", (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<Dictionary<string, MetadataItem>> (null, bcr.Error));
                    return;
                }
                Dictionary<string, MetadataItem> dict = new Dictionary<string, MetadataItem> ();
                foreach (var d in result)
                    dict.Add (d.MetaKey, new MetadataItem (this.Client, null, this, null, d, latitude, longitude));
                {
                    callback (BuddyResultCreator.Create (dict, bcr.Error));
                    return; }
                ;
            });
            return;

        }

     

        /// <summary>
        /// This method returns the sum of a set of metadata items that correspond to a certain key wildcard. Note that the values of these items
        /// need to be numbers or floats, otherwise this method will fail.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is a sum of all the found metadata item values.</param>
        /// <param name="forKeys">The key to use to filter the items that need to be summed. Is always treated as a wildcard.</param>
        /// <param name="withinDistance">Optionally sum only items within a certain number of meters from lat/long.</param>
        /// <param name="latitude">Optionally provide a latitude where the search can be started from.</param>
        /// <param name="longitude">Optionally provide a longitude where the search can be started from.</param>
        /// <param name="updatedMinutesAgo">Optionally sum only on items that have been update a number of minutes ago.</param>
        /// <param name="withAppTag">Optionally sum only items that have a certain application tag.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of SumAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult SumAsync (Action<MetadataSum, BuddyCallbackParams> callback, string forKeys, int withinDistance = -1, double latitude = 0.0, double longitude = 0.0,
                    int updatedMinutesAgo = -1, string withAppTag = "", object state = null)
        {
            SumInternal (forKeys, withinDistance, latitude, longitude, updatedMinutesAgo, withAppTag, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void SumInternal (string forKeys, int withinDistance, double latitude, double longitude,
                    int updatedMinutesAgo, string withAppTag, Action<BuddyCallResult<MetadataSum>> callback)
        {
            this.Client.Service.MetaData_ApplicationMetaDataValue_Sum (this.Client.AppName, this.Client.AppPassword,
                    forKeys, withinDistance.ToString (), latitude.ToString (CultureInfo.InvariantCulture), longitude.ToString (CultureInfo.InvariantCulture), updatedMinutesAgo.ToString (), withAppTag, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<MetadataSum> (null, bcr.Error));
                    return;
                }
                if (result.Length == 0) {
                    callback (BuddyResultCreator.Create<MetadataSum> (null, bcr.Error));
                    return;
                }
                ;

                {
                    callback (BuddyResultCreator.Create (new MetadataSum (String.IsNullOrEmpty (result [0].TotalValue) ? 0 : this.Client.TryParseDouble (result [0].TotalValue),
                               Int32.Parse (result [0].KeyCount), forKeys), bcr.Error));
                    return;
                }
                ;
            });
            return;
        }

      

        /// <summary>
        /// This method returns the sum of a set of metadata items that correspond to a certain key wildcard. Note that the values of these items
        /// need to be numbers or floats, otherwise this method will fail.
        /// Unlike the 'Sum' method this method can take a list of keys separated by semicolons and will return a list of sums for all of those keys.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is a list of sums corresponding to all the keys that were given to this method.</param>
        /// <param name="forKeys">The key to use to filter the items that need to be summed. Is always treated as a wildcard.</param>
        /// <param name="withinDistance">Optionally sum only items within a certain number of meters from lat/long.</param>
        /// <param name="latitude">Optionally provide a latitude where the search can be started from.</param>
        /// <param name="longitude">Optionally provide a longitude where the search can be started from.</param>
        /// <param name="updatedMinutesAgo">Optionally sum only on items that have been update a number of minutes ago.</param>
        /// <param name="withAppTag">Optionally sum only items that have a certain application tag.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of BatchSumAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult BatchSumAsync (Action<List<MetadataSum>, BuddyCallbackParams> callback, string forKeys, string withinDistance = "-1", double latitude = -1.0, double longitude = -1.0,
                    int updatedMinutesAgo = -1, string withAppTag = "", object state = null)
        {
            BatchSumInternal (forKeys, withinDistance, latitude, longitude, updatedMinutesAgo, withAppTag, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void BatchSumInternal (string forKeys, string withinDistance, double latitude, double longitude,
                    int updatedMinutesAgo, string withAppTag, Action<BuddyCallResult<List<MetadataSum>>> callback)
        {
            if (withinDistance == "-1")
                for (int i = 0; i < forKeys.Split(';').Length - 1; i++)
                    withinDistance += ";-1";

            this.Client.Service.MetaData_ApplicationMetaDataValue_BatchSum (this.Client.AppName, this.Client.AppPassword,
                    forKeys, withinDistance, latitude.ToString (CultureInfo.InvariantCulture), longitude.ToString (CultureInfo.InvariantCulture), updatedMinutesAgo.ToString (), withAppTag, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<List<MetadataSum>> (null, bcr.Error));
                    return;
                }
                List<MetadataSum> lst = new List<MetadataSum> ();
                foreach (var d in result)
                    lst.Add (new MetadataSum (this.Client.TryParseDouble (d.TotalValue), Int32.Parse (d.KeyCount), d.MetaKey));
                {
                    callback (BuddyResultCreator.Create (lst, bcr.Error));
                    return; }
                ;
            });
            return;

        }

#if AWAIT_SUPPORTED

     

            /// <summary>
            /// Get all the metadata items for this application. Note that this can be a very expensive method, try to retrieve specific items if possible
            /// or do a search.
            /// </summary>
            /// <returns>A Task&lt;IDictionary&lt;String,MetadataItem&gt; &gt;that can be used to monitor progress on this call.</returns>
            public System.Threading.Tasks.Task<IDictionary<String, MetadataItem>> GetAllAsync()
            {
                var tcs = new System.Threading.Tasks.TaskCompletionSource<IDictionary<String, MetadataItem>>();
                this.GetAllInternal((bcr) =>
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
            /// Get a metadata item with a key. The key can't be null or an empty string.
            /// </summary>
            /// <param name="key">The key to use to reference the metadata item.</param>
            /// <returns>A Task&lt;MetadataItem&gt;that can be used to monitor progress on this call.</returns>
            public System.Threading.Tasks.Task<MetadataItem> GetAsync( string key)
            {
                var tcs = new System.Threading.Tasks.TaskCompletionSource<MetadataItem>();
                this.GetInternal(key, (bcr) =>
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
            /// Set a metadata item value for a key. You can additional add latitude and longitude coordinate to record the location
            /// from where this item was set, or tag the item with a custom tag. 
            /// The item doesn't have to exist to be set, this method acts as an Add method in cases where the item doesn't exist.
            /// </summary>
            /// <param name="key">The key of the metadata item, can't be null or empty.</param>
            /// <param name="value">The value of the metadata item, can't be null.</param>
            /// <param name="latitude">The optional latitude of the metadata item.</param>
            /// <param name="longitude">The optional longitude of the metadata item.</param>
            /// <param name="appTag">The optional application tag for this item.</param>
            /// <returns>A Task&lt;Boolean&gt;that can be used to monitor progress on this call.</returns>
            public System.Threading.Tasks.Task<Boolean> SetAsync( string key, string value, double latitude = 0, double longitude = 0, string appTag = "")
            {
                var tcs = new System.Threading.Tasks.TaskCompletionSource<Boolean>();
                this.SetInternal(key, value, latitude, longitude, appTag, (bcr) =>
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
            /// Delete a metadata item referenced by key.
            /// </summary>
            /// <param name="key">A valid key of a metadata item. The key can't be null or mpety.</param>
            /// <returns>A Task&lt;Boolean&gt;that can be used to monitor progress on this call.</returns>
            public System.Threading.Tasks.Task<Boolean> DeleteAsync( string key)
            {
                var tcs = new System.Threading.Tasks.TaskCompletionSource<Boolean>();
                this.DeleteInternal(key, (bcr) =>
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
            /// Delete all application metadata. There is no way to recover from this operation, be careful when you call it.
            /// </summary>
            /// <returns>A Task&lt;Boolean&gt;that can be used to monitor progress on this call.</returns>
            public System.Threading.Tasks.Task<Boolean> DeleteAllAsync()
            {
                var tcs = new System.Threading.Tasks.TaskCompletionSource<Boolean>();
                this.DeleteAllInternal((bcr) =>
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
            /// Search for metadata items in this application. Note that this method will only find app-level metadata items.
            /// </summary>
            /// <param name="searchDistanceMeters">The distance in meters from the latitude and longitude to search in. To ignore this distance pass in 40075000 (the circumferance of the earth).</param>
            /// <param name="latitude">The latitude from where the saerch will start.</param>
            /// <param name="longitude">The longitude from where the saerch will start.</param>
            /// <param name="numberOfResults">Optionally limit the number of returned metadata items.</param>
            /// <param name="withKey">Optionally search for items with a specific key. The value of this parameter is treated as a wildcard.</param>
            /// <param name="withValue">Optionally search for items with a specific value. The value of this parameter is treated as a wildcard.</param>
            /// <param name="updatedMinutesAgo">Optionally return only items that were updated some minutes ago.</param>
            /// <param name="valueMin">Optionally search for metadata item values that are bigger than this number.</param>
            /// <param name="valueMax">Optionally search for metadata item values that are smaller than this number.</param>
            /// <param name="searchAsFloat">Optionally treat all metadata values as floats. Useful for min/max searches.</param>
            /// <param name="sortAscending">Optionally sort the results ascending.</param>
            /// <param name="disableCache">Optionally disable cache searches.</param>
            /// <returns>A Task&lt;IDictionary&lt;String,MetadataItem&gt; &gt;that can be used to monitor progress on this call.</returns>
            public System.Threading.Tasks.Task<IDictionary<String, MetadataItem>> FindAsync( int searchDistanceMeters, double latitude, double longitude, int numberOfResults = 10, string withKey = "", string withValue = "", int updatedMinutesAgo = -1, double valueMin = 0, double valueMax = 100, bool searchAsFloat = false, bool sortAscending = false, bool disableCache = false)
            {
                var tcs = new System.Threading.Tasks.TaskCompletionSource<IDictionary<String, MetadataItem>>();
                this.FindInternal(searchDistanceMeters, latitude, longitude, numberOfResults, withKey, withValue, updatedMinutesAgo, valueMin, valueMax, searchAsFloat, sortAscending, disableCache, (bcr) =>
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
            /// This method returns the sum of a set of metadata items that correspond to a certain key wildcard. Note that the values of these items
            /// need to be numbers or floats, otherwise this method will fail.
            /// </summary>
            /// <param name="forKeys">The key to use to filter the items that need to be summed. Is always treated as a wildcard.</param>
            /// <param name="withinDistance">Optionally sum only items within a certain number of meters from lat/long.</param>
            /// <param name="latitude">Optionally provide a latitude where the search can be started from.</param>
            /// <param name="longitude">Optionally provide a longitude where the search can be started from.</param>
            /// <param name="updatedMinutesAgo">Optionally sum only on items that have been update a number of minutes ago.</param>
            /// <param name="withAppTag">Optionally sum only items that have a certain application tag.</param>
            /// <returns>A Task&lt;MetadataSum&gt;that can be used to monitor progress on this call.</returns>
            public System.Threading.Tasks.Task<MetadataSum> SumAsync( string forKeys, int withinDistance = -1, double latitude = 0, double longitude = 0, int updatedMinutesAgo = -1, string withAppTag = "")
            {
                var tcs = new System.Threading.Tasks.TaskCompletionSource<MetadataSum>();
                this.SumInternal(forKeys, withinDistance, latitude, longitude, updatedMinutesAgo, withAppTag, (bcr) =>
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
            /// This method returns the sum of a set of metadata items that correspond to a certain key wildcard. Note that the values of these items
            /// need to be numbers or floats, otherwise this method will fail.
            /// Unlike the 'Sum' method this method can take a list of keys separated by semicolons and will return a list of sums for all of those keys.
            /// </summary>
            /// <param name="forKeys">The key to use to filter the items that need to be summed. Is always treated as a wildcard.</param>
            /// <param name="withinDistance">Optionally sum only items within a certain number of meters from lat/long.</param>
            /// <param name="latitude">Optionally provide a latitude where the search can be started from.</param>
            /// <param name="longitude">Optionally provide a longitude where the search can be started from.</param>
            /// <param name="updatedMinutesAgo">Optionally sum only on items that have been update a number of minutes ago.</param>
            /// <param name="withAppTag">Optionally sum only items that have a certain application tag.</param>
            /// <returns>A Task&lt;IEnumerable&lt;MetadataSum&gt; &gt;that can be used to monitor progress on this call.</returns>
            public System.Threading.Tasks.Task<IEnumerable<MetadataSum>> BatchSumAsync( string forKeys, string withinDistance = "-1", double latitude = -1, double longitude = -1, int updatedMinutesAgo = -1, string withAppTag = "")
            {
                var tcs = new System.Threading.Tasks.TaskCompletionSource<IEnumerable<MetadataSum>>();
                this.BatchSumInternal(forKeys, withinDistance, latitude, longitude, updatedMinutesAgo, withAppTag, (bcr) =>
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
