using HogWildSystem.DAL;
using HogWildSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HogWildSystem.BLL
{
    public class PartService
    {
        private readonly HogWildContext _context;

        //Constructor for the service class
        internal PartService(HogWildContext context)
        {
            _context = context;
        }

        #region Methods
        public List<PartView> GetParts(int partCategoryID, string description)
        {
            //Business Rules

            //rule: both part category id and/or description cannot be empty
            if (partCategoryID == 0 && string.IsNullOrWhiteSpace(description))
                throw new ArgumentNullException("Please provide either a category and/or a description.");

            Guid tempGuid = Guid.NewGuid();
            if(string.IsNullOrWhiteSpace(description))
            {
                description = tempGuid.ToString();
            }

            return _context.Parts
                    .Where(x => (x.Description.ToLower().Contains(description.ToLower().Trim())
                        || x.PartCategoryID == partCategoryID) && !x.RemoveFromViewFlag)
                    .Select(x => new PartView
                    {
                        PartID = x.PartID,
                        PartCategoryID = x.PartCategoryID,
                        CategoryName = x.PartCategory.Name,
                        Description = x.Description,
                        Cost = x.Cost,
                        Price = x.Price,
                        ROL = x.ROL,
                        QOH = x.QOH,
                        Taxable = x.Taxable,
                        RemoveFromViewFlag = x.RemoveFromViewFlag
                    })
                    .OrderBy(x => x.Description)
                    .ToList();
        }

        public PartView? GetPart(int partID)
        {
            //rule: PartID must be valid
            if (partID == 0)
                throw new ArgumentNullException("Please provide a part id.");

            return _context.Parts
                .Where(x => x.PartID == partID
                        && !x.RemoveFromViewFlag)
                .Select(x => new PartView
                {
                    PartID = x.PartID,
                    PartCategoryID = x.PartCategoryID,
                    CategoryName = x.PartCategory.Name,
                    Description = x.Description,
                    Cost = x.Cost,
                    Price = x.Price,
                    ROL = x.ROL,
                    QOH = x.QOH,
                    Taxable = x.Taxable,
                    RemoveFromViewFlag = x.RemoveFromViewFlag
                })
                .FirstOrDefault();
        }
        #endregion
    }
}
