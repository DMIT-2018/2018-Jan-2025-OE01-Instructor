using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlazorWebApp.Components.Pages.SamplePages
{
    public partial class MudStepperEx
    {
        private string firstName = string.Empty;
        private string lastName = string.Empty;
        private string email = string.Empty;
        private string phoneNumber = string.Empty;

        [Inject]
        protected ISnackbar Snackbar { get; set; } = default!;
        #region Methods
        private async Task NavigateStepAsync(StepperInteractionEventArgs arg)
        {
            if (arg.Action == StepAction.Complete)
            {
                // occurrs when clicking next
                ControlStepCompletion(arg);
            }
            else if (arg.Action == StepAction.Activate)
            {
                // occurrs when clicking a step header with the mouse
                ControlStepNavigation(arg);
            }
        }
        private void ControlStepCompletion(StepperInteractionEventArgs arg)
        {
            switch (arg.StepIndex)
            {
                case 0:
                    if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                    {
                        Snackbar.Add("Please enter a first and last name.", Severity.Error);
                        arg.Cancel = true;
                    }
                    break;
                case 1:
                    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(phoneNumber))
                    {
                        Snackbar.Add("Please enter an email and phone number.", Severity.Error);
                        arg.Cancel = true;
                    }
                    break;
            }
        }
        private void ControlStepNavigation(StepperInteractionEventArgs arg)
        {
            switch (arg.StepIndex)
            {
                case 0:
                    if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                    {
                        Snackbar.Add("Please enter a first and last name.", Severity.Error);
                        arg.Cancel = true;
                    }
                    break;
                case 1:
                    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(phoneNumber))
                    {
                        Snackbar.Add("Please enter an email and phone number.", Severity.Error);
                        arg.Cancel = true;
                    }
                    break;

                    #endregion
            }
        }
    }
}
