using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using RidgeList.Domain;

namespace RidgeList.Postgres
{


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

                foreach (var person in wishlist.People)
                {
                    var f = session.Load<UserWishlists>(person.Email) ?? new UserWishlists() { Email = person.Email };
                    
                    f.Wishlists.Add(wishlist.Id);
                    f.Wishlists = f.Wishlists.Distinct().ToList();
                    session.Store(f);
                }
                
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

        public async Task<IEnumerable<WishlistSummary>> GetWishlistSummaries(string emailAddress)
        {
            using (var session = documentStore.OpenSession())
            {
                var userWishlists = session.Load<UserWishlists>(emailAddress);

                if (userWishlists == null)
                {
                    return new List<WishlistSummary>();
                }
                
                var wishlists = userWishlists.Wishlists.Select(s => session.Load<Wishlist>(s)).ToList();
                return wishlists.Select(s => new WishlistSummary()
                {
                    Id = s.Id,
                    Name = s.Name
                });
            }
        }

        public async Task Delete(Guid id)
        {
            using (var session = documentStore.OpenSession())
            {
                var wishlist = session.Load<Wishlist>(id);
                
                foreach (var person in wishlist.People)
                {
                    var f = session.Load<UserWishlists>(person.Email);
                    if (f != null)
                    {
                        f.Wishlists.Remove(wishlist.Id);
                        session.Store(f);
                    }
                }
                
                session.Delete<Wishlist>(id);
                await session.SaveChangesAsync();
            }
        }
    }
}