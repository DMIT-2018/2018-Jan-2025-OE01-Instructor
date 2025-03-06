using HogWildSystem.BLL;
using HogWildSystem.ViewModels;
using Microsoft.AspNetCore.Components;

namespace BlazorWebApp.Components.Pages.SamplePages
{
    public partial class WorkingVersion
    {
        #region Fields
        private WorkingVersionView? workingVersionView = new();
        private string feedback = string.Empty;
        private List<string> errorMessages = [];
        #endregion

        #region Properties
        [Inject]
        protected WorkingVersionService WorkingVersionService { get; set; } = default!;
        #endregion

        #region Method
        private void GetWorkingVersion()
        {
            try
            {
                workingVersionView = WorkingVersionService.GetWorkingVersion();
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
