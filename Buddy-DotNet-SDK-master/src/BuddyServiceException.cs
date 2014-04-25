using System;
using System.Net;

namespace Buddy
{
    /// <summary>
    /// Occurs when there is an error processing the service request.
    /// </summary>
    public class BuddyServiceException : Exception
    {
        /// <summary>
        /// The error that occured.
        /// </summary>
        public string Error { get; protected set; }

        internal BuddyServiceException(BuddyServiceClient.BuddyError err)
        {
            Error = err.ToString();
        }

        internal BuddyServiceException(string error)
        {
            this.Error = error;
        }
    }
}
