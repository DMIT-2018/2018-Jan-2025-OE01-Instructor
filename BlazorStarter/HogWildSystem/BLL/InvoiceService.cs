using HogWildSystem.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HogWildSystem.BLL
{
    public class InvoiceService
    {
        private readonly HogWildContext _context;

        //Constructor for the service class
        internal InvoiceService(HogWildContext context)
        {
            _context = context;
        }
    }
}
