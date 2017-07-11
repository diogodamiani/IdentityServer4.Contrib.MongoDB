using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServer4.MongoDB.Users
{
    public class UserExtended : Test.TestUser
    {
        public ObjectId _id { get; set; }
    }
}
