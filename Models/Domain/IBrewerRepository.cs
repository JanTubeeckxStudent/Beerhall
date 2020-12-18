using System;
using System.Collections.Generic;

namespace Beerhall.Models.Domain
{
    public interface IBrewerRepository
    {
        Brewer GetBy(int brewerId);
        IEnumerable<Brewer> getAll();
        void Add(Brewer brewer);
        void Delete(Brewer brewer);
        void SaveChanges();
    }
}
