using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Buddy
{
    /// <summary>
    /// Represents an object that wraps a number of different callback return values.
    /// </summary>
    public class BuddyCallbackParams
    {
        /// <summary>
        /// Gets the exception in case there was an error while processing the callback.
        /// </summary>
        public Exception Exception { get; protected set; }

        /// <summary>
        /// Gets whether the call was successful or not.
        /// </summary>
        public bool Completed { get; protected set; }
        
        /// <summary>
        /// Gets the user defined state object passed to the Async call.
        /// </summary>
        public object State { get; protected set; }

        /// <summary>
        /// Gets the IAsyncResult structure for this async call.
        /// </summary>
        public IAsyncResult AsyncResult { get; protected set; }

        internal BuddyCallbackParams(bool succeeded, Exception ex, object state, IAsyncResult result)
        {
            this.Completed = succeeded;
            this.Exception = ex;
            this.State = state;
            this.AsyncResult = result;
        }
        internal BuddyCallbackParams(BuddyServiceClient.BuddyError buddyError)
        {
            if (buddyError != BuddyServiceClient.BuddyError.None)
            {
                Exception = new BuddyServiceException(buddyError.ToString());
                Completed = buddyError != BuddyServiceClient.BuddyError.UnknownServiceError ;
            }
            else
            {
                Completed = true;
            }
        }
    }
}
