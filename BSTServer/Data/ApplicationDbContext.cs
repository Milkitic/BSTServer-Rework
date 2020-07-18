using System;
using System.Collections.Generic;
using System.Text;
using BSTServer.Models;
using Microsoft.EntityFrameworkCore;

namespace BSTServer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Posts { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
