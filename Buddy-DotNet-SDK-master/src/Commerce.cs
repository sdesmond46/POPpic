using BuddyServiceClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Buddy
{
    /// <summary>
    /// Represents an object that can be used to handle commerce for the user.
    /// <example>
    /// <code>
    /// BuddyClient client = new BuddyClient("APPNAME", "APPPASS");
    /// client.LoginAsync((user, state) => 
    /// {
    ///     user.Commerce.GetAllStoreItemsAsync((storeItems, state2) =>
    ///     {
    ///         var item = storeItems.SingleOrDefault();
    ///     });
    /// }, "username", "password");
    /// </code>
    /// </example>
    /// </summary>
    public class Commerce : BuddyBase
    {

        protected override bool AuthUserRequired {
            get {
                return true;
            }
        }

        internal Commerce (BuddyClient client, AuthenticatedUser user)
            : base(client, user)
        {

        }

     

        /// <summary>
        /// Finds the receipt list based on the FromDateTime parameter for the currently logged in user.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is .NET List of Receipts if this method was successful.</param>
        /// <param name="fromDateTime">The starting date and time to get receipts from, leave this blank to get all the receipts.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetReceiptsForUserAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetReceiptsForUserAsync (Action<List<Receipt>, BuddyCallbackParams> callback, DateTime? fromDateTime = null, object state = null)
        {
            GetReceiptsForUserInternal (fromDateTime, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void GetReceiptsForUserInternal (DateTime? fromDateTime, Action<BuddyCallResult<List<Receipt>>> callback)
        {
            this.Client.Service.Commerce_Receipt_GetForUser (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token, fromDateTime.HasValue ? fromDateTime.ToString () : "", (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<List<Receipt>> (null, bcr.Error));
                    return;
                }
                var lst = new List<Receipt> ();
                foreach (var d in result)
                    lst.Add (new Receipt (this.Client, this.AuthUser, d));
                {
                    callback (BuddyResultCreator.Create (lst, bcr.Error));
                    return; }
                ;
            });
            return;

        }

       
        /// <summary>
        /// Finds the receipt associated with the specified CustomTransactionID for the currently logged in user.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is .NET List of Receipts if this method was successful.</param>
        /// <param name="customTransactionID">The CustomTransactionID of the transaction. For Facebook payments this is the OrderID of the transaction.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetReceiptForUserAndTransactionIDAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetReceiptForUserAndTransactionIDAsync (Action<List<Receipt>, BuddyCallbackParams> callback, string customTransactionID, object state = null)
        {
            GetReceiptForUserAndTransactionIDInternal (customTransactionID, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void GetReceiptForUserAndTransactionIDInternal (string customTransactionID, Action<BuddyCallResult<List<Receipt>>> callback)
        {
            if (string.IsNullOrEmpty (customTransactionID))
                throw new ArgumentNullException ("customTransactionID");

            this.Client.Service.Commerce_Receipt_GetForUserAndTransactionID (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token, customTransactionID, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<List<Receipt>> (null, bcr.Error));
                    return;
                }
                var lst = new List<Receipt> ();
                foreach (var d in result)
                    lst.Add (new Receipt (this.Client, this.AuthUser, d));
                {
                    callback (BuddyResultCreator.Create (lst, bcr.Error));
                    return; }
                ;
            });
            return;

        }

        /// <summary>
        /// Saves a receipt for the purchase of an item made to the application's store.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true if successful, false otherwise.</param>
        /// <param name="totalCost">The total cost for the items purchased in the transaction.</param>
        /// <param name="totalQuantity">The total number of items purchased.</param>
        /// <param name="storeItemID">The store ID of the item of the item being purchased.</param>
        /// <param name="storeName">The name of the application's store to be saved with the transaction. This field is used by the commerce analytics to track purchases.</param>
        /// <param name="receiptData">Optional information to store with the receipt such as notes about the transaction.</param>
        /// <param name="customTransactionID">An optional app-specific ID to associate with the purchase.</param>
        /// <param name="appData">Optional metadata to associate with the transaction.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of SaveReceiptAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult SaveReceiptAsync (Action<bool, BuddyCallbackParams> callback, string totalCost, int totalQuantity, int storeItemID, string storeName,
                            string receiptData = "", string customTransactionID = "", string appData = "", object state = null)
        {
            SaveReceiptInternal (totalCost, totalQuantity, storeItemID, storeName, receiptData, customTransactionID, appData, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void SaveReceiptInternal (string totalCost, int totalQuantity, int storeItemID, string storeName,
                    string receiptData, string customTransactionID, string appData, Action<BuddyCallResult<bool>> callback)
        {
            if (string.IsNullOrEmpty (totalCost))
                throw new ArgumentNullException ("totalCost");
            if (string.IsNullOrEmpty (storeName))
                throw new ArgumentNullException ("storeName");

            this.Client.Service.Commerce_Receipt_Save (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token,
                    receiptData, customTransactionID, appData, totalCost, totalQuantity.ToString (), storeItemID.ToString (), storeName, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None && bcr.Error != BuddyError.ServiceErrorNegativeOne) {
                    callback (BuddyResultCreator.Create (default(bool), bcr.Error));
                    return;
                }
                {
                    callback (BuddyResultCreator.Create (result == "1", bcr.Error));
                    return; }
                ;
            });
            return;

        }

       

        /// <summary>
        /// Verifies that a receipt received from the Apple store is actually from Apple.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true if successful, false otherwise.</param>
        /// <param name="totalCost">The total cost for the items purchased in the transaction.</param>
        /// <param name="totalQuantity">The total number of items purchased.</param>
        /// <param name="useSandbox">Set to true when testing this function in a "sandbox" to execute this call against the Apple sandbox server, or false to have the call executed against the production Apple server.</param>
        /// <param name="appleItemID">The optional ID associated with the item as assigned by Apple.</param>
        /// <param name="receiptData">Optional information to store with the receipt such as notes about the transaction.</param>
        /// <param name="customTransactionID">An optional app-specific ID to associate with the purchase.</param>
        /// <param name="appData">Optional metadata to associated with the transaction.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of VerifyiOSReceiptAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult VerifyiOSReceiptAsync (Action<bool, BuddyCallbackParams> callback, string totalCost, int totalQuantity, bool useSandbox, string appleItemID = "",
                            string receiptData = "", string customTransactionID = "", string appData = "", object state = null)
        {
            VerifyiOSReceiptInternal (totalCost, totalQuantity, useSandbox, appleItemID, receiptData, customTransactionID, appData, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void VerifyiOSReceiptInternal (string totalCost, int totalQuantity, bool useSandbox, string appleItemID,
                    string receiptData, string customTransactionID, string appData, Action<BuddyCallResult<bool>> callback)
        {
            if (string.IsNullOrEmpty (totalCost))
                throw new ArgumentNullException ("totalCost");


            this.Client.Service.Commerce_Receipt_VerifyAndSaveiOSReceipt (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token,
                    appleItemID, receiptData, customTransactionID, appData, totalCost, totalQuantity.ToString (), useSandbox ? "1" : "0", (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create (default(bool), bcr.Error));
                    return;
                }
                {
                    callback (BuddyResultCreator.Create (result == "1", bcr.Error));
                    return; }
                ;
            });
            return;


        }

       

        /// <summary>
        /// Verifies that a receipt received from the Apple store is actually from Apple and stores a copy of the receipt on Buddy's servers.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true if successful, false otherwise.</param>
        /// <param name="totalCost">The total cost for the items purchased in the transaction.</param>
        /// <param name="totalQuantity">The total number of items purchased.</param>
        /// <param name="useSandbox">Set to true when testing this function in a "sandbox" to execute this call against the Apple sandbox server, or false to have the call executed against the production Apple server.</param>
        /// <param name="appleItemID">The optional ID associated with the item as assigned by Apple.</param>
        /// <param name="receiptData">Optional information to store with the receipt such as notes about the transaction.</param>
        /// <param name="customTransactionID">An optional app-specific ID to associate with the purchase.</param>
        /// <param name="appData">Optional metadata to associated with the transaction.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of VerifyAndSaveiOSReceiptAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult VerifyAndSaveiOSReceiptAsync (Action<bool, BuddyCallbackParams> callback, string totalCost, int totalQuantity, bool useSandbox, string appleItemID = "",
                            string receiptData = "", string customTransactionID = "", string appData = "", object state = null)
        {
            VerifyAndSaveiOSReceiptInternal (totalCost, totalQuantity, useSandbox, appleItemID, receiptData, customTransactionID, appData, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void VerifyAndSaveiOSReceiptInternal (string totalCost, int totalQuantity, bool useSandbox, string appleItemID,
                    string receiptData, string customTransactionID, string appData, Action<BuddyCallResult<bool>> callback)
        {
            if (string.IsNullOrEmpty (totalCost))
                throw new ArgumentNullException ("totalCost");

            this.Client.Service.Commerce_Receipt_VerifyAndSaveiOSReceipt (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token,
                    appleItemID, receiptData, customTransactionID, appData, totalCost, totalQuantity.ToString (), useSandbox ? "1" : "0", (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create (default(bool), bcr.Error));
                    return;
                }
                {
                    callback (BuddyResultCreator.Create (result == "1", bcr.Error));
                    return; }
                ;
            });
            return;

        }

      

        /// <summary>
        /// Returns information about all items in the store for the current application.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is a .NET List of StoreItems if this method was successful.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetAllStoreItemsAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetAllStoreItemsAsync (Action<List<StoreItem>, BuddyCallbackParams> callback, object state = null)
        {
            GetAllStoreItemsInternal ((bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }
      
        internal void GetAllStoreItemsInternal (Action<BuddyCallResult<List<StoreItem>>> callback)
        {
            this.Client.Service.Commerce_Store_GetAllItems (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<List<StoreItem>> (null, bcr.Error));
                    return;
                }
                List<StoreItem> lst = new List<StoreItem> ();
                foreach (var d in result)
                    lst.Add (new StoreItem (this.Client, this.AuthUser, d));
                {
                    callback (BuddyResultCreator.Create (lst, bcr.Error));
                    return; }
                ;
            });
            return;

        }

       

        /// <summary>
        /// Returns information about all store items for an application which are currently active (available for sale).
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is a .NET List of StoreItems if this method was successful.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetActiveStoreItemsAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetActiveStoreItemsAsync (Action<List<StoreItem>, BuddyCallbackParams> callback, object state = null)
        {
            GetActiveStoreItemsInternal ((bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void GetActiveStoreItemsInternal (Action<BuddyCallResult<List<StoreItem>>> callback)
        {
            this.Client.Service.Commerce_Store_GetActiveItems (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<List<StoreItem>> (null, bcr.Error));
                    return;
                }
                List<StoreItem> lst = new List<StoreItem> ();
                foreach (var d in result)
                    lst.Add (new StoreItem (this.Client, this.AuthUser, d));
                {
                    callback (BuddyResultCreator.Create (lst, bcr.Error));
                    return; }
                ;
            });
            return;

        }


        /// <summary>
        /// Returns information about all items in the store for the current application which are marked as free.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is a .NET List of StoreItems if this method was successful.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        ///
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetFreeStoreItemsAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetFreeStoreItemsAsync (Action<List<StoreItem>, BuddyCallbackParams> callback, object state = null)
        {
            GetFreeStoreItemsInternal ((bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void GetFreeStoreItemsInternal (Action<BuddyCallResult<List<StoreItem>>> callback)
        {
            this.Client.Service.Commerce_Store_GetFreeItems (this.Client.AppName, this.Client.AppPassword, this.AuthUser.Token, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<List<StoreItem>> (null, bcr.Error));
                    return;
                }
                List<StoreItem> lst = new List<StoreItem> ();
                foreach (var d in result)
                    lst.Add (new StoreItem (this.Client, this.AuthUser, d));
                {
                    callback (BuddyResultCreator.Create (lst, bcr.Error));
                    return; }
                ;
            });
            return;

        }
    }
}
