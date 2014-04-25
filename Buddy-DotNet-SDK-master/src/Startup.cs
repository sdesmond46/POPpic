using BuddyServiceClient;
using System;
using System.Globalization;

namespace Buddy
{
    /// <summary>
    /// Represents a single, named startup in the Buddy system.
    /// </summary>
    public class Startup : BuddyBase
    {


        /// <summary>
        /// Gets the latitude of the center of the specified metro area.
        /// </summary>
        public decimal CenterLat { get; protected set; }

        /// <summary>
        /// Gets the longitude of the center of the specified metro area.
        /// </summary>
        public decimal CenterLong { get; protected set; }

        /// <summary>
        /// Gets the city in which the startup is located.
        /// </summary>
        public string City { get; protected set; }

        /// <summary>
        /// Gets the crunchbase.com URL of the startup.
        /// </summary>
        public string CrunchBaseUrl { get; protected set; }

        /// <summary>
        /// Gets the custom data of the startup.
        /// </summary>
        public string CustomData { get; protected set; }

        /// <summary>
        /// Gets the distance in kilometers from the center of the specified metro area that the startup is located.
        /// </summary>
        public double DistanceInKilometers { get; protected set; }

        /// <summary>
        /// Gets the distance in meters from the center of the specified metro area that the startup is located.
        /// </summary>
        public double DistanceInMeters { get; protected set; }

        /// <summary>
        /// Gets the distance in miles from the center of the specified metro area that the startup is located.
        /// </summary>
        public double DistanceInMiles { get; protected set; }

        /// <summary>
        /// Gets the distance in yards from the center of the specified metro area that the startup is located.
        /// </summary>
        public double DistanceInYards { get; protected set; }

        /// <summary>
        /// Gets the number of employees employed by the startup.
        /// </summary>
        public int EmployeeCount { get; protected set; }

        /// <summary>
        /// Gets URL of the startup's Facebook page.
        /// </summary>
        public string FacebookURL { get; protected set; }

        /// <summary>
        /// Gets the source of the funds raised by the startup.
        /// </summary>
        public string FundingSource { get; protected set; }

        /// <summary>
        /// Gets the URL of the statup's home page.
        /// </summary>
        public string HomePageURL { get; protected set; }

        /// <summary>
        /// Gets the industry of the startup.
        /// </summary>
        public string Industry { get; protected set; }

        /// <summary>
        /// Gets the URL of the startup's LinkedIn page.
        /// </summary>
        public string LinkedinURL { get; protected set; }

        /// <summary>
        /// Gets the logo URL of the startup.
        /// </summary>
        public string LogoURL { get; protected set; }

        /// <summary>
        /// Gets the metro area in which the startup is located.
        /// </summary>
        public string MetroLocation { get; protected set; }

        /// <summary>
        /// Gets the phone number of the startup.
        /// </summary>
        public string PhoneNumber { get; protected set; }

        /// <summary>
        /// Gets the company name of the startup.
        /// </summary>
        public string StartupName { get; protected set; }

        /// <summary>
        /// Gets the state in which the startup is located.
        /// </summary>
        public string State { get; protected set; }

        /// <summary>
        /// Gets the address of the startup.
        /// </summary>
        public string StreetAddress { get; protected set; }

        /// <summary>
        /// Gets the unique ID assigned to the startup.
        /// </summary>
        public long StartupID { get; protected set; }

        /// <summary>
        /// Gets the amount of money that the startup as raised.
        /// </summary>
        public decimal TotalFundingRaised { get; protected set; }

        /// <summary>
        /// Gets the startup's Twitter URL.
        /// </summary>
        public string TwitterURL { get; protected set; }

        /// <summary>
        /// Gets the zip or postal code of the startup.
        /// </summary>
        public double ZipPostal { get; protected set; }

        internal Startup(BuddyClient client, AuthenticatedUser user, InternalModels.DataContract_SearchStartups startup)
            : base(client, user)
        {


            this.CenterLat = decimal.Parse(startup.CenterLat, CultureInfo.InvariantCulture);
            this.CenterLong = decimal.Parse(startup.CenterLong, CultureInfo.InvariantCulture);
            this.City = startup.City;
            this.CrunchBaseUrl = startup.CrunchBaseUrl;
            this.CustomData = startup.CustomData;
            this.DistanceInKilometers = string.IsNullOrEmpty(startup.DistanceInKilometers) ? 0 : double.Parse(startup.DistanceInKilometers, CultureInfo.InvariantCulture);
            this.DistanceInMeters = string.IsNullOrEmpty(startup.DistanceInMeters) ? 0 : double.Parse(startup.DistanceInMeters, CultureInfo.InvariantCulture);
            this.DistanceInMiles = string.IsNullOrEmpty(startup.DistanceInMiles) ? 0 : double.Parse(startup.DistanceInMiles, CultureInfo.InvariantCulture);
            this.DistanceInYards = string.IsNullOrEmpty(startup.DistanceInYards) ? 0 : double.Parse(startup.DistanceInYards, CultureInfo.InvariantCulture);
            this.EmployeeCount = string.IsNullOrEmpty(startup.EmployeeCount) ? 0 : int.Parse(startup.EmployeeCount, CultureInfo.InvariantCulture);
            this.FacebookURL = startup.FacebookURL;
            this.FundingSource = startup.FundingSource;
            this.HomePageURL = startup.HomePageURL;
            this.Industry = startup.Industry;
            this.LinkedinURL = startup.LinkedinURL;
            this.LogoURL = startup.LogoURL;
            this.MetroLocation = startup.MetroLocation;
            this.PhoneNumber = startup.PhoneNumber;
            this.StartupID = long.Parse(startup.StartupID);
            this.StartupName = startup.StartupName;
            this.StreetAddress = startup.StreetAddress;
            this.TotalFundingRaised = string.IsNullOrEmpty(startup.TotalFundingRaised) ? 0 : decimal.Parse(startup.TotalFundingRaised, CultureInfo.InvariantCulture);
            this.TwitterURL = startup.TwitterURL;
            this.ZipPostal = double.Parse(startup.ZipPostal, CultureInfo.InvariantCulture);
        }
    }
}
