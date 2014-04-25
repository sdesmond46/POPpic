using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BuddyServiceClient;
using System.Globalization;

namespace Buddy
{
    /// <summary>
    /// Represents an object that describes a single game score entry.
    /// </summary>
    public class GameScore
    {
        /// <summary>
        /// Gets the name of the board this score is related to.
        /// </summary>
        public string BoardName { get; protected set; }

        /// <summary>
        /// Gets the date this score was added.
        /// </summary>
        public DateTime AddedOn { get; protected set; }

        /// <summary>
        /// Gets the optional latitude for this score.
        /// </summary>
        public double Latitude { get; protected set; }

        /// <summary>
        /// Gets the optional longitude for this score.
        /// </summary>
        public double Longitude { get; protected set; }

        /// <summary>
        /// Gets the optioanl rank value for this score.
        /// </summary>
        public string Rank { get; protected set; }

        /// <summary>
        /// Gets the numeric value of the score entry.
        /// </summary>
        public double Score { get; protected set; }

        /// <summary>
        /// Gets the user ID that owns this score.
        /// </summary>
        public int UserID { get; protected set; }

        /// <summary>
        /// Gets the user name of the user who owns this score.
        /// </summary>
        public string UserName { get; protected set; }

        /// <summary>
        /// Gets the optional application tag for this score.
        /// </summary>
        public string AppTag { get; protected set; }

        internal GameScore(BuddyClient client, InternalModels.DataContract_GameUserScoreList score)
        {
            if (client == null)throw new ArgumentNullException("client");
            if (score == null) throw new ArgumentNullException("score");

            this.BoardName = score.ScoreBoardName;
            this.AddedOn = Convert.ToDateTime(score.ScoreDate, CultureInfo.InvariantCulture);
            this.Latitude = client.TryParseDouble(score.ScoreLatitude);
            this.Longitude = client.TryParseDouble(score.ScoreLongitude);
            this.Rank = score.ScoreRank;
            this.Score = Double.Parse(score.ScoreValue, CultureInfo.InvariantCulture);
            this.UserID = Int32.Parse(score.UserID);
            this.UserName = score.UserName;
            this.AppTag = score.ApplicationTag;
        }

        internal GameScore(BuddyClient client, InternalModels.DataContract_GameBoardScoreList score)
        {
            if (client == null)throw new ArgumentNullException("client");
            if (score == null) throw new ArgumentNullException("score");

            this.BoardName = score.ScoreBoardName;
            this.AddedOn = Convert.ToDateTime(score.ScoreDate, CultureInfo.InvariantCulture);
            this.Latitude = client.TryParseDouble(score.ScoreLatitude);
            this.Longitude = client.TryParseDouble(score.ScoreLongitude);
            this.Rank = score.ScoreRank;
            this.Score = Double.Parse(score.ScoreValue, CultureInfo.InvariantCulture);
            this.UserID = Int32.Parse(score.UserID);
            this.UserName = score.UserName;
            this.AppTag = score.ApplicationTag;
        }

        internal GameScore(BuddyClient client, InternalModels.DataContract_GameScoreSearchResults score)
        {
            if (client == null) throw new ArgumentNullException("client");
            if (score == null) throw new ArgumentNullException("score");

            this.BoardName = score.ScoreBoardName;
            this.AddedOn = Convert.ToDateTime(score.ScoreDate, CultureInfo.InvariantCulture);
            this.Latitude = client.TryParseDouble(score.ScoreLatitude);
            this.Longitude = client.TryParseDouble(score.ScoreLongitude);
            this.Rank = score.ScoreRank;
            this.Score = Double.Parse(score.ScoreValue, CultureInfo.InvariantCulture);
            this.UserID = Int32.Parse(score.UserID);
            this.UserName = score.UserName;
            this.AppTag = score.ApplicationTag;
        }
    }
}
