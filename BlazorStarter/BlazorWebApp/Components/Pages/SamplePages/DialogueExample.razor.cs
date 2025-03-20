using BlazorWebApp.Components.Components.Dialogues;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlazorWebApp.Components.Pages.SamplePages
{
    public partial class DialogueExample
    {
        #region Fields
        private string buttonText = string.Empty;
        private Color? buttonColor = Color.Primary;
        private string feedbackText = string.Empty;
        #endregion

        #region Parameters
        [Inject]
        protected IDialogService DialogService { get; set; } = default!;
        #endregion

        #region Method
        private async Task CustomDialogue()
        {
            //reset to the feedback
            feedbackText = string.Empty;
            if(string.IsNullOrWhiteSpace(buttonText) || !buttonColor.HasValue)
            {
                feedbackText = "Please enter button text and select a colour to show the dialogue.";
                return;
            }

            //For custom dialogues with parameters, you need to make a DialogueParameters instance
            //to pass to the dialogue
            var parameters = new DialogParameters<DMITSimpleDialogue>
            {
                { x => x.ButtonText, buttonText },
                { x => x.Color, buttonColor.Value }
            };

            //Can also set options for the specific dialogue
            DialogOptions options = new()
            {
                CloseButton = false,
                Position = DialogPosition.Center,
                BackdropClick = false
            };

            //Create the dialogue and give it a title, the parameters, and options (optionally)
            var dialogue = await DialogService.ShowAsync<DMITSimpleDialogue>("Custom Dialogue", parameters, options);
            //await the results
            //waits for the thread and stops this function until the dialogue is canceled or submitted)
            var results = await dialogue.Result;

            if (results != null && !results.Canceled)
                feedbackText = results.Data?.ToString() ?? string.Empty;
        }
        #endregion
    }
}
