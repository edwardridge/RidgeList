using System;
using System.Collections.Generic;
using System.Linq;
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
        public WishlistModel Map(Wishlist wishlist)
        {
            return new WishlistModel()
            {
                Id = wishlist.Id,
                Name = wishlist.Name,
                People = wishlist.GetPeople().Select(s => 
                    new WishlistPersonModel()
                    {
                        Name = s.Name, 
                        Email = s.Email,
                        Giftee = s.Giftee,
                        PresentIdeas = s.PresentIdeas
                            .Select(t => new PresentIdeaModel()
                            {
                                Id = t.Id,
                                Description = t.Description,
                                ClaimerName = 
                                    string.IsNullOrEmpty(t.Claimer) ? 
                                        null : 
                                        wishlist.GetPeople().Single(g => g.Email == t.Claimer).Name,
                                ClaimerEmail= 
                                string.IsNullOrEmpty(t.Claimer) ? 
                                null : 
                                wishlist.GetPeople().Single(g => g.Email == t.Claimer).Email
                            }).ToList()
                    }).ToList()
            };
        }
    }

    public class WishlistPersonModel
    {
        public string Email { get; set; }
        
        public string Name { get; set; }

        public List<PresentIdeaModel> PresentIdeas { get; set; }
        
        public bool Giftee { get; set; }
    }

    public class PresentIdeaModel
    {
        public Guid Id { get; set; }

        public string Description { get; set; }
        
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