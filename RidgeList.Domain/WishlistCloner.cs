using System;
using System.Threading.Tasks;
using Force.DeepCloner;

namespace RidgeList.Domain
{
    public class WishlistCloner
    {
        private readonly IWishlistRepository _wishlistRepository;

        public WishlistCloner(IWishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }
        
        public async Task<Wishlist> Clone(Guid id, string newName)
        {
            var existingWishlist = await _wishlistRepository.Load(id);
            var newWishlist = existingWishlist.DeepClone();
            newWishlist.Id = Guid.NewGuid();
            newWishlist.Name = newName;
            foreach (var person in newWishlist.People)
            {
                person.PresentIdeas.Clear();
            }
            
            await _wishlistRepository.Save(newWishlist);
            return newWishlist;
        }
    }
}