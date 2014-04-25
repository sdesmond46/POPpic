using BuddyServiceClient;
using System;

namespace Buddy
{
    /// <summary>
    /// Represents a single, named metropolitan area in the Buddy system.
    /// </summary>
    public class MetroArea : BuddyBase
    {

        /// <summary>
        /// Gets the name of the the supported metro area.
        /// </summary>
        public string MetroName { get; protected set; }

        /// <summary>
        /// Gets the image URL an image for the area.
        /// </summary>
        public string ImageURL { get; protected set; }

        internal MetroArea(BuddyClient client, AuthenticatedUser user, InternalModels.DataContract_MetroList metro)
            : base(client, user)
        {


            this.MetroName = metro.MetroName;

            this.ImageURL = metro.ImageURL;
        }
    }
}
