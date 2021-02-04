using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using RidgeList.Domain;

namespace RidgeList.Postgres
{

    public class MartenDbSummaryRepository : IWishlistSummaryRepository
    {
        private readonly IDocumentStore documentStore;

        public MartenDbSummaryRepository(IDocumentStore documentStore)
        {
            this.documentStore = documentStore;
        }
        
        public async Task<IEnumerable<Guid>> GetWishlistSummaries(Guid personId)
        {
            using (var session = documentStore.OpenSession())
            {
                var userWishlists = await session.LoadAsync<UserWishlists>(personId);
                return userWishlists?.Wishlists ?? new Guid[0].ToList();
            }
        }

        public async Task AddWishlistToPerson(Guid personId, Guid wishlistId)
        {
            using (var session = documentStore.OpenSession())
            {
                var userWishlists = await session.LoadAsync<UserWishlists>(personId);
                if (userWishlists == null)
                {
                    userWishlists = new UserWishlists()
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
                var userWishlists = await session.LoadAsync<UserWishlists>(personId);
                userWishlists.Wishlists.Remove(wishlistId);
                session.Store(userWishlists);
                session.SaveChanges();
            }
        }

        public async Task<UserWishlists> GetDetails(Guid personId)
        {
            using (var session = documentStore.OpenSession())
            {
                return await session.LoadAsync<UserWishlists>(personId);
            }
        }

        public async Task SetEmailAndName(Guid personId, string email, string name)
        {
            using (var session = documentStore.OpenSession())
            {
                var userWishlists = await session.LoadAsync<UserWishlists>(personId);
                userWishlists.Email = email;
                userWishlists.Name = name;
                session.Store(userWishlists);
                session.SaveChanges();
            }
        }

        public Task<UserWishlists> GetUserFromEmail(string email)
        {
            using (var session = documentStore.OpenSession())
            {
                var userWishlists = session.Query<UserWishlists>().SingleOrDefault(s => s.Email.ToLower() == email.ToLower());
                return Task.FromResult(userWishlists);
            }
        }

        public async Task CreatePerson(Guid personId, string email)
        {
            using (var session = documentStore.OpenSession())
            {
                var userWishlists = session.Load<UserWishlists>(personId);
                if(userWishlists == null)
                {
                    session.Store(new UserWishlists()
                    {
                        Id = personId,
                        Email = email
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