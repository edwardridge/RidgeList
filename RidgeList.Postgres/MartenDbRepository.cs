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
        
        public async Task<IEnumerable<Guid>> GetWishlistSummaries(string emailAddress)
        {
            using (var session = documentStore.OpenSession())
            {
                return (await session.LoadAsync<UserWishlists>(emailAddress)).Wishlists;
            }
        }

        public async Task AddWishlistToPerson(string email, Guid wishlistId)
        {
            using (var session = documentStore.OpenSession())
            {
                var userWishlists = await session.LoadAsync<UserWishlists>(email);
                if (userWishlists == null)
                {
                    userWishlists = new UserWishlists()
                    {
                        Email = email,
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

        public async Task RemoveWishlistFromPerson(string email, Guid wishlistId)
        {
            using (var session = documentStore.OpenSession())
            {
                var userWishlists = await session.LoadAsync<UserWishlists>(email);
                userWishlists.Wishlists.Remove(wishlistId);
                session.Store(userWishlists);
                session.SaveChanges();
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