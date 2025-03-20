using HogWildSystem.DAL;
using HogWildSystem.Entities;
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
            if (customerID == 0)
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
                    RemoveFromViewFlag = x.RemoveFromViewFlag,
                    Invoices = x.Invoices
                                .Where(i => !i.RemoveFromViewFlag)
                                .Select(i => new InvoiceCustomerView
                                {
                                    InvoiceID = i.InvoiceID,
                                    InvoiceDate = i.InvoiceDate,
                                    Total = i.SubTotal + i.Tax
                                })
                                .OrderByDescending(i => i.InvoiceDate)
                                .ToList()
                }).FirstOrDefault();
        }

        public CustomerEditView AddOrEditCustomer(CustomerEditView editCustomer)
        {
            List<Exception> errorList = [];

            //rule: Customer cannot be null
            if (editCustomer == null)
            {
                throw new ArgumentNullException("No customer was supplied.");
            }

            //rule: first Name, last name, phone number, email are required
            if (string.IsNullOrWhiteSpace(editCustomer.FirstName))
            {
                errorList.Add(new ArgumentNullException("First Name is required."));
            }
            if (string.IsNullOrWhiteSpace(editCustomer.LastName))
            {
                errorList.Add(new ArgumentNullException("Last Name is required."));
            }
            if (string.IsNullOrWhiteSpace(editCustomer.Email))
            {
                errorList.Add(new ArgumentNullException("Email is required."));
            }
            if (string.IsNullOrWhiteSpace(editCustomer.Phone))
            {
                errorList.Add(new ArgumentNullException("Phone Number is required."));
            }
            if (string.IsNullOrWhiteSpace(editCustomer.Address1))
            {
                errorList.Add(new ArgumentNullException("Address is required."));
            }
            if (string.IsNullOrWhiteSpace(editCustomer.City))
            {
                errorList.Add(new ArgumentNullException("City is required."));
            }
            if (editCustomer.ProvStateID == null || editCustomer.ProvStateID == 0)
            {
                errorList.Add(new Exception("ProvStateID is required"));
            }
            if (editCustomer.CountryID == null || editCustomer.CountryID == 0)
            {
                errorList.Add(new Exception("CountryID is required"));
            }
            if (editCustomer.StatusID == null || editCustomer.StatusID == 0)
            {
                errorList.Add(new Exception("StatusID is required"));
            }

            //Should also be checking length (not required for this course)
            //for DMIT2018 you only have to validate on the Input (in the textfield) stuff like Length
            // In General, for service methods you should be validating that the record can actually
            // go into your database properly.

            // rule: first name, last name, and phone number cannot be duplicated (must be unique)
            bool customerExist = false;
            if (editCustomer.CustomerID == 0)
            {
                customerExist = _context.Customers
                                .Any(x => x.FirstName == editCustomer.FirstName
                                        && x.LastName == editCustomer.LastName
                                        && x.Phone == editCustomer.Phone);
            }
            else
            {
                // For existing customer, add the not CustomerID
                // If you edit something other than FirstName, Lastname, or phone
                // You want to make sure you don't block those edits.
                // !(CustomerID == CustomerID) make sure we check all other customers
                // in case the user has edited one customer to match another
                // Example: Customer 1: Jimmy Bob 555-555-5555
                //          Customer 2: Jimmy John 555-555-4444
                // I cannot edit Customer 2 to be Jimmy Bob 555-555-5555 (matching Customer 1)
                customerExist = _context.Customers
                                .Any(x => x.FirstName == editCustomer.FirstName
                                        && x.LastName == editCustomer.LastName
                                        && x.Phone == editCustomer.Phone
                                        && !(x.CustomerID == editCustomer.CustomerID));
            }
            if(customerExist)
            {
                errorList.Add(new ArgumentException("Customer already exists in the database with the enter first name, last name, and phone number."));
            }

            Customer? customer = _context.Customers
                                .Where(x => x.CustomerID == editCustomer.CustomerID)
                                .FirstOrDefault();
            //check if null to ensure it's a new customer
            //Create a new customer record if null
            if(customer == null)
            {
                customer = new();
            }

            //Update all fields
            customer.FirstName = editCustomer.FirstName;
            customer.LastName = editCustomer.LastName;
            customer.Address1 = editCustomer.Address1;
            customer.Address2 = editCustomer.Address2;
            customer.City = editCustomer.City;
            //Must check if it has a value or use the null check (??) when it is a nullable int (int?)
            customer.ProvStateID = editCustomer.ProvStateID.HasValue ? editCustomer.ProvStateID.Value : 0;
            customer.CountryID = editCustomer.CountryID ?? 0;
            customer.PostalCode = editCustomer.PostalCode;
            customer.Email = editCustomer.Email;
            customer.Phone = editCustomer.Phone;
            customer.StatusID = editCustomer.StatusID ?? 0;
            customer.RemoveFromViewFlag = editCustomer.RemoveFromViewFlag;

            if(errorList.Count > 0)
            {
                _context.ChangeTracker.Clear();
                throw new AggregateException("Unable to add or edit the customer. Please check error message(s)", errorList);
            }
            else
            {
                //check if new or existing and Stage the database changes
                // remember .Add and .Update are only local (in memory)
                if(customer.CustomerID == 0)
                    _context.Customers.Add(customer);
                else
                    _context.Customers.Update(customer);

                //Actually save to the database
                _context.SaveChanges();

                //Can return the EditCustomer (even if new)
                // In case it is new, get the new CustomerID from the customer record
                editCustomer.CustomerID = customer.CustomerID;
                return editCustomer;
            }
        }
    }
}
