using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data.Seeder;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api
{
    public class DataContext: IdentityDbContext<AppUser, AppRole, int, IdentityUserClaim<int>
       , AppUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>

    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) 
        {
            
        }

        public DbSet<LatestActivity> LatestActivities { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
               .HasMany(ur => ur.UserRoles)
               .WithOne(u => u.User)
               .HasForeignKey(f => f.UserId)
               .IsRequired();

            builder.Entity<AppRole>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.Role)
                .HasForeignKey(f => f.RoleId)
                .IsRequired();
        }
    }
}