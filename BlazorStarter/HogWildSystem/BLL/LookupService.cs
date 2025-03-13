using HogWildSystem.DAL;
using HogWildSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HogWildSystem.BLL
{
    public class LookupService
    {
        private readonly HogWildContext _context;

        //Constructor for the service class
        internal LookupService(HogWildContext context)
        {
            _context = context;
        }

        #region Methods
         public List<LookupView> GetLookups(string categoryName)
        {
            return _context.Lookups
                .Where(x => x.Category.CategoryName == categoryName)
                .OrderBy(x => x.Name)
                .Select(x => new LookupView
                {
                    LookupID = x.LookupID,
                    CategoryID = x.CategoryID,
                    Name = x.Name,
                    RemoveFromViewFlag = x.RemoveFromViewFlag
                }).ToList();
        }
        #endregion
    }
}
