using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Function.Movies.API.Data
{

    public class Category
    {
        public int id { get; set; }

        public string CategoryName { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }
    }
}
