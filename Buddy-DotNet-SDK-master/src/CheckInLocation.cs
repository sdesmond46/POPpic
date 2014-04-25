using System;
using System.Linq;
using System.Net;

namespace Buddy
{
    /// <summary>
    /// Represents a single user check-in location.
    /// <example>
    /// <code>
    ///     BuddyClient client = new BuddyClient("APPNAME", "APPPASS");
    ///     client.Login((user, state) => { 
    ///         user.CheckInAsync(null, 0.0, 0.0, "some comment");
    ///         user.GetCheckInsAsync((r, params2) => {
    ///             List&lt;CheckInLocation&gt; items = r;
    ///         });
    ///     }, "username", "password");
    /// </code>
    /// </example>
    /// </summary>
    public class CheckInLocation
    {
        /// <summary>
        /// Gets the latitude of the check-in location.
        /// </summary>
        public double Latitude { get; protected set; }

        /// <summary>
        /// Gets the longitude of the check-in location.
        /// </summary>
        public double Longitude { get; protected set; }

        /// <summary>
        /// Gets the datetime of the check-in.
        /// </summary>
        public DateTime CheckInDate { get; protected set; }

        /// <summary>
        /// Gets the name of the place where the check-in happend.
        /// </summary>
        public string PlaceName { get; protected set; }

        /// <summary>
        /// Gets the comment associated with this check-in.
        /// </summary>
        public string Comment { get; protected set; }

        /// <summary>
        /// Gets the application tag associated with this check-in.
        /// </summary>
        public string AppTag { get; protected set; }

        internal CheckInLocation(double latitude, double longitude, DateTime checkInDate, string placeName, string comment, string appTag)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.CheckInDate = checkInDate;
            this.PlaceName = placeName;
            this.Comment = comment;
            this.AppTag = appTag;
        }
    }
}
