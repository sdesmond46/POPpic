using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BuddyServiceClient;

namespace Buddy
{
    /// <summary>
    /// Represents a single game state object.
    /// </summary>
    public class GameState
    {
        /// <summary>
        /// Gets the optional application tag for this GameState.
        /// </summary>
        public string AppTag { get; protected set; }

        /// <summary>
        /// Gets the date this GameState was created.
        /// </summary>
        public DateTime AddedOn { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        private int ID { get;  set; }

        /// <summary>
        /// Get the Key for this GameState object.
        /// </summary>
        public string Key { get; protected set; }


        /// <summary>
        /// Gets the the value for this GameState object
        /// </summary>
        public string Value { get; protected set; }

        internal GameState(InternalModels.DataContract_GameStateObject gs)
        {
            this.AppTag = gs.AppTag;
            this.Key = gs.StateKey;
            this.ID = Int32.Parse(gs.StateID);
            this.Value = gs.StateValue;
            DateTime dt;
            if (DateTime.TryParse(gs.StateDateTime, out dt))
            {
                this.AddedOn = dt;
            }

        }

       
    }
}
