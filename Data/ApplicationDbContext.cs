using System;
using Beerhall.Models.Domain;
using Beerhall.Data.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Beerhall.Data
{
    public class ApplicationDbContext : DbContext
    {

        public DbSet<Brewer> Brewers { get; set; }
        public DbSet<Location> Locations { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new BrewerConfiguration());
            modelBuilder.ApplyConfiguration(new LocationConfiguration());
            //modelBuilder.ApplyConfiguration(new BeerConfiguration());
        }
    }
}
