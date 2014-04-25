using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BuddyServiceClient;
using System.Globalization;

namespace Buddy
{
    /// <summary>
    /// Represents a game player object.
    /// </summary>
    public class GamePlayer
    {
       
        /// <summary>
        /// Gets the name of the player.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets the date the player was created.
        /// </summary>
        public DateTime CreatedOn { get; protected set; }

        /// <summary>
        /// Gets the name of the board the player belongs to.
        /// </summary>
        public string BoardName { get; protected set; }

        /// <summary>
        /// Gets the optional application tag for the player.
        /// </summary>
        public string ApplicationTag { get; protected set; }

        /// <summary>
        /// Gets the latitude where the player was created.
        /// </summary>
        public double Latitude { get; protected set; }
        
        /// <summary>
        /// Gets the longitude where the player was created.
        /// </summary>
        public double Longitude { get; protected set; }

        /// <summary>
        /// Gets the UserID of the user this player is tied to.
        /// </summary>
        public int UserID { get; protected set; }

        /// <summary>
        /// Gets the distance in kilo-meters from the given origin in the Metadata Search method.
        /// </summary>
        public double DistanceInKilometers { get; protected set; }

        /// <summary>
        /// Gets the distance in meters from the given origin in the Metadata Search method.
        /// </summary>
        public double DistanceInMeters { get; protected set; }

        /// <summary>
        /// Gets the distance in miles from the given origin in the Metadata Search method.
        /// </summary>
        public double DistanceInMiles { get; protected set; }

        /// <summary>
        /// Gets the distance in yards from the given origin in the Metadata Search method.
        /// </summary>
        public double DistanceInYards { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public string Rank { get; protected set; }

        internal GamePlayer(BuddyClient client, AuthenticatedUser user, InternalModels.DataContract_GamePlayerInfo info)
        {
            if (client == null) throw new ArgumentNullException("client");
            if (user == null) throw new ArgumentNullException("user");
            if (info == null) throw new ArgumentNullException("info");

            this.UserID = user.ID;
            
            this.ApplicationTag = info.ApplicationTag;
            this.BoardName = info.PlayerBoardName;
            this.CreatedOn = Convert.ToDateTime(info.PlayerDate, CultureInfo.InvariantCulture);
            this.Latitude = client.TryParseDouble(info.PlayerLatitude);
            this.Longitude = client.TryParseDouble(info.PlayerLongitude);
            this.Name = info.PlayerName;
            this.Rank = info.PlayerRank;
        }

        internal GamePlayer(BuddyClient client, AuthenticatedUser user, InternalModels.DataContract_GamePlayerSearchResults info)
        {
            if (client == null) throw new ArgumentNullException("client");
            if (user == null) throw new ArgumentNullException("user");
            if (info == null) throw new ArgumentNullException("info");

            this.UserID = Int32.Parse(info.UserID);
            this.ApplicationTag = info.ApplicationTag;
            this.BoardName = info.PlayerBoardName;
            this.CreatedOn = Convert.ToDateTime(info.PlayerDate, CultureInfo.InvariantCulture);
            this.Latitude = client.TryParseDouble(info.PlayerLatitude);
            this.Longitude = client.TryParseDouble(info.PlayerLongitude);
            this.Name = info.PlayerName;
            this.DistanceInKilometers = client.TryParseDouble(info.DistanceInKilometers);
            this.DistanceInMeters = client.TryParseDouble(info.DistanceInMeters);
            this.DistanceInMiles = client.TryParseDouble(info.DistanceInMiles);
            this.DistanceInYards = client.TryParseDouble(info.DistanceInYards);
            this.Rank = info.PlayerRank;
        }
    }
}
