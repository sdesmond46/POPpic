using System;
using System.Net;
using BuddyServiceClient;

namespace Buddy
{
    /// <summary>
    /// Represents an identity item that belongs to a user.
    /// <example>
    /// <code>
    ///     BuddyClient client = new BuddyClient("APPNAME", "APPPASS");
    ///     client.LoginAsync((user, state) => {
    ///         user.IdentityValues.GetAllAsync(null);
    ///     }, "username", "password");
    /// </code>
    /// </example>
    /// </summary>
    public class IdentityItem
    {
        /// <summary>
        /// Gets the value of the identity item.
        /// </summary>
        public string Value { get; protected set; }

        /// <summary>
        /// Gets the date the identity value was added.
        /// </summary>
        public DateTime CreatedOn { get; protected set; }

        internal IdentityItem(string value, DateTime created)
        {
            this.CreatedOn = created;
            this.Value = value;
        }
    }
}
