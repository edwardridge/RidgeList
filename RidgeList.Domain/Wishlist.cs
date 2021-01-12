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
    
    public class Wishlist
    {
        public static Wishlist Create(string name, string emailOfCreator)
        {
            return new Wishlist()
            {
                Id = Guid.NewGuid(),
                Name =  name,
                People = new WishlistPeople{ new WishlistPerson() { Email = emailOfCreator } },
                Creator = emailOfCreator
            };
        }

        public Wishlist()
        {
            this.People = new WishlistPeople();
        }

        public Guid Id { get; init; }

        private WishlistPeople People { get; set; }
        
        public string Name { get; set; }
        
        public string Creator { get; set; }

        public void AddPerson(string name, string email)
        {
            if (this.People.ContainsEmail(email) == false)
            {
                this.People.Add(new WishlistPerson() { Email = email, Name = name});
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
        public string Name { get; set; }

        public string Email { get; set; }
    }
}