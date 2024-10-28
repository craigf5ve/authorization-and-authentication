using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronMan.Data.DbContexts;

namespace IronMan.Data.Repos
{
    public interface IIronManRepository
    {
        
    }
    public class IronManRepository : IIronManRepository
    {

         private readonly IronManDbContext _context;

        public IronManRepository(IronManDbContext context)
        {
            _context = context;
        }

    }
}