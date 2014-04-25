using BuddyServiceClient;
using System;

namespace Buddy
{
    /// <summary>
    /// Represents a receipt in the Buddy system.
    /// </summary>
    public class Receipt : BuddyBase
    {

        /// <summary>
        /// Gets the ID of the retrieved receipt history item.
        /// </summary>
        public long ReceiptHistoryID { get; protected set; }

        /// <summary>
        /// Gets the name of the store in which this receipt was saved.
        /// </summary>
        public string StoreName { get; protected set; }

        /// <summary>
        /// Gets the ID of the user this receipt was saved for.
        /// </summary>
        public long UserID { get; protected set; }

        /// <summary>
        /// Gets the DateTime this receipt was saved or modified.
        /// </summary>
        public DateTime HistoryDateTime { get; protected set; }

        /// <summary>
        /// Gets the receipt data that was stored with this receipt.
        /// </summary>
        public string ReceiptData { get; protected set; }

        /// <summary>
        /// Gets the total cost of the transaction associated with this receipt.
        /// </summary>
        public string TotalCost { get; protected set; }

        /// <summary>
        /// Gets the number of items which were purchased during the transaction associated with this receipt.
        /// </summary>
        public int ItemQuantity { get; protected set; }

        /// <summary>
        /// Gets the (optional) metadata that was stored with this receipt.
        /// </summary>
        public string AppData { get; protected set; }

        /// <summary>
        /// Gets the customTransactionID that was saved for this receipt.
        /// </summary>
        public string HistoryCustomTransactionID { get; protected set; }

        /// <summary>
        /// Gets the raw verification data associated with the receipt as returned from the underlying Facebook or Apple servers.
        /// </summary>
        public string VerificationResultData { get; protected set; }

        /// <summary>
        /// Gets the Buddy StoreItemID of the item purchased in this transaction.
        /// </summary>
        public long StoreItemID { get; protected set; }

        internal Receipt(BuddyClient client, AuthenticatedUser user, InternalModels.DataContract_CommerceReceipt receipt)
            : base(client, user)
        {


            this.AppData = receipt.AppData;
            this.HistoryCustomTransactionID = receipt.HistoryCustomTransactionID;
            this.HistoryDateTime = DateTime.Parse(receipt.HistoryDateTime);
            this.ItemQuantity = int.Parse(receipt.ItemQuantity);
            this.ReceiptData = receipt.ReceiptData;
            this.ReceiptHistoryID = long.Parse(receipt.ReceiptHistoryID);
            this.StoreItemID = long.Parse(receipt.StoreItemID);
            this.StoreName = receipt.StoreName;
            this.TotalCost = receipt.TotalCost;
            this.UserID = long.Parse(receipt.UserID);
            this.VerificationResultData = !string.IsNullOrEmpty(receipt.VerificationResult) && bool.Parse(receipt.VerificationResult) ? this.VerificationResultData : "";
        }
    }
}