using CareoBack.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareoBack.DAL
{
    public class Context : IdentityDbContext<AppUser>
    {
        public Context(DbContextOptions<Context> options):base(options)
        {

        }
        public DbSet<Customer> Customers { get; set; }
    }
}
