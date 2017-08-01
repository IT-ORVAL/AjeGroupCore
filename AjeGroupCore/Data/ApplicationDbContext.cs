using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AjeGroupCore.Models;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AjeGroupCore.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>().Property(u => u.UserName).HasMaxLength(255);
            builder.Entity<ApplicationUser>().Property(u => u.Email).HasMaxLength(255);
            builder.Entity<IdentityRole>().Property(r => r.Name).HasMaxLength(255);

            //builder.Entity<HistoryRow>().Property(h => h.MigrationId).HasMaxLength(100).IsRequired();

        }
    }
}
