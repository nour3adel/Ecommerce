﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace TechXpress.Data.Models
{
    public class AppUser:IdentityUser
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Address { get; set; }

        public ShoppingCart ShoppingCart { get; set; }

        public IEnumerable<Order> Orders { get; set; }
    }
}
