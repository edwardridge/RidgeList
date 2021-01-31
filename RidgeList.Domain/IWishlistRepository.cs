using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RidgeList.Domain
{
    public interface IWishlistSummaryRepository
    {
        Task<IEnumerable<Guid>> GetWishlistSummaries(string emailAddress);

        Task AddWishlistToPerson(string email, Guid wishlistId);
        
        Task RemoveWishlistFromPerson(string email, Guid wishlistId);
    }

    public class InMemoryWishlistSummaryRepository : IWishlistSummaryRepository
    {
        public Dictionary<string, List<Guid>> _wishlistsSummaries = new Dictionary<string, List<Guid>>();
        
        public Task<IEnumerable<Guid>> GetWishlistSummaries(string emailAddress)
        {
            return Task.FromResult(_wishlistsSummaries[emailAddress] as IEnumerable<Guid>);
        }

        public Task AddWishlistToPerson(string email, Guid wishlistId)
        {
            if (_wishlistsSummaries.ContainsKey(email))
            {
                _wishlistsSummaries[email].Add(wishlistId);
                return Task.CompletedTask;
            }

            _wishlistsSummaries[email] = new List<Guid>() {wishlistId};
            return Task.CompletedTask;
        }

        public Task RemoveWishlistFromPerson(string email, Guid wishlistId)
        {
            _wishlistsSummaries[email].Remove(wishlistId);
            return Task.CompletedTask;
        }
    }
    
    public class InMemoryWishlistRepository : IWishlistRepository
    {
        public Dictionary<Guid, Wishlist> _wishlists;

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

        Task Delete(Guid id);
    }
}