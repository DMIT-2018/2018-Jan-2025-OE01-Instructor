using HogWildSystem.BLL;
using HogWildSystem.ViewModels;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlazorWebApp.Components.Pages.SamplePages
{
    public partial class InvoiceEdit
    {
        #region Fields
        private InvoiceView invoice = new();

        //Part Search
        private int? categoryID;
        private List<LookupView> partCategories = [];
        private string description = string.Empty;
        private bool noParts;
        private List<PartView> parts = [];
        private List<PartView> partList = [];

        //Errors and Feedback
        private List<string> errorDetails = [];
        private string errorMessage = string.Empty;
        private string feedbackMessage = string.Empty;
        #endregion
        #region Properties
        //These Parameter datatypes must match what is in the @page declaration
        [Parameter]
        public int InvoiceID { get; set; }
        [Parameter]
        public int CustomerID { get; set; }
        [Parameter]
        public int EmployeeID { get; set; }

        //Injected Services
        [Inject]
        protected InvoiceService InvoiceService { get; set; } = default!;
        [Inject]
        protected PartService PartService { get; set; } = default!;
        [Inject] 
        protected LookupService LookupService { get; set; } = default!;
        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;
        //So we can use a dialogue
        [Inject]
        protected IDialogService DialogService { get; set; } = default!;
        #endregion

        #region Methods
        protected override void OnInitialized()
        {
            try
            {
                invoice = InvoiceService.GetInvoice(InvoiceID, CustomerID, EmployeeID);
                partCategories = LookupService.GetLookups("Part Categories");
                partList = PartService.GetAllParts();
            }
            catch(ArgumentNullException ex)
            {
                errorMessage = BlazorHelperClass.GetInnerException(ex).Message;
            }
            catch(AggregateException ex)
            {
                if(!string.IsNullOrWhiteSpace(errorMessage))
                {
                    errorMessage = $"{errorMessage}{Environment.NewLine}";
                }
                //Debug here
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
        private void SearchParts()
        {
            try
            {
                // reset the error detail list
                errorDetails.Clear();

                //reset the error message
                errorMessage = string.Empty;

                //reset feedback message
                feedbackMessage = string.Empty;

                //clear the part list before we search to fill it again
                parts.Clear();

                //reset no parts to false
                noParts = false;

                //rule: Category ID or description must have a value.
                if (!categoryID.HasValue && string.IsNullOrWhiteSpace(description))
                    throw new ArgumentException("Please provide either a category and/or a description.");

                //search for the parts using the service
                //If CategoryID has a value since it is nullable we can give the value
                //if there is no value we give the default int value of 0
                parts = PartService.GetParts(categoryID.HasValue ? categoryID.Value : 0, description);

                if(parts.Count() > 0)
                {
                    feedbackMessage = "Search for part(s) was successful.";
                }
                else
                {
                    //set noParts = true because the search worked but nothing was found
                    noParts = true;
                    feedbackMessage = "No parts were found for your search criteria.";
                }
            }
            catch(ArgumentNullException ex)
            {
                errorMessage = BlazorHelperClass.GetInnerException(ex).Message;
            }
            catch(AggregateException ex)
            {
                if(!string.IsNullOrWhiteSpace(errorMessage))
                {
                    errorMessage = $"{errorMessage}{Environment.NewLine}";
                }
                //Debug here
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
        private void AddPart(PartView part)
        {
            try
            {
                if(part != null)
                {
                    InvoiceLineView invoiceLine = new();
                    invoiceLine.PartID = part.PartID;
                    invoiceLine.Description = part.Description;
                    invoiceLine.Price = part.Price;
                    invoiceLine.Taxable = part.Taxable;
                    invoiceLine.Quantity = 1;
                    invoice.InvoiceLines.Add(invoiceLine);
                    UpdateSubtotalAndTax();
                }
            }
            catch(ArgumentNullException ex)
            {
                errorMessage = BlazorHelperClass.GetInnerException(ex).Message;
            }
            catch(AggregateException ex)
            {
                if(!string.IsNullOrWhiteSpace(errorMessage))
                {
                    errorMessage = $"{errorMessage}{Environment.NewLine}";
                }
                //Debug here
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
        private async Task DeleteInvoiceLine(InvoiceLineView invoiceLine)
        {
            //Show a simple dialogue
            // Must have a title, the dialogue text, yesText: Our text, cancelText: Out text
            // Yes results in true
            // Cancel results in Null
            // No Text results in false
            // this call must be used async, so any method calling it must be async Task
                // async Task is the async equvilent of void
            // We should make the results datatype bool? to ensure it is nullable.
            bool? results = await DialogService.ShowMessageBox("Confirm Delete", $"Are you sure that you wish to remove {invoiceLine.Description}?", yesText: "Remove", noText: "Nope", cancelText: "Cancel");

            if(results == true)
            {
                invoice.InvoiceLines.Remove(invoiceLine);
                UpdateSubtotalAndTax();
            }
        }
        private void SyncPrice(InvoiceLineView line)
        {
            //Find the original price of the Part from the database
            try
            {
                PartView? part = PartService.GetPart(line.PartID);
                if(part != null)
                {
                    line.Price = part.Price;
                    UpdateSubtotalAndTax();
                }
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
                errorMessage = $"{errorMessage}Unable to search for part";
                foreach (var error in ex.InnerExceptions)
                {
                    errorDetails.Add(error.Message);
                }
            }
        }
        private void QuantityEdited(InvoiceLineView lineView, int newQuantity)
        {
            lineView.Quantity = newQuantity;
            UpdateSubtotalAndTax();
        }
        private void PriceEdited(InvoiceLineView lineView, decimal newPrice)
        {
            lineView.Price = newPrice;
            UpdateSubtotalAndTax();
        }
        private void UpdateSubtotalAndTax()
        {
            invoice.SubTotal = invoice.InvoiceLines
                .Where(x => !x.RemoveFromViewFlag)
                .Sum(x => x.Quantity * x.Price);
            invoice.Tax = invoice.InvoiceLines
                .Where(x => !x.RemoveFromViewFlag)
                .Sum(x => x.Taxable ? x.Quantity * x.Price * 0.05m : 0);
        }
        private void SaveInvoice()
        {
            try
            {
                
                //reset errors and feedback
                errorDetails.Clear();
                errorMessage = string.Empty;
                feedbackMessage = string.Empty;

                //check if new invoice
                bool isNewInvoice = invoice.InvoiceID == 0;
                invoice = InvoiceService.AddOrEditInvoice(invoice);
                InvoiceID = invoice.InvoiceID;
                feedbackMessage = isNewInvoice
                    ? $"New Invoice No {InvoiceID} was created."
                    : $"Invoice No {InvoiceID} was updated.";
            }
            catch(ArgumentNullException ex)
            {
                errorMessage = BlazorHelperClass.GetInnerException(ex).Message;
            }
            catch(AggregateException ex)
            {
                if(!string.IsNullOrWhiteSpace(errorMessage))
                {
                    errorMessage = $"{errorMessage}{Environment.NewLine}";
                }
                //Debug here
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
        private async Task Close()
        {
            bool? result = await DialogService.ShowMessageBox("Confirm Close", "Are you sure you want to close the invoice editor? All unsaved changes will be lost.", yesText: "Yes", cancelText: "Cancel");

            if (result == true)
                NavigationManager.NavigateTo($"/SamplePages/CustomerEdit/{CustomerID}");
        }
        #endregion
    }
}
