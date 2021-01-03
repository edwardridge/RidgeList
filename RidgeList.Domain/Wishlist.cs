using System;
using System.Collections.Generic;

namespace RidgeList.Domain
{
    public class WishlistSummary
    {
        public string Name { get; set; }
    }
    
    public class Wishlist
    {
        public static Wishlist Create(string name)
        {
            return new Wishlist()
            {
                Id = Guid.NewGuid(),
                Name =  name
            };
        }

        public Wishlist()
        {
            this.People = new List<string>();
        }

        public Guid Id { get; init; }

        private List<string> People { get; set; }
        
        public string Name { get; set; }

        public void AddPerson(string email)
        {
            this.People.Add(email);
        }

        public List<string> GetPeople()
        {
            return this.People;
        }
    }
}