using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlazorWebApp.Components.Components.Dialogues
{
    public partial class DMITSimpleDialogue
    {
        #region Fields
        private string feedbackText = string.Empty;
        #endregion
        #region Parameters
        //We need to include a cascading parameter in order to reference it
        // to pass results back or cancel the dialogue
        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; } = default!;
        [Parameter]
        public string ButtonText { get; set; } = string.Empty;
        [Parameter]
        public Color Color { get; set; } = Color.Primary;
        #endregion
        #region Methods
        private void Cancel() => MudDialog.Cancel();

        //When the user submits or confirms the dialogue
        //We can tell the calling page the result was ok
        //and pass back to the caller the feedbackText field data.
        private void Submit() => MudDialog.Close(DialogResult.Ok(feedbackText));
        #endregion
    }
}
