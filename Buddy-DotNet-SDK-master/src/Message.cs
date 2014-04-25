using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BuddyServiceClient;

namespace Buddy
{
    /// <summary>
    /// Represents a single message that one user sent to another.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Gets the DateTime the message was sent.
        /// </summary>
        public DateTime DateSent { get; protected set; }

        /// <summary>
        /// Gets the ID of the user who sent the message.
        /// </summary>
        public int FromUserID { get; protected set; }

        /// <summary>
        /// Gets the ID of the user who received the message.
        /// </summary>
        public int ToUserID { get; protected set; }

        /// <summary>
        /// Gets the text value of the message.
        /// </summary>
        public string Text { get; protected set; }

        internal Message(InternalModels.DataContract_Messages msg, int toId)
        {
            this.DateSent = msg.DateSent;
            this.FromUserID = Int32.Parse(msg.FromUserID);
            this.ToUserID = toId;
            this.Text = msg.MessageString;
        }

        internal Message(InternalModels.DataContract_MessagesFromMe msg, int fromId)
        {
            this.DateSent = msg.DateSent;
            this.FromUserID = fromId;
            this.ToUserID = Int32.Parse(msg.ToUserID);
            this.Text = msg.MessageString;
        }
    }
}
