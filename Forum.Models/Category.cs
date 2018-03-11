using System;
using System.Collections.Generic;

namespace Forum.Models
{
    public class Category
    {
        public Category(int id, string name, ICollection<int> posts)
        {
            this.Id = id;
            this.Name = name;
            this.Posts = posts;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<int> Posts { get; set; }
    }
}
