using System;

namespace Buddy
{
   
    public abstract class BuddyBase
    {
        protected BuddyClient Client { get; private set; }
        protected AuthenticatedUser AuthUser { get; private set; }

        protected virtual bool AuthUserRequired
        {
            get
            {
                return false;
            }
        }

        protected BuddyBase(BuddyClient client)
        {
            if (client == null) throw new ArgumentNullException("client");

            this.Client = client;
        }

        protected BuddyBase(BuddyClient client, AuthenticatedUser user):this(client)
        {
            if (user == null && AuthUserRequired) throw new ArgumentNullException("user");

            this.AuthUser = user;
            
        }
    }
}
