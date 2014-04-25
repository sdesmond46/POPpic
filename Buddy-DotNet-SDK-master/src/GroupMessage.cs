using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BuddyServiceClient;
using System.Globalization;

namespace Buddy
{
    /// <summary>
    /// Represents a message that was sent to a group of users through AuthenticatedUser.Messages.Groups.SendMessageAsync.
    /// </summary>
    public class GroupMessage
    {
        /// <summary>
        /// Gets the DateTime the message was sent.
        /// </summary>
        public DateTime DateSent { get; protected set; }

        /// <summary>
        /// Gets the user ID of the user that sent the message to the group.
        /// </summary>
        public int FromUserID { get; protected set; }

        /// <summary>
        /// Gets the Message group that the message was sent to.
        /// </summary>
        public MessageGroup Group { get; protected set; }

        /// <summary>
        /// Gets the text value of the message.
        /// </summary>
        public string Text { get; protected set; }

        /// <summary>
        /// Gets the optional latitude from where the message was sent.
        /// </summary>
        public double Latitude { get; protected set; }
        
        /// <summary>
        /// Gets the optional longitude from where the message was sent.
        /// </summary>
        public double Longitude { get; protected set; }

        internal GroupMessage(BuddyClient client, InternalModels.DataContract_GroupMessage msg, MessageGroup group)
        {
            this.DateSent = Convert.ToDateTime(msg.SentDateTime, CultureInfo.InvariantCulture); 
            this.Group = group;

            this.Latitude = client.TryParseDouble(msg.Latitude);
            this.Longitude = client.TryParseDouble(msg.Longitude);

            this.FromUserID = Int32.Parse(msg.FromUserID);
            this.Text = msg.MessageText;
        }
    }
}
