using System;
using System.Net;

namespace Buddy
{
    /// <summary>
    /// Represents a single identity search result. Use the AuthenticatedUser.IdentityValues.CheckForValues() method to search for items. A search item
    /// can belong to any user in the system. 
    /// <example>
    /// <code>
    ///     BuddyClient client = new BuddyClient("APPNAME", "APPPASS");
    ///     client.LoginAsync((user, state) => {
    ///         List&lt;IdentityItemSearchResult&gt; items = user.IdentityValues.CheckForValuesAsync((r, ex) => {  }, "somevalue");
    ///     }, "username", "password");
    /// </code>
    /// </example>
    /// </summary>
    public class IdentityItemSearchResult : IdentityItem
    {
        /// <summary>
        /// Gets whether the specific item was found.
        /// </summary>
        public bool Found { get; protected set; }

        /// <summary>
        /// Gets the ID of the user the item was found on.
        /// </summary>
        public int BelongsToUserId { get; protected set; }

        internal IdentityItemSearchResult(string value, DateTime createdOn, bool found, int userId)
            :base (value, createdOn)
        {
            this.Found = found;
            this.BelongsToUserId = userId;
        }
    }
}
