using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Rentaunsedan.Web.Components.Shared
{
    public class JsPageBase : ComponentBase
    {
        [Inject] protected IJSRuntime JS { get; set; } = default!;

        protected bool CanUseJs { get; private set; }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
                CanUseJs = true;
        }

        protected async Task ShowAlert(string icon, string title, string text)
        {
            if (CanUseJs)
            {
                await JS.InvokeVoidAsync("Swal.fire", new { icon, title, text });
            }
        }
    }
}
