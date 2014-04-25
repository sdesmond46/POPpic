using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using BuddyServiceClient;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;

namespace Buddy
{
    public class SocialAuthenticatedUser : AuthenticatedUser
    {
        public bool IsNew { get; protected set; }

        public SocialAuthenticatedUser(BuddyClient client, AuthenticatedUser user, bool isNew)
            : base(client, user)
        {
            IsNew = isNew;
        }
    }

    /// <summary>
    /// Represents a user that has been authenticated with the Buddy Platform. Use this object to interact with the service on behalf of the user.
    /// <example>
    /// <code>
    ///     BuddyClient client = new BuddyClient("APPNAME", "APPPASS");
    ///
    ///     AuthenticatedUser user;
    ///     client.CreateUserAsync((u, state) => {
    ///         user = u;
    ///     }, "username", "password");
    ///     
    ///     AuthenticatedUser user2;
    ///     client.LoginAsync((u, state) => {
    ///         user2 = u;
    ///     }, "username2", "password2");
    /// </code>
    /// </example>
    /// </summary>
    public class AuthenticatedUser : User
    {
        /// <summary>
        /// Gets the unique user token that is the secret used to log-in this user. Each user has a unique ID, a secret user token and a user/pass combination.
        /// </summary>
        public string Token { get; protected set; }

        /// <summary>
        /// Gets the email of the user. Can be an empty string or null.
        /// </summary>
        public string Email { get; protected set; }

        /// <summary>
        /// Gets whether location fuzzing is enabled. When enabled any reported locations for this user will be randomized for a few miles. This is a security feature
        /// that makes it difficult for users to track each other.
        /// </summary>
        public bool LocationFuzzing { get; protected set; }

        /// <summary>
        /// Gets whether celebrity mode is enabled for this user. When enabled the user will be hidden from all searches in the system.
        /// </summary>
        public bool CelebrityMode { get; protected set; }

        /// <summary>
        /// Gets the collection of user metadata. Note that the actual metadata is loaded on demand when you call the All or Get method.
        /// </summary>
        public UserMetadata Metadata { get; protected set; }

        /// <summary>
        /// Gets the collection of identity values for the user.
        /// </summary>
        public Identity IdentityValues { get; protected set; }

        /// <summary>
        /// Gets the collection of photo albums for this user. Note that the actual album information is loaded on demand when you call the All or Get method.
        /// </summary>
        public PhotoAlbums PhotoAlbums { get; protected set; }

        /// <summary>
        /// Gets the collection of virtual albums for this users. All virtual albums are owned by a single user, however any user may add existing photos to the album. Only the owner of the virtual album can delete the album.
        /// </summary>
        public VirtualAlbums VirtualAlbums { get; protected set; }

        /// <summary>
        /// Gets an object that can be used to register a device for push notifications, send notifications or query the state of devices and groups.
        /// </summary>
        public Notifications PushNotifications { get; protected set; }

        /// <summary>
        /// Gets an object that can be user for search for locations around the user (places, not other users).
        /// </summary>
        public Places Places { get; protected set; }

        /// <summary>
        /// Gets the collection of friends for this user. Note that the actual friends information is loaded on demand when you call the All or Get method.
        /// </summary>
        public Friends Friends { get; protected set; }

        /// <summary>
        /// Gets an object that can be used to send or receive messages, create message groups, etc.
        /// </summary>
        public Messages Messages { get; protected set; }

        /// <summary>
        /// Gets an object that can be used for search for startups around the user (startups, not other users).
        /// </summary>
        public Startups Startups { get; protected set; }

        /// <summary>
        /// Gets an object that can be used for commerce for the user.
        /// </summary>
        public Commerce Commerce { get; protected set; }

        /// <summary>
        /// Gets an object that can be used to manipulate game players for the user.
        /// </summary>
        public GamePlayers GamePlayers { get; protected set; }

        /// <summary>
        /// Gets an object that can be used for Blob for the user.
        /// </summary>
        public Blobs Blobs { get; protected set; }

        /// <summary>
        /// Gets an object that can be used for Video for the user.
        /// </summary>
        public Videos Videos { get; protected set; }

        internal override string TokenOrId {
            get {
                return this.Token;
            }
        }

        internal AuthenticatedUser (string token, InternalModels.DataContract_FullUserProfile profile, BuddyClient client)
            : base(client, Int32.Parse(profile.UserID))
        {
            if (client == null)
                throw new ArgumentNullException ("client");
            if (String.IsNullOrEmpty (token))
                throw new ArgumentException ("Can't be null or empty.", "token");

            this.Token = token;
            this.Metadata = new UserMetadata (client, token);
            this.IdentityValues = new Identity (client, Token);
            this.PhotoAlbums = new PhotoAlbums (client, this);
            this.VirtualAlbums = new VirtualAlbums (client, this);
            this.Friends = new Friends (client, this);
            this.PushNotifications = new Notifications (this.Client, this);
            this.Places = new Places (this.Client, this);
            this.Messages = new Messages (this.Client, this);
            this.Startups = new Startups (this.Client, this);
            this.Commerce = new Commerce (this.Client, this);
            this.GamePlayers = new GamePlayers (this.Client, this);

            this.Blobs = new Blobs(this.Client, this);
            this.Videos = new Videos(this.Client, this);

            this.UpdateFromProfile (profile);

            this.GameScores = new GameScores (this.Client, this, null);
        }

        internal AuthenticatedUser (BuddyClient client, InternalModels.DataContract_ApplicationUserProfile profile)
            : base(client, Int32.Parse(profile.UserID))
        {
            //this.Token = profile.user; <-- removed from contract (in ws) for security reasons 
            this.Token = string.Empty;
            this.Metadata = new UserMetadata (client, this.Token);
            this.IdentityValues = new Identity (client, this.Token);
            this.PhotoAlbums = new PhotoAlbums (client, this);
            this.VirtualAlbums = new VirtualAlbums (client, this);
            this.Friends = new Friends (client, this);
            this.PushNotifications = new Notifications (this.Client, this);
            this.Places = new Places (this.Client, this);
            this.Messages = new Messages (this.Client, this);
            this.Startups = new Startups (this.Client, this);
            this.Commerce = new Commerce (this.Client, this);
            this.GamePlayers = new GamePlayers (this.Client, this);

            this.Blobs = new Blobs(this.Client, this);
            this.Videos = new Videos(this.Client, this);

            this.Name = profile.UserName;
            this.ID = Int32.Parse (profile.UserID);
            this.Gender = (UserGender)Enum.Parse (typeof(UserGender), profile.UserGender, true);
            this.Latitude = this.Client.TryParseDouble (profile.UserLatitude);
            this.Longitude = this.Client.TryParseDouble (profile.UserLongitude);

            this.LastLoginOn = Convert.ToDateTime (profile.LastLoginDate, CultureInfo.InvariantCulture); //fixes bug where DateTime.Parse was blowing up on non-US phones

            this.InitializeProfilePicture (profile.ProfilePictureUrl);

            this.CreatedOn = Convert.ToDateTime (profile.CreatedDate, CultureInfo.InvariantCulture); //fixes bug where DateTime.Parse was blowing up on non-US phones

            this.Status = (UserStatus)Int32.Parse (profile.StatusID);
            this.Age = Int32.Parse (profile.Age);
            this.Email = profile.UserEmail;
            this.LocationFuzzing = Boolean.Parse (profile.LocationFuzzing);
            this.CelebrityMode = Boolean.Parse (profile.CelebMode);
        }

        internal AuthenticatedUser(BuddyClient client, AuthenticatedUser user) : base(client, user.ID)
        {
            this.Age = user.Age;
            this.ApplicationTag = user.ApplicationTag;
            this.Blobs = user.Blobs;
            this.CelebrityMode = user.CelebrityMode;
            this.Commerce = user.Commerce;
            this.CreatedOn = user.CreatedOn;
            this.DistanceInKilometers = user.DistanceInKilometers;
            this.DistanceInMeters = user.DistanceInMeters;
            this.DistanceInMiles = user.DistanceInMiles;
            this.DistanceInYards = user.DistanceInYards;
            this.Email = user.Email;
            this.FriendRequestPending = user.FriendRequestPending;
            this.Friends = user.Friends;
            this.GamePlayers = user.GamePlayers;
            this.GameScores = user.GameScores;
            this.GameStates = user.GameStates;
            this.Gender = user.Gender;
            this.ID = user.ID;
            this.IdentityValues = user.IdentityValues;
            this.LastLoginOn = user.LastLoginOn;
            this.Latitude = user.Latitude;
            this.LocationFuzzing = user.LocationFuzzing;
            this.Longitude = user.Longitude;
            this.Messages = user.Messages;
            this.Metadata = user.Metadata;
            this.Name = user.Name;
            this.PhotoAlbums = user.PhotoAlbums;
            this.Places = user.Places;
            this.ProfilePicture = user.ProfilePicture;
            this.ProfilePictureID = user.ProfilePictureID;
            this.PushNotifications = user.PushNotifications;
            this.Startups = user.Startups;
            this.Status = user.Status;
            this.Token = user.Token;
            this.Videos = user.Videos;
            this.VirtualAlbums = user.VirtualAlbums;
        }

        internal void UpdateFromProfile (InternalModels.DataContract_FullUserProfile profile)
        {
            this.Name = profile.UserName;
            this.ID = Int32.Parse (profile.UserID);
            this.Gender = (UserGender)Enum.Parse (typeof(UserGender), profile.UserGender, true);
            this.ApplicationTag = profile.UserApplicationTag;
            this.Latitude = this.Client.TryParseDouble (profile.UserLatitude);
            this.Longitude = this.Client.TryParseDouble (profile.UserLongitude);
            this.LastLoginOn = Convert.ToDateTime (profile.LastLoginDate, CultureInfo.InvariantCulture);
            this.InitializeProfilePicture (profile.ProfilePictureUrl);
            this.CreatedOn = Convert.ToDateTime (profile.CreatedDate, CultureInfo.InvariantCulture);
            this.Status = (UserStatus)Int32.Parse (profile.StatusID);
            this.Age = Int32.Parse (profile.Age);
            this.Email = profile.UserEmail;
            this.LocationFuzzing = Boolean.Parse (profile.LocationFuzzing);
            this.CelebrityMode = Boolean.Parse (profile.CelebMode);
        }

        public override string ToString ()
        {
            return base.ToString () + ", Email: " + this.Email + ", LocationFuzzing: " + this.LocationFuzzing + ", CelebrityMode: " + this.CelebrityMode;
        }





        /// <summary>
        /// Find the public profile of a user from their unique User ID. This method can be used to find any user associated with this Application.
        /// </summary>
        /// <param name="id">The ID of the user, must be bigger than 0.</param>
        /// <param name="callback">The async callback to call on success or error. The first parameter is the user account associated with the ID.</param>
        /// <exception cref="Buddy.BuddyServiceException">With value: InvalidUserId, when the user ID doesn't exist in the system.</exception>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of FindUserAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult FindUserAsync (Action<User, BuddyCallbackParams> callback, int id, object state = null)
        {
            FindUserInternal (id, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void FindUserInternal (int id, Action<BuddyCallResult<User>> callback)
        {
            if (id <= 0)
                throw new ArgumentException ("Can't be smaller or equal to zero.", "id");

            Client.Service.UserAccount_Profile_GetFromUserID (Client.AppName, Client.AppPassword, this.Token, id.ToString (), (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None || result.Length == 0)
                {
                    callback(BuddyResultCreator.Create<User>(null, bcr.Error));
                    return;
                }

                callback(BuddyResultCreator.Create(new User(this.Client, result[0]), bcr.Error));
                
                ;
            });
            return;

        }

        internal void FindUserInternal(string userName, Action<BuddyCallResult<User>> callback)
        {
            if (String.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("Can't be smaller or equal to zero.", "id");
            }

            Client.Service.UserAccount_Profile_GetFromUserName(Client.AppName, Client.AppPassword, this.Token, userName, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None || result.Length == 0)
                {
                    callback(BuddyResultCreator.Create<User>(null, bcr.Error));
                    return;
                }
                
                callback(BuddyResultCreator.Create(new User(this.Client, result[0]), bcr.Error));
                
            
            });
            return;
        }


        /// <summary>
        /// Find the public profiles of all users that match the serch parameters.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is a list of users that was found by this search, or an empty list if there are no results.</param>
        /// <param name="latitude">The latitude of the position to search from. Must be a value between -90.0 and 90.0.</param>
        /// <param name="longitude">The Longitude of the position to search from. Must be a value between -180.0 and 180.0.</param>
        /// <param name="searchDistance">The distance in meters from the specified latitude/longitude to search for results. To ignore this distance pass in 40075000 (the circumferance of the earth).</param>
        /// <param name="recordLimit">The maximum number of users to return with this search.</param>
        /// <param name="gender">The gender of the users, use UserGender.Any to search for both.</param>
        /// <param name="ageStart">Specifies the starting age for the range of ages to search in. The value must be >= 0.</param>
        /// <param name="ageStop">Specifies the ending age for the range of ages to search in. The value must be > ageStart.</param>
        /// <param name="status">The status of the users to search for. Use UserStatus.Any to ignore this parameter.</param>
        /// <param name="checkinsWithinDays">Filter for users who have checked-in in the past 'checkinsWithinDays' number of days.</param>
        /// <param name="appTag">Search for the custom appTag that was stored with the user.</param>
        /// <exception cref="System.ArgumentException">When latitude or longitude are incorrect.</exception>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of FindUserAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult FindUserAsync (Action<List<User>, BuddyCallbackParams> callback, double latitude = 0.0, double longitude = 0.0,
                                             uint searchDistance = Int32.MaxValue, uint recordLimit = 10, UserGender gender = UserGender.Any,
                                             uint ageStart = 0, uint ageStop = 200, UserStatus status = UserStatus.Any,
                                             uint checkinsWithinDays = Int32.MaxValue, string appTag = "", object state = null)
        {
            FindUsersInternal (latitude, longitude, searchDistance, recordLimit, gender, ageStart,
                             ageStop, status, checkinsWithinDays, appTag, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        // delete this with the async call
        internal void FindUserInternal(double latitude, double longitude,
                                                               uint searchDistance, uint recordLimit, UserGender gender,
                                                               uint ageStart, uint ageStop, UserStatus status,
                                                               uint checkinsWithinMinutes, string appTag, Action<BuddyCallResult<List<User>>> callback)
        {
            FindUsersInternal(latitude, longitude, searchDistance, recordLimit, gender, ageStart, ageStop, status, checkinsWithinMinutes, appTag, callback);
        }
        

        internal void FindUsersInternal (double latitude, double longitude,
                                                               uint searchDistance, uint recordLimit, UserGender gender,
                                                               uint ageStart, uint ageStop, UserStatus status,
                                                               uint checkinsWithinMinutes, string appTag, Action<BuddyCallResult<List<User>>> callback)
        {
            if (latitude > 90.0 || latitude < -90.0)
                throw new ArgumentException ("Can't be bigger than 90.0 or smaller than -90.0.", "atLatitude");
            if (longitude > 180.0 || longitude < -180.0)
                throw new ArgumentException ("Can't be bigger than 180.0 or smaller than -180.0.", "atLongitude");

            Client.Service.UserAccount_Profile_Search (
                        Client.AppName, Client.AppPassword, this.Token, searchDistance.ToString (),
                        latitude.ToString (CultureInfo.InvariantCulture), longitude.ToString (CultureInfo.InvariantCulture), recordLimit.ToString (),
                        gender == UserGender.Any ? "" : Enum.GetName (typeof(UserGender), gender).ToLower (),
                        ageStart.ToString (), ageStop.ToString (), status == UserStatus.Any ? "" : ((int)status).ToString (),
                        checkinsWithinMinutes.ToString (), appTag, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<List<User>> (null, bcr.Error));
                    return;
                }
                List<User> users = new List<User> ();
                foreach (var u in result)
                    users.Add (new User (this.Client, u));
                {
                    callback (BuddyResultCreator.Create (users, bcr.Error));
                    return; }
                ;
            });
            return;

        }


        /// <summary>
        /// Add a profile photo for this user.
        /// </summary>
        /// <param name="blob">An array of bytes that represent the image you are adding.</param>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true if the profile photo was added, false otherwise.</param>
        /// <param name="appTag">An optional tag for the photo.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of AddProfilePhotoAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult AddProfilePhotoAsync (Action<bool, BuddyCallbackParams> callback, byte[] blob, string appTag = "", object state = null)
        {
            AddProfilePhotoInternal (new MemoryStream(blob), appTag, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void AddProfilePhotoInternal (Stream photoStream, string appTag, Action<BuddyCallResult<bool>> callback)
        {
            if (photoStream == null || photoStream.Length == 0)
                throw new ArgumentException ("Can't be null or empty.", "blob");
            if (appTag == null)
                throw new ArgumentNullException ("appTag");

            this.Client.Service.Pictures_ProfilePhoto_Add (this.Client.AppName, this.Client.AppPassword, this.Token, photoStream, appTag, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None && bcr.Error != BuddyError.ServiceErrorNegativeOne) {
                    callback (BuddyResultCreator.Create (default(bool), bcr.Error));
                    return;
                }
                callback (BuddyResultCreator.Create (result == "1", BuddyError.None));
            });
            return;

        }


        /// <summary>
        /// Check-in the user at a location.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true if the check-in was successful, false otherwise.</param>
        /// <param name="latitude">The latitude of the location.</param>
        /// <param name="longitude">The longitude of the location.</param>
        /// <param name="comment">An optional comment for the check-in.</param>
        /// <param name="appTag">An optional application specific tag for the location.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of CheckInAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult CheckInAsync (Action<bool, BuddyCallbackParams> callback, double latitude, double longitude, string comment = "", string appTag = "", object state = null)
        {
            CheckInInternal (latitude, longitude, comment, appTag, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void CheckInInternal (double latitude, double longitude, string comment, string appTag, Action<BuddyCallResult<bool>> callback)
        {
            this.Client.Service.UserAccount_Location_Checkin (this.Client.AppName, this.Client.AppPassword, this.Token,
                        latitude.ToString (CultureInfo.InvariantCulture), longitude.ToString (CultureInfo.InvariantCulture), comment, appTag, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None && bcr.Error != BuddyError.ServiceErrorNegativeOne) {
                    callback (BuddyResultCreator.Create (default(bool), bcr.Error));
                    return;
                }
                callback (BuddyResultCreator.Create (result == "1", BuddyError.None));
            });
            return;

        }


        /// <summary>
        /// Get a list of user check-in locations.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is a list of user checkins.</param>
        /// <param name="afterDate">Filter the list to return only check-in after a date.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetCheckInsAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetCheckInsAsync (Action<List<CheckInLocation>, BuddyCallbackParams> callback, DateTime afterDate = default(DateTime), object state = null)
        {
            GetCheckInsInternal (afterDate, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void GetCheckInsInternal (DateTime afterDate, Action<BuddyCallResult<List<CheckInLocation>>> callback)
        {
            this.Client.Service.UserAccount_Location_GetHistory (this.Client.AppName, this.Client.AppPassword, this.Token,
                        afterDate == DateTime.MinValue ? "1/1/1950" : afterDate.ToString (CultureInfo.InvariantCulture), (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<List<CheckInLocation>> (null, bcr.Error));
                    return;
                }
                List<CheckInLocation> locations = new List<CheckInLocation> ();
                foreach (var d in result)
                    locations.Add (new CheckInLocation (this.Client.TryParseDouble (d.Latitude), this.Client.TryParseDouble (d.Longitude),
                                 d.CreatedDate, d.PlaceName, null, null));
                {
                    callback (BuddyResultCreator.Create (locations, bcr.Error));
                    return; }
                ;
            });
            return;

        }


        /// <summary>
        /// Delete this user.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true if the user was deleted, false otherwise.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of DeleteAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult DeleteAsync (Action<bool, BuddyCallbackParams> callback, object state = null)
        {
            DeleteInternal ((bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void DeleteInternal (Action<BuddyCallResult<bool>> callback)
        {
            this.Client.Service.UserAccount_Profile_DeleteAccount (this.Client.AppName, this.Client.AppPassword, this.ID.ToString (), (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None && bcr.Error != BuddyError.ServiceErrorNegativeOne) {
                    callback (BuddyResultCreator.Create (default(bool), bcr.Error));
                    return;
                }
                callback (BuddyResultCreator.Create (result == "1", BuddyError.None));
            });
            return;

        }

      

        /// <summary>
        /// Update the profile of this user.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true if the update was successful, false otherwise.</param>
        /// <param name="name">Optional new name for the user, can't be null or empty.</param>
        /// <param name="password">Optional new password for the user, can't be null.</param>
        /// <param name="gender">Optional new gender for the user.</param>
        /// <param name="age">Optional new age for the user.</param>
        /// <param name="email">Optional new email for the user.</param>
        /// <param name="status">Optional new status for the user.</param>
        /// <param name="fuzzLocation">Optional change in location fuzzing for this user. If location fuzzing is enable, user location will be 
        /// randomized in all searches by other users.</param>
        /// <param name="celebrityMode">Optional change in celebrity mode for this user. If celebrity mode is enabled the user will be hidden from all searches in the system.</param>
        /// <param name="appTag">Optional update to the custom application tag for this user.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of UpdateAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult UpdateAsync (Action<bool, BuddyCallbackParams> callback, string name = "", string password = "", UserGender gender = UserGender.Any, int age = 0,
                            string email = "", UserStatus status = UserStatus.Any, bool fuzzLocation = false, bool celebrityMode = false, string appTag = "", object state = null)
        {
            UpdateInternal (name, password, gender, age, email, status, fuzzLocation, celebrityMode, appTag, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void UpdateInternal (string name, string password, UserGender gender, int age,
                    string email, UserStatus status, bool fuzzLocation, bool celebrityMode, string appTag, Action<BuddyCallResult<bool>> callback)
        {
            //if (String.IsNullOrEmpty(name)) throw new ArgumentException("Can't be null or empty.", "name");
            //if (password == null) throw new ArgumentNullException("password");

            this.Client.Service.UserAccount_Profile_Update (this.Client.AppName, this.Client.AppPassword, this.Token, name, password,
                            gender == UserGender.Any ? "" : gender.ToString().ToLowerInvariant(), age, email, (int)status, fuzzLocation ? 1 : 0,
                            celebrityMode ? 1 : 0, appTag, (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create (default(bool), bcr.Error));
                    return;
                }
                if (result == "1") {
                    if (!String.IsNullOrEmpty (name))
                        this.Name = name;
                    if (gender != UserGender.Any)
                        this.Gender = gender;
                    if (age != -1)
                        this.Age = age;
                    if (email != "")
                        this.Email = email;
                    if (status != UserStatus.Any)
                        this.Status = status;
                    this.LocationFuzzing = fuzzLocation;
                    this.CelebrityMode = celebrityMode;
                    if (appTag != "")
                        this.ApplicationTag = appTag;

                    {
                        callback (BuddyResultCreator.Create (true, BuddyError.None));
                        return; }
                    ;
                } else {
                    callback (BuddyResultCreator.Create (false, BuddyError.None));
                    return;
                }
                ;
            });
            return;

        }

       

        /// <summary>
        /// Retrieve a picture by its unique ID. Any picture that the user owns or is publicly available can be retrieved.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is the picture if found, null otherwise.</param>
        /// <param name="pictureId">The id of the picture to retrieve.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of GetPictureAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult GetPictureAsync (Action<Picture, BuddyCallbackParams> callback, int pictureId, object state = null)
        {
            GetPictureInternal (pictureId, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void GetPictureInternal (int pictureId, Action<BuddyCallResult<Picture>> callback)
        {
            if (pictureId < 0)
                throw new ArgumentException ("Can't be smaller than 0.", "pictureId");
            this.Client.Service.Pictures_Photo_Get (this.Client.AppName, this.Client.AppPassword, this.Token, this.ID.ToString (), pictureId.ToString (), (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<Picture> (null, bcr.Error));
                    return;
                }
                if (result.Length == 0) {
                    callback (BuddyResultCreator.Create<Picture> (null, bcr.Error));
                    return;
                }
                ;

                {
                    callback (BuddyResultCreator.Create (new Picture (this.Client, this, result [0]), bcr.Error));
                    return; }
                ;
            });
            return;

        }

       
        /// <summary>
        /// Search for public albums from other users.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is a list of public albums.</param>
        /// <param name="searchDistanceInMeters">Optionally search only within a certain distance from the supplied lat/long.</param>
        /// <param name="latitude">Optionally search for photos added near a latitude.</param>
        /// <param name="longitude">Optionally search for photos added near a longitude.</param>
        /// <param name="limitResults">Optionally limit the number of returned photos. Note that this parameter limits the photos returned, not albums. It's possible
        /// that a partial album is returned.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of SearchForAlbumsAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult SearchForAlbumsAsync (Action<List<PhotoAlbumPublic>, BuddyCallbackParams> callback, int searchDistanceInMeters = 99999999, double latitude = 0.0,
                            double longitude = 0.0, int limitResults = 50, object state = null)
        {
            SearchForAlbumsInternal (searchDistanceInMeters, latitude, longitude, limitResults, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void SearchForAlbumsInternal (int searchDistanceInMeters,
                    double latitude, double longitude, int limitResults, Action<BuddyCallResult<List<PhotoAlbumPublic>>> callback)
        {
            this.Client.Service.Pictures_SearchPhotos_Nearby (this.Client.AppName, this.Client.AppPassword, this.Token, searchDistanceInMeters.ToString (CultureInfo.InvariantCulture),
                    latitude.ToString (CultureInfo.InvariantCulture), longitude.ToString (CultureInfo.InvariantCulture), limitResults.ToString (), (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None) {
                    callback (BuddyResultCreator.Create<List<PhotoAlbumPublic>> (null, bcr.Error));
                    return;
                }
                Dictionary<string, PhotoAlbumPublic> dict = new Dictionary<string, PhotoAlbumPublic> ();

                foreach (var d in result) {
                    if (!dict.ContainsKey (d.PhotoAlbumName))
                        dict.Add (d.PhotoAlbumName, new PhotoAlbumPublic (this.Client, Int32.Parse (d.UserProfileID), d.PhotoAlbumName));
                    dict [d.PhotoAlbumName].pictures.Add (new PicturePublic (this.Client, null, d, Int32.Parse (d.UserProfileID)));
                }
                ;
                {
                    callback (BuddyResultCreator.Create (dict.Values.ToList (), bcr.Error));
                    return; }
                ;
            });
            return;

        }


        /// <summary>
        /// Delete a profile photo for this user. You can use the GetProfilePhotosAsync method to retrieve all the profile photos.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true on success, false otherwise.</param>
        /// <param name="picture">The photo to delete.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of DeleteProfilePhotoAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult DeleteProfilePhotoAsync (Action<bool, BuddyCallbackParams> callback, PicturePublic picture, object state = null)
        {
            DeleteProfilePhotoInternal (picture, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void DeleteProfilePhotoInternal (PicturePublic picture, Action<BuddyCallResult<bool>> callback)
        {
            if (picture == null)
                throw new ArgumentNullException ("picture");

            this.Client.Service.Pictures_ProfilePhoto_Delete (this.Client.AppName, this.Client.AppPassword, this.Token, picture.PhotoID.ToString (), (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None && bcr.Error != BuddyError.ServiceErrorNegativeOne) {
                    callback (BuddyResultCreator.Create (default(bool), bcr.Error));
                    return;
                }
                callback (BuddyResultCreator.Create (result == "1", BuddyError.None));
            });
            return;

        }

      

        /// <summary>
        /// Set a new "active" profile photo from the list of profile photos that the user has uploaded. The photo needs to be already uploaded.
        /// </summary>
        /// <param name="callback">The async callback to call on success or error. The first parameter is true on success, false otherwise.</param>
        /// <param name="picture">The photo to set as the "active" profile photo.</param>
        /// <param name="state">An optional user defined object that will be passed to the callback.</param>
        /// <returns>An IAsyncResult handle that can be used to monitor progress on this call.</returns>
        #if AWAIT_SUPPORTED
	[Obsolete("This method has been deprecated, please call one of the other overloads of SetProfilePhotoAsync.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public IAsyncResult SetProfilePhotoAsync (Action<bool, BuddyCallbackParams> callback, PicturePublic picture, object state = null)
        {
            SetProfilePhotoInternal (picture, (bcr) => {
                if (callback == null)
                    return;
                callback (bcr.Result, new BuddyCallbackParams (bcr.Error)); });
            return null;
        }

        internal void SetProfilePhotoInternal (PicturePublic picture, Action<BuddyCallResult<bool>> callback)
        {
            if (picture == null)
                throw new ArgumentNullException ("picture");

            this.Client.Service.Pictures_ProfilePhoto_Set (this.Client.AppName, this.Client.AppPassword, this.Token, picture.PhotoID.ToString (), (bcr) =>
            {
                var result = bcr.Result;
                if (bcr.Error != BuddyError.None && bcr.Error != BuddyError.ServiceErrorNegativeOne) {
                    callback (BuddyResultCreator.Create (default(bool), bcr.Error));
                    return;
                }
                callback (BuddyResultCreator.Create (result == "1", BuddyError.None));
            });
            return;

        }

#if AWAIT_SUPPORTED

        // <summary>
        //            Find the public profiles of all users that match the serch parameters.
        //            </summary>
        /// <param name="latitude">The latitude of the position to search from. Must be a value between -90.0 and 90.0.</param>
        /// <param name="longitude">The Longitude of the position to search from. Must be a value between -180.0 and 180.0.</param>
        /// <param name="searchDistance">The distance in meters from the specified latitude/longitude to search for results. To ignore this distance pass in 40075000 (the circumferance of the earth).</param>
        /// <param name="recordLimit">The maximum number of users to return with this search.</param>
        /// <param name="gender">The gender of the users, use UserGender.Any to search for both.</param>
        /// <param name="ageStart">Specifies the starting age for the range of ages to search in. The value must be &gt;= 0.</param>
        /// <param name="ageStop">Specifies the ending age for the range of ages to search in. The value must be &gt; ageStart.</param>
        /// <param name="status">The status of the users to search for. Use UserStatus.Any to ignore this parameter.</param>
        /// <param name="checkinsWithinMinutes">Filter for users who have checked-in in the past 'checkinsWithinMinutes' number of minutes.</param>
        /// <param name="appTag">Search for the custom appTag that was stored with the user.</param><exception cref="T:System.ArgumentException">When latitude or longitude are incorrect.</exception>
        /// <returns>A Task&lt;IEnumerable&lt;User&gt;&gt; that can be used to monitor progress on this call.</returns>
        public System.Threading.Tasks.Task<IEnumerable<User>> FindUsersAsync( double latitude = 0, double longitude = 0, uint searchDistance = 2147483647, uint recordLimit = 10, Buddy.UserGender gender = UserGender.Any, uint ageStart = 0, uint ageStop = 200, Buddy.UserStatus status = UserStatus.Any, uint checkinsWithinMinutes = 2147483647, string appTag = "")
        {
            return this.FindUserAsync(latitude, longitude, searchDistance, recordLimit, gender, ageStart, ageStop, status, checkinsWithinMinutes, appTag);
        }
        

        public  System.Threading.Tasks.Task<Buddy.User> FindUser(string userNameToFetch)
        {
                  var tcs = new System.Threading.Tasks.TaskCompletionSource<User>();
                  this.FindUserInternal(userNameToFetch,  (bcr) =>
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


       [Obsolete("Please use the overload of AddProfilePhoto that takes a Stream instead of a byte[]")]
       public System.Threading.Tasks.Task<bool> AddProfilePhoto(byte[] blob, string appTag = "") {
        return this.AddProfilePhotoAsync(new MemoryStream(blob), appTag);
        }

       /// <summary>
       /// Delete this user.
       /// </summary>
       /// <returns>A Task&lt;Boolean&gt;that can be used to monitor progress on this call.</returns>
       public System.Threading.Tasks.Task<Boolean> DeleteAsync()
       {
           var tcs = new System.Threading.Tasks.TaskCompletionSource<Boolean>();
           DeleteInternal((bcr) =>
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

       /// <summary>
       /// Update the profile of this user.
       /// </summary>
       /// <param name="name">Optional new name for the user, can't be null or empty.</param>
       /// <param name="password">Optional new password for the user, can't be null.</param>
       /// <param name="gender">Optional new gender for the user.</param>
       /// <param name="age">Optional new age for the user.</param>
       /// <param name="email">Optional new email for the user.</param>
       /// <param name="status">Optional new status for the user.</param>
       /// <param name="fuzzLocation">Optional change in location fuzzing for this user. If location fuzzing is enable, user location will be 
       /// randomized in all searches by other users.</param>
       /// <param name="celebrityMode">Optional change in celebrity mode for this user. If celebrity mode is enabled the user will be hidden from all searches in the system.</param>
       /// <param name="appTag">Optional update to the custom application tag for this user.</param>
       /// <returns>A Task&lt;Boolean&gt;that can be used to monitor progress on this call.</returns>
       public System.Threading.Tasks.Task<Boolean> UpdateAsync(string name = "", string password = "", Buddy.UserGender gender = UserGender.Any, int age = 0, string email = "", Buddy.UserStatus status = UserStatus.Any, bool fuzzLocation = false, bool celebrityMode = false, string appTag = "")
       {
           var tcs = new System.Threading.Tasks.TaskCompletionSource<Boolean>();
           UpdateInternal(name, password, gender, age, email, status, fuzzLocation, celebrityMode, appTag, (bcr) =>
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

       /// <summary>
       /// Retrieve a picture by its unique ID. Any picture that the user owns or is publicly available can be retrieved.
       /// </summary>
       /// <param name="pictureId">The id of the picture to retrieve.</param>
       /// <returns>A Task&lt;Picture&gt;that can be used to monitor progress on this call.</returns>
       public System.Threading.Tasks.Task<Picture> GetPictureAsync(int pictureId)
       {
           var tcs = new System.Threading.Tasks.TaskCompletionSource<Picture>();
           GetPictureInternal(pictureId, (bcr) =>
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

       /// <summary>
       /// Search for public albums from other users.
       /// </summary>
       /// <param name="searchDistanceInMeters">Optionally search only within a certain distance from the supplied lat/long.</param>
       /// <param name="latitude">Optionally search for photos added near a latitude.</param>
       /// <param name="longitude">Optionally search for photos added near a longitude.</param>
       /// <param name="limitResults">Optionally limit the number of returned photos. Note that this parameter limits the photos returned, not albums. It's possible
       /// that a partial album is returned.</param>
       /// <returns>A Task&lt;IEnumerable&lt;PhotoAlbumPublic&gt; &gt;that can be used to monitor progress on this call.</returns>
       public System.Threading.Tasks.Task<IEnumerable<PhotoAlbumPublic>> SearchForAlbumsAsync(int searchDistanceInMeters = 99999999, double latitude = 0, double longitude = 0, int limitResults = 50)
       {
           var tcs = new System.Threading.Tasks.TaskCompletionSource<IEnumerable<PhotoAlbumPublic>>();
           SearchForAlbumsInternal(searchDistanceInMeters, latitude, longitude, limitResults, (bcr) =>
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

       /// <summary>
       /// Delete a profile photo for this user. You can use the GetProfilePhotosAsync method to retrieve all the profile photos.
       /// </summary>
       /// <param name="picture">The photo to delete.</param>
       /// <returns>A Task&lt;Boolean&gt;that can be used to monitor progress on this call.</returns>
       public System.Threading.Tasks.Task<Boolean> DeleteProfilePhotoAsync(Buddy.PicturePublic picture)
       {
           var tcs = new System.Threading.Tasks.TaskCompletionSource<Boolean>();
           DeleteProfilePhotoInternal(picture, (bcr) =>
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

       /// <summary>
       /// Set a new "active" profile photo from the list of profile photos that the user has uploaded. The photo needs to be already uploaded.
       /// </summary>
       /// <param name="picture">The photo to set as the "active" profile photo.</param>
       /// <returns>A Task&lt;Boolean&gt;that can be used to monitor progress on this call.</returns>
       public System.Threading.Tasks.Task<Boolean> SetProfilePhotoAsync(Buddy.PicturePublic picture)
       {
           var tcs = new System.Threading.Tasks.TaskCompletionSource<Boolean>();
           SetProfilePhotoInternal(picture, (bcr) =>
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

       /// <summary>
       /// Find the public profile of a user from their unique User ID. This method can be used to find any user associated with this Application.
       /// </summary>
       /// <param name="id">The ID of the user, must be bigger than 0.</param><exception cref="T:Buddy.BuddyServiceException">With value: InvalidUserId, when the user ID doesn't exist in the system.</exception>
       /// <returns>A Task&lt;User&gt;that can be used to monitor progress on this call.</returns>
       public System.Threading.Tasks.Task<User> FindUserAsync(int id)
       {
           var tcs = new System.Threading.Tasks.TaskCompletionSource<User>();
           FindUserInternal(id, (bcr) =>
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

       /// <summary>
       /// Find the public profiles of all users that match the serch parameters.
       /// </summary>
       /// <param name="latitude">The latitude of the position to search from. Must be a value between -90.0 and 90.0.</param>
       /// <param name="longitude">The Longitude of the position to search from. Must be a value between -180.0 and 180.0.</param>
       /// <param name="searchDistance">The distance in meters from the specified latitude/longitude to search for results. To ignore this distance pass in 40075000 (the circumferance of the earth).</param>
       /// <param name="recordLimit">The maximum number of users to return with this search.</param>
       /// <param name="gender">The gender of the users, use UserGender.Any to search for both.</param>
       /// <param name="ageStart">Specifies the starting age for the range of ages to search in. The value must be &gt;= 0.</param>
       /// <param name="ageStop">Specifies the ending age for the range of ages to search in. The value must be &gt; ageStart.</param>
       /// <param name="status">The status of the users to search for. Use UserStatus.Any to ignore this parameter.</param>
       /// <param name="checkinsWithinMinutes">Filter for users who have checked-in in the past 'checkinsWithinMinutes' number of minutes.</param>
       /// <param name="appTag">Search for the custom appTag that was stored with the user.</param><exception cref="T:System.ArgumentException">When latitude or longitude are incorrect.</exception>
       /// <returns>A Task&lt;IEnumerable&lt;User&gt; &gt;that can be used to monitor progress on this call.</returns>
       [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
       public System.Threading.Tasks.Task<IEnumerable<User>> FindUserAsync(double latitude = 0, double longitude = 0, uint searchDistance = 2147483647, uint recordLimit = 10, Buddy.UserGender gender = UserGender.Any, uint ageStart = 0, uint ageStop = 200, Buddy.UserStatus status = UserStatus.Any, uint checkinsWithinMinutes = 2147483647, string appTag = "")
       {
           var tcs = new System.Threading.Tasks.TaskCompletionSource<IEnumerable<User>>();
           FindUserInternal(latitude, longitude, searchDistance, recordLimit, gender, ageStart, ageStop, status, checkinsWithinMinutes, appTag, (bcr) =>
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

       /// <summary>
       /// Add a profile photo for this user.
       /// </summary>
       /// <param name="photoSteam">An array of bytes that represent the image you are adding.</param>
       /// <param name="appTag">An optional tag for the photo.</param>
       /// <returns>A Task&lt;Boolean&gt;that can be used to monitor progress on this call.</returns>
       public System.Threading.Tasks.Task<Boolean> AddProfilePhotoAsync(Stream photoSteam, string appTag = "")
       {
           var tcs = new System.Threading.Tasks.TaskCompletionSource<Boolean>();
           AddProfilePhotoInternal(photoSteam, appTag, (bcr) =>
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

       /// <summary>
       /// Check-in the user at a location.
       /// </summary>
       /// <param name="latitude">The latitude of the location.</param>
       /// <param name="longitude">The longitude of the location.</param>
       /// <param name="comment">An optional comment for the check-in.</param>
       /// <param name="appTag">An optional application specific tag for the location.</param>
       /// <returns>A Task&lt;Boolean&gt;that can be used to monitor progress on this call.</returns>
       public System.Threading.Tasks.Task<Boolean> CheckInAsync(double latitude, double longitude, string comment = "", string appTag = "")
       {
           var tcs = new System.Threading.Tasks.TaskCompletionSource<Boolean>();
           CheckInInternal(latitude, longitude, comment, appTag, (bcr) =>
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

       /// <summary>
       /// Get a list of user check-in locations.
       /// </summary>
       /// <param name="afterDate">Filter the list to return only check-in after a date.</param>
       /// <returns>A Task&lt;IEnumerable&lt;CheckInLocation&gt; &gt;that can be used to monitor progress on this call.</returns>
       public System.Threading.Tasks.Task<IEnumerable<CheckInLocation>> GetCheckInsAsync(System.DateTime afterDate = default(DateTime))
       {
           var tcs = new System.Threading.Tasks.TaskCompletionSource<IEnumerable<CheckInLocation>>();
           GetCheckInsInternal(afterDate, (bcr) =>
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
