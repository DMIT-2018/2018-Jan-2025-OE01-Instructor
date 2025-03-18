using HogWildSystem.DAL;
using HogWildSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HogWildSystem.BLL
{
    public class CustomerService
    {
        private readonly HogWildContext _context;

        //Constructor for the service class
        internal CustomerService(HogWildContext context)
        {
            _context = context;
        }

        public List<CustomerSearchView> GetCustomers(string lastName, string phone)
        {
            //Rule: Either last name or phone cannot be empty, one must have a value
            if (string.IsNullOrWhiteSpace(lastName) && string.IsNullOrWhiteSpace(phone))
                throw new ArgumentNullException("Please provide either a last name and/or phone number.");

            //Need to update parameters so we are not searching on an empty value.
            //If we don't give the search parameters a random value, an empty string 
            //will return all records.
            if (string.IsNullOrWhiteSpace(lastName))
                lastName = Guid.NewGuid().ToString();
            if (string.IsNullOrWhiteSpace(phone))
                phone = Guid.NewGuid().ToString();

            return _context.Customers
                    .Where(x => (x.LastName.ToLower().Contains(lastName.ToLower().Trim())
                        || x.Phone.Contains(phone.Trim()))
                        && !x.RemoveFromViewFlag)
                    .Select(x => new CustomerSearchView
                    {
                        CustomerID = x.CustomerID,
                        FullName = x.FirstName + " " + x.LastName,
                        City = x.City,
                        Phone = x.Phone,
                        Email = x.Email,
                        StatusID = x.StatusID,
                        TotalSales = x.Invoices.Sum(x => x.SubTotal + x.Tax)
                    })
                    .OrderBy(x => x.FullName)
                    .ToList();
        }

        public CustomerEditView? GetCustomer(int customerID)
        {
            //rule: CustomerID must be valid (> 0)
            if(customerID == 0)
            {
                throw new ArgumentNullException("Please provide a customer ID.");
            }

            return _context.Customers
                .Where(x => (x.CustomerID == customerID
                             && !x.RemoveFromViewFlag))
                .Select(x => new CustomerEditView
                {
                    CustomerID = x.CustomerID,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Address1 = x.Address1,
                    Address2 = x.Address2,
                    City = x.City,
                    ProvStateID = x.ProvStateID,
                    CountryID = x.CountryID,
                    PostalCode = x.PostalCode,
                    Phone = x.Phone,
                    Email = x.Email,
                    StatusID = x.StatusID,
                    RemoveFromViewFlag = x.RemoveFromViewFlag
                }).FirstOrDefault();
        }
    }
}
