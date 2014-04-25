using System;
using System.Net;
using BuddyServiceClient;

namespace Buddy
{
    public class RegisteredDeviceWin8
    {
        protected AuthenticatedUser User { get; set; }

        public string DeviceURI { get; protected set; }
        public string GroupName { get; protected set; }
        public string ClientID { get; protected set; }
        public string ClientSecret { get; protected set; }
        public DateTime LastUpdateDate { get; protected set; }
        public DateTime RegistrationDate { get; protected set; }
        public int UserID { get; protected set; }

        internal RegisteredDeviceWin8(InternalModels.DataContract_Win8DeviceList device, AuthenticatedUser user)
        {
            if (device == null) throw new ArgumentNullException("device");
            if (user == null) throw new ArgumentNullException("user");

            this.UserID = Int32.Parse(device.UserID);
            this.DeviceURI = device.DeviceURI;
            this.GroupName = device.GroupName;
            this.LastUpdateDate = device.DeviceModified;
            this.RegistrationDate = device.DeviceRegistered;
            this.ClientID = device.ClientID;
            this.ClientSecret = device.ClientSecret;
            this.User = user;
        }
    }
}
