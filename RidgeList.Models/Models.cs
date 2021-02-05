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