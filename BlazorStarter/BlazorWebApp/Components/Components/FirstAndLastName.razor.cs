using Microsoft.AspNetCore.Components;

namespace BlazorWebApp.Components.Components
{
    public partial class FirstAndLastName
    {
        #region Parameters
        
        [Parameter]
        public string FirstName { get; set; } = string.Empty;
        //Add an EventCallback to notify the parent component when the first name changes
        //Must be named the exact same as the parameter with the worth Changed as a suffix
        //This needs the Parameter attribute to be recognized as a parameter
        [Parameter]
        public EventCallback<string> FirstNameChanged { get; set; } = default!;
        [Parameter]
        public string LastName { get; set; } = string.Empty;
        [Parameter]
        public EventCallback<string> LastNameChanged { get; set; } = default!;
        #endregion

        #region Methods
        //This method is called when the first name changes
        private void OnLastNameChanged(string newValue)
        {
            //Set the new LastName Value
            LastName = newValue;
            //Invoke the EventCallback to notify the parent component
            LastNameChanged.InvokeAsync(LastName);
        }
        private void OnFirstNameChanged(string newValue)
        {
            //Set the new FirstName Value
            FirstName = newValue;
            //Invoke the EventCallback to notify the parent component
            FirstNameChanged.InvokeAsync(FirstName);
        }
        #endregion
    }
}
