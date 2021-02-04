using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RidgeList.Domain;

namespace RidgeList.Models
{
    public class WishlistSummaryModel
    {
        public string Name { get; set; }        
        
        public Guid Id { get; set; }

        public static WishlistSummaryModel Map(WishlistSummary summary)
        {
            return new WishlistSummaryModel()
            {
                Id = summary.Id,
                Name = summary.Name
            };
        }
    }

    public class WishlistMapper
    {
        private IWishlistSummaryRepository wishlistSummariesRepo;

        public WishlistMapper(IWishlistSummaryRepository wishlistSummariesRepo)
        {
            this.wishlistSummariesRepo = wishlistSummariesRepo;
        }

        public async Task<WishlistModel> Map(Wishlist wishlist)
        {
            var userWishlists = new List<UserWishlists>();
            foreach(var person in wishlist.People)
            {
                var userWishlist = await wishlistSummariesRepo.GetDetails(person.PersonId);
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
                                ClaimerEmail=
                                t.ClaimerId.HasValue == false ?
                                        null :
                                userWishlists.Single(g => g.Id == t.ClaimerId).Email
                            }).ToList()
                    }).ToList()
            };
        }
    }

    public class WishlistPersonModel
    {
        public Guid PersonId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public List<PresentIdeaModel> PresentIdeas { get; set; }
        
        public bool Giftee { get; set; }
    }

    public class PresentIdeaModel
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public Guid? ClaimerId { get; set; }

        public string ClaimerName { get; set; }
        
        public string ClaimerEmail { get; set; }
    }
    
    public class WishlistModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<WishlistPersonModel> People { get; set; }
    }
}