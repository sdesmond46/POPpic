using System;
using System.Net;
using BuddyServiceClient;

namespace Buddy
{
    public class RegisteredDevice
    {
        protected AuthenticatedUser User { get; set; }

        public string DeviceUri { get; protected set; }
        public string GroupName { get; protected set; }
        public bool TilesEnabled { get; protected set; }
        public bool RawMessagesEnabled { get; protected set; }
        public bool ToastMessagesEnabled { get; protected set; }
        public DateTime LastUpdateDate { get; protected set; }
        public DateTime RegistrationDate { get; protected set; }
        public int UserID { get; protected set; }

        internal RegisteredDevice(InternalModels.DataContract_WPDeviceList device, AuthenticatedUser user)
        {
            if (device == null) throw new ArgumentNullException("device");
            if (user == null) throw new ArgumentNullException("user");

            this.UserID = Int32.Parse(device.UserID);
            this.DeviceUri = device.DeviceURI;
            this.GroupName = device.GroupName;
            this.TilesEnabled = Boolean.Parse(device.EnableTiles);
            this.RawMessagesEnabled = Boolean.Parse(device.EnableRawMessages);
            this.ToastMessagesEnabled = Boolean.Parse(device.EnableToastMessages);
            this.LastUpdateDate = device.DeviceModified;
            this.RegistrationDate = device.DeviceRegistered;
            this.User = user;
        }
    }
}
