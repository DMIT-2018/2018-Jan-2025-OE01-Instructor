using HogWildSystem.BLL;
using HogWildSystem.ViewModels;
using Microsoft.AspNetCore.Components;

namespace BlazorWebApp.Components.Pages.SamplePages
{
    public partial class Customers
    {
        #region Fields
        private string lastName = string.Empty;
        private string phoneNumber = string.Empty;
        private List<CustomerSearchView> customerList = [];
        private bool noRecords;

        //Messages
        private string feedbackMessage = string.Empty;
        public List<string> errorMessages = [];
        #endregion

        #region Parameters
        [Inject]
        protected CustomerService CustomerService { get; set; } = default!;
        #endregion

        #region Methods
        private void Search()
        {
            // reset the error Messages and Feedback
            errorMessages.Clear();
            feedbackMessage = string.Empty;

            // clear any previous search results
            customerList.Clear();
            noRecords = false;

            try
            {
                //Validate the input - Check if both are empty
                if (string.IsNullOrWhiteSpace(lastName) && string.IsNullOrWhiteSpace(phoneNumber))
                    throw new ArgumentException("Please provide either a last name and/or phone number.");

                //search for customers 
                customerList = CustomerService.GetCustomers(lastName, phoneNumber);
                if (customerList.Count > 0)
                {
                    feedbackMessage = "Search for customer(s) was successful.";
                } 
                else
                {
                    feedbackMessage = "No customers were found for your provided search criteria.";
                    noRecords = true;
                }
            }
            catch(AggregateException ex)
            {
                foreach (var error in ex.InnerExceptions)
                {
                    errorMessages.Add(error.Message);
                }
            }
            catch(ArgumentNullException ex)
            {
                errorMessages.Add(BlazorHelperClass.GetInnerException(ex).Message);
            }
            catch(Exception ex)
            {
                errorMessages.Add(BlazorHelperClass.GetInnerException(ex).Message);
            }
            
                
        }
        #endregion
    }
}
