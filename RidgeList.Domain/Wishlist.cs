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

    public class UserWishlists
    {

        public UserWishlists()
        {
            this.Wishlists = new List<Guid>();
        }
        public string Email { get; set; }

        public List<Guid> Wishlists { get; set; }
    }
    
    public class Wishlist
    {
        public static Wishlist Create(string name, string emailOfCreator, string nameOfCreator, bool creatorIsGiftee = true)
        {
            return new Wishlist()
            {  
                Id = Guid.NewGuid(),
                Name =  name,
                People = new WishlistPeople{ new WishlistPerson() { Email = emailOfCreator, Name = nameOfCreator,Giftee = creatorIsGiftee} },
                Creator = emailOfCreator
            };
        }

        public Wishlist()
        {
            this.People = new WishlistPeople();
        }

        public Guid Id { get; init; }

        public WishlistPeople People { get; set; }
        
        public string Name { get; set; }
        
        public string Creator { get; set; }

        public void AddPerson(string name, string email, bool isGiftee)
        {
            if (this.People.ContainsEmail(email) == false)
            {
                this.People.Add(new WishlistPerson() { Email = email, Name = name, Giftee = isGiftee });
            }
        }

        public WishlistPeople GetPeople()
        {
            return this.People;
        }

        public void EditPerson(string email, string newName)
        {
            this.People.Single(s => s.Email == email).Name = newName;
        }

        public void AddGiftIdea(string email, string present)
        {
            this.GetPerson(email)?.AddPresentIdea(present);
        }

        public WishlistPerson GetPerson(string email)
        {
            return this.People.SingleOrDefault(s => s.Email == email);
        }

        public void ClaimGift(Guid presentId, string emailOfClaimer)
        {
            this.People.SelectMany(s => s.PresentIdeas).First(s => s.Id == presentId).Claimer = emailOfClaimer;
        }

        public void UnclaimPresent(Guid presentId)
        {
            this.People.SelectMany(s => s.PresentIdeas).First(s => s.Id == presentId).Claimer = null;
        }

        public void RemoveGiftIdea(string emailOfCreator, Guid presentId)
        {
            var present = this.GetPerson(emailOfCreator).PresentIdeas.Single(s => s.Id == presentId);
            this.GetPerson(emailOfCreator).PresentIdeas.Remove(present);
        }
    }

    public class WishlistPeople : Collection<WishlistPerson>
    {
        public bool ContainsEmail(string email)
        {
            return this.Any(s => s.Email == email);
        }
    }

    public class WishlistPerson
    {
        public WishlistPerson()
        {
            this.PresentIdeas = new List<PresentIdea>();
        }
        
        public string Name { get; set; }

        public string Email { get; set; }
        
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
        
        public string Claimer { get; set; }
    }
}