using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlashApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FlashApp.Data
{
    public class ApplicationDBContext : IdentityDbContext<AppUser>
    {
        private const string ADMINGUID = "b8e61f75-ca0d-4399-8842-2853e1ca1649";
        private const string USERGUID = "29e1772e-cdcc-4ce0-9bf2-c548f2e241db";

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options) { }

        public DbSet<Flashcard> Flashcards { get; set; }
        public DbSet<Deck> Decks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = ADMINGUID,
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                },
                new IdentityRole
                {
                    Id = USERGUID,
                    Name = "User",
                    NormalizedName = "USER",
                },
            };
            modelBuilder.Entity<IdentityRole>().HasData(roles);

            modelBuilder
                .Entity<AppUser>()
                .HasMany(u => u.Decks)
                .WithOne(d => d.AppUser)
                .HasForeignKey(d => d.AppUserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<AppUser>()
                .HasMany(u => u.Flashcards)
                .WithOne(f => f.AppUser)
                .HasForeignKey(f => f.AppUserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
