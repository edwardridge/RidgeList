using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using RidgeList.Domain;

namespace RidgeList.Postgres
{

    public class MartenDbSummaryRepository : IUserRepository
    {
        private readonly IDocumentStore documentStore;

        public MartenDbSummaryRepository(IDocumentStore documentStore)
        {
            this.documentStore = documentStore;
        }

        public async Task AddWishlistToPerson(Guid personId, Guid wishlistId)
        {
            using (var session = documentStore.OpenSession())
            {
                var userWishlists = await session.LoadAsync<User>(personId);
                if (userWishlists == null)
                {
                    userWishlists = new User()
                    {
                        Id = personId,
                        Wishlists = {wishlistId}
                    };
                }
                else
                {
                    userWishlists.Wishlists.Add(wishlistId);
                }
                
                session.Store(userWishlists);
                session.SaveChanges();
            }
        }

        public async Task RemoveWishlistFromPerson(Guid personId, Guid wishlistId)
        {
            using (var session = documentStore.OpenSession())
            {
                var userWishlists = await session.LoadAsync<User>(personId);
                userWishlists.Wishlists.Remove(wishlistId);
                session.Store(userWishlists);
                session.SaveChanges();
            }
        }

        public async Task<User> GetUser(Guid userId)
        {
            using (var session = documentStore.OpenSession())
            {
                return await session.LoadAsync<User>(userId);
            }
        }

        public async Task<IList<User>> GetUsers(params Guid[] userIds)
        {
            using (var session = documentStore.OpenSession())
            {
                return (await session.LoadManyAsync<User>(userIds)).ToList();
            }
        }

        public async Task SetEmailAndName(Guid personId, string email, string name)
        {
            using (var session = documentStore.OpenSession())
            {
                var userWishlists = await session.LoadAsync<User>(personId);
                userWishlists.Email = email;
                userWishlists.Name = name;
                session.Store(userWishlists);
                session.SaveChanges();
            }
        }

        public Task<User> GetUserFromEmail(string email)
        {
            using (var session = documentStore.OpenSession())
            {
                var userWishlists = session.Query<User>().SingleOrDefault(s => s.Email.ToLower() == email.ToLower());
                return Task.FromResult(userWishlists);
            }
        }

        public async Task CreatePerson(Guid personId, string email, string name)
        {
            using (var session = documentStore.OpenSession())
            {
                var userWishlists = session.Load<User>(personId);
                if(userWishlists == null)
                {
                    session.Store(new User()
                    {
                        Id = personId,
                        Email = email,
                        Name = name
                    });
                }
                await session.SaveChangesAsync();
            }
        }
    }
    
    public class DbSettings
    {
        public string DbUsername { get; set; }

        public string DbPassword { get; set; }

        public string DbHost { get; set; }

        public string DbDatabase { get; set; }
    }

    public class MartenDbRepository : IWishlistRepository
    {
        private IDocumentStore documentStore;

        public static string BuildConnectionString(DbSettings dbSettingsFromSecrets, bool useSsl)
        {
            var useSslString = useSsl ? ";sslmode=Require;Trust Server Certificate=true;" : "";
            var buildConnectionString = @$"Username={dbSettingsFromSecrets.DbUsername};Password={dbSettingsFromSecrets.DbPassword};Host={dbSettingsFromSecrets.DbHost};Port=5432;Database={dbSettingsFromSecrets.DbDatabase}{useSslString}";
            return buildConnectionString;
        }

        public MartenDbRepository(IDocumentStore documentStore)
        {
            this.documentStore = documentStore;
        }
        
        public async Task Save(Wishlist wishlist)
        {
            using (var session = documentStore.OpenSession())
            {
                session.Store(wishlist);
                await session.SaveChangesAsync();
            }
        }

        public async Task<Wishlist> Load(Guid id)
        {
            using (var session = documentStore.OpenSession())
            {
                return await session.LoadAsync<Wishlist>(id);
            }
        }

        public async Task Delete(Guid id)
        {
            using (var session = documentStore.OpenSession())
            {
                var wishlist = session.Load<Wishlist>(id);
                session.Delete<Wishlist>(id);
                await session.SaveChangesAsync();
            }
        }
    }
}