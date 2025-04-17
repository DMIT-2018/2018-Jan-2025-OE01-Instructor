using Microsoft.JSInterop;

namespace BlazorWebApp.Services
{
    internal class JsService: IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> _dmitModule;

        public JsService(IJSRuntime jsRuntime)
        {
            _dmitModule = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./js/dmitscripts.js").AsTask());
        }

        public async ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);

            if (_dmitModule.IsValueCreated)
            {
                var module = await _dmitModule.Value;
                await module.DisposeAsync();
            }
        }
    }
}
