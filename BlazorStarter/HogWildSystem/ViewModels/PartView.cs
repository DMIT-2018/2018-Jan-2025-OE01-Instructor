using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HogWildSystem.ViewModels
{
    public class PartView
    {
        public int PartID { get; set; }
        public int PartCategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public decimal Price { get; set; }
        //Re-order Level
        public int ROL { get; set; }
        //Quantity on Hand
        public int QOH { get; set; }
        public bool Taxable { get; set; }
        public bool RemoveFromViewFlag { get; set; }
    }
}
