using System;
using System.Net;

namespace Buddy
{
    /// <summary>
    /// Represents the sum of a collection of metadata items with a similar key.
    /// <example>
    /// <code>
    ///     BuddyClient client = new BuddyClient("APPNAME", "APPPASS");
    ///     client.Metadata.SetAsync(null, "Test count1", "10");
    ///     client.Metadata.SetAsync(null, "Test count2", "20");
    ///     client.Metadata.SetAsync(null, "Test count3", "30");
    ///     client.Metadata.SumAsync((sum, ex) => {  }, "Test count");
    /// </code>
    /// </example>
    /// </summary>
    public class MetadataSum
    {
        /// <summary>
        /// Gets the total sum of the metadata items.
        /// </summary>
        public double Total { get; protected set; }

        /// <summary>
        /// Gets the number of items that were summed.
        /// </summary>
        public int KeysCounted { get; protected set; }

        /// <summary>
        /// Gets the common key that was used to produce this sum.
        /// </summary>
        public string Key { get; protected set; }

        internal MetadataSum(double total, int keysCounted, string key)
        {
            this.Total = total;
            this.KeysCounted = keysCounted;
            this.Key = key;
        }
    }
}
