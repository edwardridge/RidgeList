using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RidgeList.Domain
{
    public class WishlistSummary
    {
        public Guid Id { get; set; }        
        
        public string Name { get; set; }
    }

    public class User
    {

        public User()
        {
            this.Wishlists = new List<Guid>();
        }

        public Guid Id { get; set; }

        public string Email { get; set; }

        public List<Guid> Wishlists { get; set; }

        public string Name { get; set; }
    }
    
    public class Wishlist
    {
        public static Wishlist Create(string name, Guid idOfCreator, bool creatorIsGiftee = true)
        {
            return new Wishlist()
            {  
                Id = Guid.NewGuid(),
                Name =  name,
                People = new WishlistPeople{ new WishlistPerson() { PersonId = idOfCreator, Giftee = creatorIsGiftee} },
                CreatorId = idOfCreator
            };
        }

        public Wishlist()
        {
            this.People = new WishlistPeople();
        }

        public Guid Id { get; init; }

        public WishlistPeople People { get; set; }
        
        public string Name { get; set; }
        
        public Guid CreatorId { get; set; }

        public void AddPerson(Guid personId, bool isGiftee)
        {
            if (this.People.ContainsPerson(personId) == false)
            {
                this.People.Add(new WishlistPerson() { PersonId = personId, Giftee = isGiftee });
            }
        }

        public WishlistPeople GetPeople()
        {
            return this.People;
        }

        public void AddGiftIdea(Guid personId, string present)
        {
            this.GetPerson(personId)?.AddPresentIdea(present);
        }

        public WishlistPerson GetPerson(Guid id)
        {
            return this.People.SingleOrDefault(s => s.PersonId == id);
        }

        public void ClaimGift(Guid presentId, Guid claimerId)
        {
            this.People.SelectMany(s => s.PresentIdeas).First(s => s.Id == presentId).ClaimerId = claimerId;
        }

        public void UnclaimPresent(Guid presentId)
        {
            this.People.SelectMany(s => s.PresentIdeas).First(s => s.Id == presentId).ClaimerId = null;
        }

        public void RemoveGiftIdea(Guid creatorId, Guid presentId)
        {
            var present = this.GetPerson(creatorId).PresentIdeas.Single(s => s.Id == presentId);
            this.GetPerson(creatorId).PresentIdeas.Remove(present);
        }
    }

    public class WishlistPeople : Collection<WishlistPerson>
    {

        public bool ContainsPerson(Guid personId)
        {
            return this.Any(s => s.PersonId == personId);
        }
    }

    public class WishlistPerson
    {
        public WishlistPerson()
        {
            this.PresentIdeas = new List<PresentIdea>();
        }
        
        public Guid PersonId { get; set; }
        
        public List<PresentIdea> PresentIdeas { get; set; }
        
        public bool Giftee { get; set; }

        public void AddPresentIdea(string present)
        {
            this.PresentIdeas.Add(
                new PresentIdea()
                {
                    Id = Guid.NewGuid(),
                    Description = present
                });
        }
    }

    public class PresentIdea
    {
        public string Description { get; set; }
        
        public Guid Id { get; set; }
        
        public Guid? ClaimerId { get; set; }
    }
}