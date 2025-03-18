using HogWildSystem.BLL;
using HogWildSystem.ViewModels;
using Microsoft.AspNetCore.Components;

namespace BlazorWebApp.Components.Pages.SamplePages
{
    public partial class CustomerEdit
    {
        #region Fields
        private CustomerEditView customer = new();
        //Lists to populate when the page opens
        //Used to populate values in the select lists
        private List<LookupView> provices = [];
        private List<LookupView> countries = [];
        private List<LookupView> statuses = [];
        private string closeButtonText = "Close";

        //Error Messages and Feedback
        private string? errorMessage;
        private List<string> errorDetails = [];
        private string feedbackMessage = string.Empty;
        #endregion

        #region Properties
        [Inject] 
        protected LookupService LookupService { get; set; } = default!;
        [Inject]
        protected CustomerService CustomerService { get; set; } = default!;
        [Parameter]
        public int CustomerID { get; set; }
        #endregion

        protected override void OnInitialized()
        {
            //Anytime a Service is used, it MUST be in a try/catch
            try
            {
                //Clear any errors, just in case
                errorDetails.Clear();
                errorMessage = string.Empty;
                feedbackMessage = string.Empty;

                //Check to see if this is a new Customer (CustomerID == 0)
                // or an existing Customer (so an edit)
                if (CustomerID > 0)
                {
                    //Remember: ?? means if null, set the value to ___ (in this case a new record)
                    customer = CustomerService.GetCustomer(CustomerID) ?? new();
                    closeButtonText = "Cancel";
                }

                // populate the lookups for the selects
                provices = LookupService.GetLookups("Province");
                countries = LookupService.GetLookups("Country");
                statuses = LookupService.GetLookups("Customer Status");
            }
            catch (ArgumentNullException ex)
            {
                errorMessage = BlazorHelperClass.GetInnerException(ex).Message;
            }
            catch (ArgumentException ex)
            {
                errorMessage = BlazorHelperClass.GetInnerException(ex).Message;
            }
            catch (AggregateException ex)
            {
                //  have a collection of errors
                //  each error should be place into a separate line
                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    errorMessage = $"{errorMessage}{Environment.NewLine}";
                }

                errorMessage = $"{errorMessage}Unable to search for customer";
                foreach (var error in ex.InnerExceptions)
                {
                    errorDetails.Add(error.Message);
                }
            }
            catch (Exception ex)
            {
                errorMessage = BlazorHelperClass.GetInnerException(ex).Message;
            }
        }
    }
}
