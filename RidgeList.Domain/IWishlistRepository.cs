using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RidgeList.Domain
{
    
    public class InMemoryWishlistRepository : IWishlistRepository
    {
        private Dictionary<Guid, Wishlist> _wishlists;

        public InMemoryWishlistRepository()
        {
            this._wishlists = new Dictionary<Guid, Wishlist>();
        }
        
        public Task Save(Wishlist wishlist)
        {
            this._wishlists[wishlist.Id] = wishlist;
            return Task.CompletedTask;
        }

        public Task<Wishlist> Load(Guid id)
        {
            return Task.FromResult(this._wishlists[id]);
        }

        public Task<IEnumerable<WishlistSummary>> GetWishlistSummaries()
        {
            var summaries = this._wishlists.Select(s => new WishlistSummary()
            {
                Name = s.Value.Name
            });
            return Task.FromResult(summaries);
        }
    }

    public interface IWishlistRepository
    {
        Task Save(Wishlist wishlist);

        Task<Wishlist> Load(Guid id);

        Task<IEnumerable<WishlistSummary>> GetWishlistSummaries();

        // Task<WishlistSummary> GetW
    }
}