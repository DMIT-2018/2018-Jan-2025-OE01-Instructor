using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HogWildSystem.ViewModels
{
    public class InvoiceCustomerView
    {
        public int InvoiceID { get; set; }
        public DateOnly InvoiceDate { get; set; }
        public decimal Total { get; set; }
    }
}
