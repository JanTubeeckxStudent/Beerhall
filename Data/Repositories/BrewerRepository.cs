using System;
using System.Collections.Generic;
using System.Linq;
using Beerhall.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Beerhall.Data.Repositories
{
    public class BrewerRepository : IBrewerRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<Brewer> _brewers;

        public BrewerRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _brewers = dbContext.Brewers;
        }

        public void Add(Brewer brewer)
        {
            _brewers.Add(brewer);
        }

        public void Delete(Brewer brewer)
        {
            _brewers.Remove(brewer);
        }

        public IEnumerable<Brewer> getAll()
        {
            return _brewers.Include(b => b.Location).ToList();
        }

        public Brewer GetBy(int brewerId)
        {
            return _brewers.SingleOrDefault(b => b.BrewerId == brewerId);
        }

        public Brewer GetByWithBeers(int brewerId)
        {
            return _brewers.Include(b => b.Beers).SingleOrDefault(b => b.BrewerId == brewerId);
        }

        public IEnumerable<Brewer> GetAllWithBeers()
        {
            return _brewers.Include(b => b.Location).Include(b => b.Beers).ToList();
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}
