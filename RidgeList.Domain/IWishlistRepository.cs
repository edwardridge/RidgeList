using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RidgeList.Domain
{
    public interface IUserRepository
    {
        Task<User> GetUser(Guid personId);

        Task<IList<User>> GetUsers(params Guid[] personIds);

        Task AddWishlistToPerson(Guid personId, Guid wishlistId);

        Task CreatePerson(Guid personId, string email, string name);

        Task RemoveWishlistFromPerson(Guid personId, Guid wishlistId);

        Task SetEmailAndName(Guid personId, string email, string name);

        Task<User> GetUserFromEmail(string email);
    }

    public class InMemoryWishlistSummaryRepository : IUserRepository
    {
        public Dictionary<Guid, User> _wishlistsSummaries = new Dictionary<Guid, User>();
        
        public Task AddWishlistToPerson(Guid personId, Guid wishlistId)
        {
            if (_wishlistsSummaries.ContainsKey(personId))
            {
                _wishlistsSummaries[personId].Wishlists.Add(wishlistId);
                return Task.CompletedTask;
            }

            _wishlistsSummaries[personId] = new User() { Id = personId, Wishlists = {  wishlistId } };
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

        public Task<User> GetUser(Guid personId)
        {
            return Task.FromResult(_wishlistsSummaries[personId]);
        }

        public Task<IList<User>> GetUsers(params Guid[] personIds)
        {
            return Task.FromResult(personIds.Select(GetUser).ToList() as IList<User>);
        }

        public Task<User> GetUserFromEmail(string email)
        {
            var userWishlist = this._wishlistsSummaries.SingleOrDefault(s => s.Value.Email == email);
            return Task.FromResult(userWishlist.Value);
        }

        public Task CreatePerson(Guid personId, string email, string name)
        {
            this._wishlistsSummaries.Add(personId, new User() { Id = personId, Email = email, Name = name });
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