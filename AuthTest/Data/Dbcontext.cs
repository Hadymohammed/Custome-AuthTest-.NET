using AuthTest.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Numerics;

namespace AuthTest.Data
{
    public class Dbcontext : DbContext
    {
        public Dbcontext(DbContextOptions<Dbcontext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
    }
}
