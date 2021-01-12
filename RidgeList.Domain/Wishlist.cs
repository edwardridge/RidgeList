using System;
using System.Collections.Generic;

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
                People = { emailOfCreator },
                Creator = emailOfCreator
            };
        }

        public Wishlist()
        {
            this.People = new List<string>();
        }

        public Guid Id { get; init; }

        private List<string> People { get; set; }
        
        public string Name { get; set; }
        
        public string Creator { get; set; }

        public void AddPerson(string email)
        {
            if (this.People.Contains(email) == false)
            {
                this.People.Add(email);
            }
        }

        public List<string> GetPeople()
        {
            return this.People;
        }
    }
}