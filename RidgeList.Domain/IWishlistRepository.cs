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

        public Task<IEnumerable<WishlistSummary>> GetWishlistSummaries(string emailAddress)
        {
            var summaries = this._wishlists
                .Where(s => s.Value.GetPeople().Contains(emailAddress))
                .Select(s => new WishlistSummary()
            {
                Id = s.Value.Id,
                Name = s.Value.Name
            });
            return Task.FromResult(summaries);
        }

        public Task Delete(Guid id)
        {
            this._wishlists.Remove(id);
            return Task.CompletedTask;
        }
    }

    public interface IWishlistRepository
    {
        Task Save(Wishlist wishlist);

        Task<Wishlist> Load(Guid id);

        Task<IEnumerable<WishlistSummary>> GetWishlistSummaries(string emailAddress);

        Task Delete(Guid id);

        // Task<WishlistSummary> GetW
    }
}