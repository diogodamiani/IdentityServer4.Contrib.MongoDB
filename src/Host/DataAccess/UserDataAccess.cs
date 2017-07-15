using IdentityModel;
using IdentityServer4.MongoDB.Users;
using IdentityServer4.Quickstart.UI;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Host.DataAccess
{
    public class UserDataAccess
    {
        IMongoClient _mongoClient;
        readonly string dataBaseName = "IS4";
        readonly string collectionName = "Users";

        public UserDataAccess(IMongoClient mongoClient)
        {
            _mongoClient = mongoClient;
        }

        public UserExtended GetUser(LoginInputModel model)
        {
            IMongoDatabase database = _mongoClient.GetDatabase(dataBaseName);
            IMongoCollection<UserExtended> userCollection = database.GetCollection<UserExtended>(collectionName);

            var filter = Builders<UserExtended>.Filter.Where(e => e.Password == model.Password && e.Username == model.Username);

            var findFluent = userCollection.Find(filter);

            return findFluent.FirstOrDefault();
        }

        public UserExtended GetUserByProvider(string provider, string id)
        {
            IMongoDatabase database = _mongoClient.GetDatabase(dataBaseName);
            IMongoCollection<UserExtended> userCollection = database.GetCollection<UserExtended>(collectionName);

            var filter = Builders<UserExtended>.Filter.Where(e => e.ProviderName == provider && e.ProviderSubjectId == id);

            var findFluent = userCollection.Find(filter);

            return findFluent.FirstOrDefault();
        }

        public UserExtended AutoProvisionUser(string provider, string userId, List<Claim> claims)
        {
            List<Claim> claimList1 = new List<Claim>();
            using (List<Claim>.Enumerator enumerator = claims.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Claim current = enumerator.Current;
                    if (current.GetType().ToString() == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")
                        claimList1.Add(new Claim("name", current.Value));
                    else if (((IDictionary<string, string>)JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap).ContainsKey(current.GetType().ToString()))
                        claimList1.Add(new Claim(((IDictionary<string, string>)JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap)[current.GetType().ToString()], current.Value));
                    else
                        claimList1.Add(current);
                }
            }
            List<Claim> claimList2 = claimList1;
            Func<Claim, bool> predicate = (Func<Claim, bool>)(x => x.GetType().ToString() == "name");

            if (!((IEnumerable<Claim>)claimList2).Any<Claim>(predicate))
            {
                Claim claim1 = ((IEnumerable<Claim>)claimList1).FirstOrDefault<Claim>((Func<Claim, bool>)(x => x.GetType().ToString() == "given_name"));
                string str1 = claim1 != null ? claim1.Value : (string)null;
                Claim claim2 = ((IEnumerable<Claim>)claimList1).FirstOrDefault<Claim>((Func<Claim, bool>)(x => x.GetType().ToString() == "family_name"));
                string str2 = claim2 != null ? claim2.Value : (string)null;
                if (str1 != null && str2 != null)
                    claimList1.Add(new Claim("name", str1 + " " + str2));
                else if (str1 != null)
                    claimList1.Add(new Claim("name", str1));
                else if (str2 != null)
                    claimList1.Add(new Claim("name", str2));
            }
            string uniqueId = CryptoRandom.CreateUniqueId(32);
            Claim claim = ((IEnumerable<Claim>)claimList1).FirstOrDefault<Claim>((Func<Claim, bool>)(c => c.Value == "name"));
            string str = (claim != null ? claim.Value : (string)null) ?? uniqueId;
            UserExtended user = new UserExtended()
            {
                SubjectId = uniqueId,
                Username = str,
                ProviderName = provider,
                ProviderSubjectId = userId,
                Claims = (ICollection<Claim>)claimList1
            };

            IMongoDatabase database = _mongoClient.GetDatabase(dataBaseName);
            IMongoCollection<UserExtended> userCollection = database.GetCollection<UserExtended>(collectionName);

            userCollection.InsertOne(user);

            return user;
        }
    }
}
