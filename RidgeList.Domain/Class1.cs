using System;
using System.Collections.Generic;
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
        
        public async Task Save(Wishlist wishlist)
        {
            this._wishlists[wishlist.Id] = wishlist;
        }

        public Task<Wishlist> Load(Guid id)
        {
            return Task.FromResult(this._wishlists[id]);
        }
    }

    public interface IWishlistRepository
    {
        Task Save(Wishlist wishlist);

        Task<Wishlist> Load(Guid id);
    }
}