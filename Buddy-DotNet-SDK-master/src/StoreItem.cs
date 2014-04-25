using BuddyServiceClient;
using System;

namespace Buddy
{
    /// <summary>
    /// Represents a single, named store item in the Buddy system.
    /// </summary>
    public class StoreItem : BuddyBase
    {

        /// <summary>
        /// Gets the optional metadata associated with the item.
        /// </summary>
        public string AppData { get; protected set; }

        /// <summary>
        /// Gets the ID by which external sources identify the item by.
        /// </summary>
        public string CustomItemID { get; protected set; }

        /// <summary>
        /// Gets the flag indicating if the item is currently available for sale.
        /// </summary>
        public bool ItemAvailableFlag { get; protected set; }

        /// <summary>
        /// Gets the cost of the item.
        /// </summary>
        public string ItemCost { get; protected set; }

        /// <summary>
        /// Gets the date and time when the item was created or last updated.
        /// </summary>
        public DateTime ItemDateTime { get; protected set; }

        /// <summary>
        /// Gets the brief description of the item.
        /// </summary>
        public string ItemDescription { get; protected set; }

        /// <summary>
        /// Gets the URI where the item can be downloaded from.
        /// </summary>
        public string ItemDownloadUri { get; protected set; }

        /// <summary>
        /// Gets the flag indicating if the item is free.
        /// </summary>
        public bool ItemFreeFlag { get; protected set; }

        /// <summary>
        /// Gets the URI of the icon to display for this item.
        /// </summary>
        public string ItemIconUri { get; protected set; }

        /// <summary>
        /// Gets the name of the item.
        /// </summary>
        public string ItemName { get; protected set; }

        /// <summary>
        /// Gets the URI where the item can be previewed.
        /// </summary>
        public string ItemPreviewUri { get; protected set; }

        /// <summary>
        /// Gets the ID of the store item.
        /// </summary>
        public string StoreItemID { get; protected set; }

        internal StoreItem(BuddyClient client, AuthenticatedUser user, InternalModels.DataContract_CommerceStoreGetItems storeItem)
            : base(client, user)
        {

            this.AppData = storeItem.AppData;
            this.CustomItemID = storeItem.CustomItemID;
            this.StoreItemID = storeItem.StoreItemID;
            this.ItemAvailableFlag = storeItem.ItemAvailableFlag == "true";
            this.ItemCost = storeItem.ItemCost;
            this.ItemDateTime = DateTime.Parse(storeItem.ItemDateTime);
            this.ItemDescription = storeItem.ItemDescription;
            this.ItemDownloadUri = storeItem.ItemDownloadUri;
            this.ItemFreeFlag = storeItem.ItemFreeFlag == "true";
            this.ItemIconUri = storeItem.ItemIconUri;
            this.ItemName = storeItem.ItemName;
            this.ItemPreviewUri = this.ItemPreviewUri;
        }
    }
}