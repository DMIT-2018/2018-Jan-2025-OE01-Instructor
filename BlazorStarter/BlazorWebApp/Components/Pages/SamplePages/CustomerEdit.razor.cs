using Microsoft.AspNetCore.Components;

namespace BlazorWebApp.Components.Pages.SamplePages
{
    public partial class CustomerEdit
    {
        #region Fields
        #endregion

        #region Properties
        [Parameter]
        public int CustomerID { get; set; }
        #endregion
    }
}
