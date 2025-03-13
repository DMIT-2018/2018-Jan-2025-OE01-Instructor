using HogWildSystem.BLL;
using HogWildSystem.ViewModels;
using Microsoft.AspNetCore.Components;

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
        #endregion

        #region Methods
        protected override void OnInitialized()
        {
            try
            {
                invoice = InvoiceService.GetInvoice(InvoiceID, CustomerID, EmployeeID);
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
        #endregion
    }
}
