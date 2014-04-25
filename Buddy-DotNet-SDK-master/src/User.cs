using BuddyServiceClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Buddy
{
    /// <summary>
    /// Represents the gender of a user.
    /// </summary>
    public enum UserGender
    {
        Male, Female, Any
    }

    /// <summary>
    /// Represents the status of the user.
    /// </summary>
    public enum UserStatus
    {
        Single = 1,
        Dating = 2,
        Engaged = 3,
        Married = 4,
        Divorced = 5,
        Widowed = 6,
        OnTheProwl = 7,
        Any = -1
    }

    /// <summary>
    /// Represents a public user profile. Public user profiles are usually returned when looking at an AuthenticatedUser's friends or making a search with FindUser.
    /// <example>
    /// <code>
    ///     BuddyClient client = new BuddyClient("APPNAME", "APPPASS");
    ///     client.LoginAsync("username", "password", (user, state) => {
    ///     
    ///         // Return all users for this application.
    ///         user.FindUserAsync((users, state2) => { });
    ///     });
    /// </code>
    /// </example>
    /// </summary>
    public class User : BuddyBase
    {
		public User(BuddyClient client) : base(client) {
		}


        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets the system-wide unique ID of the user.
        /// </summary>
        public int ID { get; protected set; }

        /// <summary>
        /// Gets the gender of the user.
        /// </summary>
        public UserGender Gender { get; protected set; }

        /// <summary>
        /// Gets the optional application tag for the user.
        /// </summary>
        public string ApplicationTag { get; protected set; }

        /// <summary>
        /// Gets the latitude of the last check-in for this user.
        /// </summary>
        public double Latitude { get; protected set; }

        /// <summary>
        /// Gets the longitude of the last check-in for this user.
        /// </summary>
        public double Longitude { get; protected set; }

        /// <summary>
        /// Gets the last time this user logged on to the platform.
        /// </summary>
        public DateTime LastLoginOn { get; protected set; }

        /// <summary>
        /// Gets the profile picture for this user.
        /// </summary>
        public Uri ProfilePicture { get; protected set; }

        /// <summary>
        /// Gets the profile picture ID for this user.
        /// </summary>
        public string ProfilePictureID { get; protected set; }

        /// <summary>
        /// Gets the age of this user.
        /// </summary>
        public int Age { get; protected set; }

        /// <summary>
        /// Gets the status of the user.
        /// </summary>
        public UserStatus Status { get; protected set; }

        /// <summary>
        /// Gets the date this user was created.
        /// </summary>
        public DateTime CreatedOn { get; protected set; }

        /// <summary>
        /// If this user profile was returned from a search, gets the distance in kilometers from the search origin.
        /// </summary>
        public double DistanceInKilometers { get; protected set; }

        /// <summary>
        /// If this user profile was returned from a search, gets the distance in meters from the search origin.
        /// </summary>
        public double DistanceInMeters { get; protected set; }

        /// <summary>
        /// If this user profile was returned from a search, gets the distance in miles from the search origin.
        /// </summary>
        public double DistanceInMiles { get; protected set; }

        /// <summary>
        /// If this user profile was returned from a search, gets the distance in yards from the search origin.
        /// </summary>
        public double DistanceInYards { get; protected set; }

        /// <summary>
        /// Does this user have a friends request pending.
        /// </summary>
        public bool FriendRequestPending { get; protected set; }

        /// <summary>
        /// Add and remove gamescore for this user.
        /// </summary>
        public GameScores GameScores { get; protected set; }

        /// <summary>
        /// Add and remove GameStates for this user.
        /// </summary>
        public GameStates GameStates { get; protected set; }



        internal virtual string TokenOrId
        {
            get
            {
                return this.ID.ToString();
            }
        }

        internal User(BuddyClient client, int id)
            : base(client)
        {
            this.ID = id;
            this.GameScores = new GameScores(this.Client, null, this);
            this.GameStates = new GameStates(this.Client, this);
        }

        internal User(BuddyClient client, InternalModels.DataContract_PublicUserProfile publicProfile)
            : this(client, Int32.Parse(publicProfile.UserID))
        {
            this.ID = Int32.Parse(publicProfile.UserID);
            this.Name = publicProfile.UserName;
            this.Gender = (UserGender)Enum.Parse(typeof(UserGender), publicProfile.UserGender, true);
            this.ApplicationTag = publicProfile.UserApplicationTag;
            this.Latitude = this.Client.TryParseDouble(publicProfile.UserLatitude);
            this.Longitude = this.Client.TryParseDouble(publicProfile.UserLongitude);
            this.LastLoginOn = publicProfile.LastLoginDate;
            this.InitializeProfilePicture(publicProfile.ProfilePictureUrl);
            this.CreatedOn = publicProfile.CreatedDate;
            this.Status = (UserStatus)Int32.Parse(publicProfile.StatusID);
            this.Age = Int32.Parse(publicProfile.Age);
        }

        internal User(BuddyClient client, InternalModels.DataContract_ApplicationUserProfile applicationUserProfile)
            : this(client, Int32.Parse(applicationUserProfile.UserID))
        {
            this.Name = applicationUserProfile.UserName;
            this.ID = Int32.Parse(applicationUserProfile.UserID);
            this.Gender = (UserGender)Enum.Parse(typeof(UserGender), applicationUserProfile.UserGender, true);
            this.Latitude = this.Client.TryParseDouble(applicationUserProfile.UserLatitude);
            this.Longitude = this.Client.TryParseDouble(applicationUserProfile.UserLongitude);
            this.LastLoginOn = DateTime.Parse(applicationUserProfile.LastLoginDate, CultureInfo.InvariantCulture);
            this.InitializeProfilePicture(applicationUserProfile.ProfilePictureUrl);
            this.CreatedOn = DateTime.Parse(applicationUserProfile.CreatedDate, CultureInfo.InvariantCulture);
            this.Status = (UserStatus)Int32.Parse(applicationUserProfile.StatusID);
            this.Age = Int32.Parse(applicationUserProfile.Age);
        }

        internal User(BuddyClient client, InternalModels.DataContract_FriendList publicProfile, int userId)
            : this(client, Int32.Parse(publicProfile.FriendID) == userId ? Int32.Parse(publicProfile.UserID) : Int32.Parse(publicProfile.FriendID))
        {
            this.ID = Int32.Parse(publicProfile.UserID);
            this.Name = publicProfile.UserName;
            this.Gender = (UserGender)Enum.Parse(typeof(UserGender), publicProfile.UserGender, true);
            this.ApplicationTag = publicProfile.UserApplicationTag;
            this.Latitude = this.Client.TryParseDouble(publicProfile.UserLatitude);
            this.Longitude = this.Client.TryParseDouble(publicProfile.UserLongitude);
            this.LastLoginOn = publicProfile.LastLoginDate;
            this.InitializeProfilePicture(publicProfile.ProfilePictureUrl);
            this.CreatedOn = publicProfile.CreatedDate;
            this.Status = (UserStatus)Int32.Parse(publicProfile.StatusID);
            this.Age = Int32.Parse(publicProfile.Age);
            this.FriendRequestPending = publicProfile.Status == "0";
        }

        internal User(BuddyClient client, InternalModels.DataContract_FriendRequests publicProfile, int userId)
            : this(client, Int32.Parse(publicProfile.FriendID) == userId ? Int32.Parse(publicProfile.UserID) : Int32.Parse(publicProfile.FriendID))
        {
            this.ID = int.Parse(publicProfile.FriendID);
            this.Name = publicProfile.UserName;
            this.Gender = (UserGender)Enum.Parse(typeof(UserGender), publicProfile.UserGender, true);
            this.ApplicationTag = publicProfile.UserApplicationTag;
            this.Latitude = this.Client.TryParseDouble(publicProfile.UserLatitude);
            this.Longitude = this.Client.TryParseDouble(publicProfile.UserLongitude);
            this.LastLoginOn = publicProfile.LastLoginDate;
            this.InitializeProfilePicture(publicProfile.ProfilePictureUrl);
            this.CreatedOn = publicProfile.CreatedDate;
            this.Status = (UserStatus)Int32.Parse(publicProfile.StatusID);
            this.Age = Int32.Parse(publicProfile.Age);
        }

        internal User(BuddyClient client, InternalModels.DataContract_SearchPeople publicProfile)
            : base(client)
        {
            this.Name = publicProfile.UserName;
            this.ID = Int32.Parse(publicProfile.UserID);
            this.Gender = (UserGender)Enum.Parse(typeof(UserGender), publicProfile.UserGender, true);
            this.ApplicationTag = publicProfile.UserApplicationTag;
            this.Latitude = this.Client.TryParseDouble(publicProfile.UserLatitude);
            this.Longitude = this.Client.TryParseDouble(publicProfile.UserLongitude);
            //this.LastLoginOn = publicProfile.LastLoginDate;
            this.InitializeProfilePicture(publicProfile.ProfilePictureUrl);
            //this.CreatedOn = publicProfile.CreatedDate;
            this.Status = (UserStatus)Int32.Parse(publicProfile.StatusID);
            this.Age = Int32.Parse(publicProfile.Age);
            this.DistanceInKilometers = this.Client.TryParseDouble(publicProfile.DistanceInKilometers);
            this.DistanceInMeters = this.Client.TryParseDouble(publicProfile.DistanceInMeters);
            this.DistanceInMiles = this.Client.TryParseDouble(publicProfile.DistanceInMiles);
            this.DistanceInYards = this.Client.TryParseDouble(publicProfile.DistanceInYards);
        }

        protected void InitializeProfilePicture(string profilePictureUrlOrId)
        {
            Uri parsedUri;
            if (Uri.TryCreate(profilePictureUrlOrId, UriKind.Absolute, out parsedUri))
            {
                this.ProfilePicture = parsedUri;
            }
            else
            {
                this.ProfilePictureID = profilePictureUrlOrId;
            }
        }

        protected string GetProfilePictureString()
        {
            if (this.ProfilePicture != null)
            {
                return this.ProfilePicture.ToString();
            }
            else
            {
                return this.ProfilePictureID ?? "";
            }
        }

        public override string ToString()
        {
            return "[" + this.ID + "], Name: " + this.Name + ", Gender: " + Enum.GetName(typeof(UserGender), this.Gender) + ", Tag: " + this.ApplicationTag +
                ", Latitude: " + this.Latitude.ToString() + ", Longitude: " + this.Longitude.ToString() + ", CreatedOn: " + this.CreatedOn.ToString() +
                ", LastLoginOn: " + this.LastLoginOn.ToString() + ", ProfilePicture: " + this.GetProfilePictureString() + ", Age: " + this.Age + ", Status: " + this.Status;
        }

       

        /// <summary>
        /// Gets a list of profile photos for this user.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is a list of profile photos.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetProfilePhotosAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetProfilePhotosAsync(Action<List<PicturePublic>, BuddyCallbackParams> callback, object state = null) { GetProfilePhotosInternal((bcr) => { if (callback == null) return; callback(bcr.Result, new BuddyCallbackParams(bcr.Error)); }); return null; }

        

        internal void GetProfilePhotosInternal(Action<BuddyCallResult<List<PicturePublic>>> callback)
        {
            this.Client.Service.Pictures_ProfilePhoto_GetAll(this.Client.AppName, this.Client.AppPassword, this.ID.ToString(), (bcr) =>
            {
                var result = bcr.Result;

                if (bcr.Error != BuddyError.None)
                {
                    callback(BuddyResultCreator.Create<List<PicturePublic>>(null, bcr.Error));
                    return;
                }
                List<PicturePublic> pictures = new List<PicturePublic>();
                foreach (var photo in result) pictures.Add(new PicturePublic(this.Client, photo.FullPhotoURL, photo.ThumbnailPhotoURL,
                    this.Client.TryParseDouble(photo.Latitude), this.Client.TryParseDouble(photo.Longitude), photo.PhotoComment,
                    null, photo.AddedDateTime, Int32.Parse(photo.PhotoID), this));
                { callback(BuddyResultCreator.Create(pictures, bcr.Error)); return; };
            }); return;
        }

#if AWAIT_SUPPORTED
           /// <summary>
        /// Gets a list of profile photos for this user.
        /// </summary>
        /// <returns>A Task&lt;IEnumerable&lt;PicturePublic&gt; &gt;that can be used to monitor progress on this call.</returns>
        public System.Threading.Tasks.Task<IEnumerable<PicturePublic>> GetProfilePhotosAsync()
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<IEnumerable<PicturePublic>>();
            GetProfilePhotosInternal((bcr) =>
            {
                if (bcr.Error != BuddyServiceClient.BuddyError.None)
                {
                    tcs.TrySetException(new BuddyServiceException(bcr.Error));
                }
                else
                {
                    tcs.TrySetResult(bcr.Result);
                }
            });
            return tcs.Task;
        }

#endif
    }
}
