using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RidgeList.Domain
{
    public interface IWishlistSummaryRepository
    {
        //Todo: combine
        Task<IEnumerable<Guid>> GetWishlistSummaries(Guid personId);

        Task<UserWishlists> GetDetails(Guid personId);

        Task AddWishlistToPerson(Guid personId, Guid wishlistId);

        Task CreatePerson(Guid personId, string email, string name);

        Task RemoveWishlistFromPerson(Guid personId, Guid wishlistId);

        Task SetEmailAndName(Guid personId, string email, string name);

        Task<UserWishlists> GetUserFromEmail(string email);
    }

    public class InMemoryWishlistSummaryRepository : IWishlistSummaryRepository
    {
        public Dictionary<Guid, UserWishlists> _wishlistsSummaries = new Dictionary<Guid, UserWishlists>();
        
        public Task<IEnumerable<Guid>> GetWishlistSummaries(Guid personId)
        {
            return Task.FromResult(_wishlistsSummaries[personId] as IEnumerable<Guid>);
        }

        public Task AddWishlistToPerson(Guid personId, Guid wishlistId)
        {
            if (_wishlistsSummaries.ContainsKey(personId))
            {
                _wishlistsSummaries[personId].Wishlists.Add(wishlistId);
                return Task.CompletedTask;
            }

            _wishlistsSummaries[personId] = new UserWishlists() { Id = personId, Wishlists = {  wishlistId } };
            return Task.CompletedTask;
        }

        public Task RemoveWishlistFromPerson(Guid personId, Guid wishlistId)
        {
            _wishlistsSummaries[personId].Wishlists.Remove(wishlistId);
            return Task.CompletedTask;
        }

        public Task SetEmailAndName(Guid personId, string email, string name)
        {
            _wishlistsSummaries[personId].Email = email;
            _wishlistsSummaries[personId].Name = name;
            return Task.CompletedTask;
        }

        public Task<UserWishlists> GetDetails(Guid personId)
        {
            return Task.FromResult(_wishlistsSummaries[personId]);
        }

        public Task<UserWishlists> GetUserFromEmail(string email)
        {
            var userWishlist = this._wishlistsSummaries.Single(s => s.Value.Email == email);
            return Task.FromResult(userWishlist.Value);
        }

        public Task CreatePerson(Guid personId, string email, string name)
        {
            this._wishlistsSummaries.Add(personId, new UserWishlists() { Id = personId, Email = email, Name = name });
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