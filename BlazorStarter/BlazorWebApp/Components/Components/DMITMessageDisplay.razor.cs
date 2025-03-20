using Microsoft.AspNetCore.Components;

namespace BlazorWebApp.Components.Components
{
    public partial class DMITMessageDisplay
    {
        #region Parameters
        [Parameter]
        public List<string> ErrorMsgs { get; set; } = [];
        [Parameter]
        public string ErrorMessage { get; set; } = string.Empty;
        [Parameter]
        public string Feedback { get; set; } = string.Empty;
        #endregion

        #region Fields
        // has feedback
        //private bool hasFeedback()
        //{
        //    return !string.IsNullOrWhiteSpace(Feedback);
        //}

        //Lamda syntax for a simple method, this is the same as the commented out method above.
        //Can only use this if there is no other logic and you just have a return.
        private bool hasFeedback => !string.IsNullOrWhiteSpace(Feedback);

        // has error
        private bool hasSingleError => !string.IsNullOrWhiteSpace(ErrorMessage);

        //Multiple Errors or one message
        private bool hasMultipleErrors => ErrorMsgs.Count > 0;
        #endregion
    }
}
