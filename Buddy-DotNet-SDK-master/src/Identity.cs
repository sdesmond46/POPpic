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
    /// Represents a class that can access identity values for a user or search for values accross the entire app. Identity values can be used to share public 
    /// information between users, for example hashes of email address that can be used to check whether a certain user is in the system.
    /// <example>
    /// <code>
    ///     BuddyClient client = new BuddyClient("APPNAME", "APPPASS");
    ///     client.LoginAsync((user, state) => {
    ///         var lst = user.IdentityValues.GetAllAsync(null);
    ///     }, "username", "password");
    /// </code>
    /// </example>
    /// </summary>
    public class Identity : BuddyBase
    {
        protected string Token { get; set; }

        internal Identity (BuddyClient client, string token)
            : base(client)
        {
            if (String.IsNullOrEmpty (token))
                throw new ArgumentException ("Can't be null or empty.", "token");

            this.Token = token;
        }

        /// <summary>
        /// Returns all the identity values for this user.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is a list of identity values.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetAllAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetAllAsync (Action<List<IdentityItem>, BuddyCallbackParams> callback, object state = null)
        {
            GetAllInternal ((bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void GetAllInternal (Action<BuddyCallResult<List<IdentityItem>>> callback)
        {
            this.Client.Service.UserAccount_Identity_GetMyList (this.Client.AppName, this.Client.AppPassword, this.Token, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<List<IdentityItem>> (null, bcr.Error));
                    return;
                }
                List<IdentityItem> items = new List<IdentityItem> ();
                foreach (var d in result)
                    items.Add (new IdentityItem (d.IdentityValue, Convert.ToDateTime (d.CreatedDateTime, CultureInfo.InvariantCulture)));
                {
                    callback (BuddyResultCreator.Create (items, bcr.Error));
                    return; }
                ;
            });
            return;
        }


        /// <summary>
        /// Add an identity value for this user.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true if the value was added, false otherwise.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of AddAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult AddAsync (Action<bool, BuddyCallbackParams> callback, string value, object state = null)
        {
            AddInternal (value, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void AddInternal (string value, Action<BuddyCallResult<bool>> callback)
        {
            if (String.IsNullOrEmpty (value))
                throw new ArgumentException ("Can't be null or empty.", "value");

            this.Client.Service.UserAccount_Identity_AddNewValue (this.Client.AppName, this.Client.AppPassword, this.Token, value, (bcr) =>
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
        /// Remove an identity value for this user.
        /// </summary>
        /// <param name="value">The value to remove.</param>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true if the value was removed, false otherwise.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of RemoveAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult RemoveAsync (Action<bool, BuddyCallbackParams> callback, string value, object state = null)
        {
            RemoveInternal (value, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void RemoveInternal (string value, Action<BuddyCallResult<bool>> callback)
        {
            if (String.IsNullOrEmpty (value))
                throw new ArgumentException ("Can't be null or empty.", "value");

            this.Client.Service.UserAccount_Identity_RemoveValue (this.Client.AppName, this.Client.AppPassword, this.Token, value, (bcr) =>
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
        /// Check for the existance of an identity value in the system. The search is perform for the entire app.
        /// </summary>
        /// <param name="values">The value to search for. This can either be a single value or a semi-colon separated list of values.</param>
        /// <param name="callback">The async callback to call on success or error. The first parameter is a list of identity values that were found.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of CheckForValuesAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult CheckForValuesAsync (Action<List<IdentityItemSearchResult>, BuddyCallbackParams> callback, string values, object state = null)
        {
            CheckForValuesInternal (values, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void CheckForValuesInternal (string values, Action<BuddyCallResult<List<IdentityItemSearchResult>>> callback)
        {
            if (String.IsNullOrEmpty (values))
                throw new ArgumentException ("Can't be null or empty.", "value");

            this.Client.Service.UserAccount_Identity_CheckForValues (this.Client.AppName, this.Client.AppPassword, this.Token, values, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<List<IdentityItemSearchResult>> (null, bcr.Error));
                    return;
                }
                List<IdentityItemSearchResult> lst = new List<IdentityItemSearchResult> ();
                foreach (var d in result)
                    lst.Add (new IdentityItemSearchResult (d.ValueFound, DateTime.MinValue, d.ValueFound == "1" ? true : false,
                    d.ValueFound == "1" ? Int32.Parse (d.UserProfileID) : -1));
                {
                    callback (BuddyResultCreator.Create (lst, bcr.Error));
                    return; }
                ;
            });
            return;
        }
    }
}
