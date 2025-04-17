using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Text.Json;

namespace BlazorWebApp.Components.Pages.SamplePages
{
    public partial class FileExample
    {
        private IBrowserFile? browserFile;
        private string picture = string.Empty;
        private IJSObjectReference module;
        private ImageDimensions dimensions = new();
        private string timezone;

        //Must Inject JSRuntime to use JSInterop, which allows us to use JavaScript functions in Blazor.
        [Inject]
        IJSRuntime JSRuntime { get; set; } = default!;

        //Can use this OnAfterRender to load JS files
        //This is triggered after all elements are already rendered (loaded) on the page.
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                //binding the dmitscripts and the functions inside to the module variable
                //Now we can use the functions inside the dmitscripts.js file
                module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/dmitscripts.js");
            }
        }
        private async Task UploadFileChange(InputFileChangeEventArgs e)
        {
            var file = e.File;
            //A reference to the file stream that can be used to read the file.
            var streamRef = new DotNetStreamReference(file.OpenReadStream(file.Size));
            //A buffer to hold the file data.
            var buffers = new byte[file.Size];
            //Once the file is read, we can use the resulting bytes to save it to a database or file system.
            await file.OpenReadStream().ReadAsync(buffers);
            //Pass in the file stream to the JavaScript function
            var json = await module.InvokeAsync<string>("getImageDimensions", streamRef);
            //The json string is returned from the JavaScript function
            dimensions = JsonSerializer.Deserialize<ImageDimensions>(json) ?? new();
            //Looking at the browser to get the users timezone offset
            //Just another example of using JSInterop
            timezone = await module.InvokeAsync<string>("blazorGetTimezone");
        }
    }

    public class ImageDimensions
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
