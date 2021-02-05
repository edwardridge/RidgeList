using RidgeList.Domain;
using RidgeList.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RidgeList.ApplicationServices
{
    public class WishlistMapper
    {
        private IUserRepository userRepo;

        public WishlistMapper(IUserRepository userRepo)
        {
            this.userRepo = userRepo;
        }

        public async Task<WishlistModel> Map(Wishlist wishlist)
        {
            var userWishlists = new List<User>();
            foreach (var person in wishlist.People)
            {
                var userWishlist = await userRepo.GetUser(person.PersonId);
                userWishlists.Add(userWishlist);
            }

            return new WishlistModel()
            {
                Id = wishlist.Id,
                Name = wishlist.Name,
                People = wishlist.GetPeople().Select(s =>
                    new WishlistPersonModel()
                    {
                        PersonId = s.PersonId,
                        Giftee = s.Giftee,
                        Name = userWishlists.Single(g => g.Id == s.PersonId).Name,
                        Email = userWishlists.Single(g => g.Id == s.PersonId).Email,
                        PresentIdeas = s.PresentIdeas
                            .Select(t => new PresentIdeaModel()
                            {
                                Id = t.Id,
                                Description = t.Description,
                                ClaimerId = t.ClaimerId.HasValue == false ?
                                        null :
                                        t.ClaimerId,
                                ClaimerName =
                                    t.ClaimerId.HasValue == false ?
                                        null :
                                        userWishlists.Single(g => g.Id == t.ClaimerId).Name,
                                ClaimerEmail =
                                t.ClaimerId.HasValue == false ?
                                        null :
                                userWishlists.Single(g => g.Id == t.ClaimerId).Email
                            }).ToList()
                    }).ToList()
            };
        }
    }
}
