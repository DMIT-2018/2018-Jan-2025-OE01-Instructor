using HogWildSystem.BLL;
using HogWildSystem.ViewModels;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlazorWebApp.Components.Pages.SamplePages
{
    public partial class CustomerEdit
    {
        #region Fields
        private CustomerEditView customer = new();
        //Lists to populate when the page opens
        //Used to populate values in the select lists
        private List<LookupView> provinces = [];
        private List<LookupView> countries = [];
        private List<LookupView> statuses = [];
        private string closeButtonText = "Close";

        //form setup fields
        private MudForm customerForm = new();
        private bool isFormValid;

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
        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        protected IDialogService DialogService { get; set; } = default!;
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
                provinces = LookupService.GetLookups("Province");
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

        private async Task CancelAsync()
        {
            bool? results = await DialogService.ShowMessageBox($"{closeButtonText}", $"Are you sure you want to {closeButtonText}?", yesText: $"{closeButtonText}", cancelText: "Cancel");

            if(results == true)
                NavigationManager.NavigateTo("/SamplePages/Customers");
        }

        private void NewInvoice()
        {
            //  NOTE: we will hard code employee ID (1)            
            NavigationManager.NavigateTo($"/SamplePages/InvoiceEdit/0/{CustomerID}/1");
        }

        private void Save()
        {
            //Double Check Validation (not required, but for paranoia sake)
            if (!isFormValid)
                return;
            //  reset the error detail list
            errorDetails.Clear();

            //  reset the error message to an empty string
            errorMessage = string.Empty;

            //  reset feedback message to an empty string
            feedbackMessage = String.Empty;
            try
            {
                customer = CustomerService.AddOrEditCustomer(customer);
                feedbackMessage = "Data was successfully saved!";
            }
            catch (ArgumentNullException ex)
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
                errorMessage = $"{errorMessage}Unable to save the customer";
                foreach (var error in ex.InnerExceptions)
                {
                    errorDetails.Add(error.Message);
                }
            }
            catch(Exception ex)
            {
                errorMessage = BlazorHelperClass.GetInnerException(ex).Message;
            }
        }
    }
}
