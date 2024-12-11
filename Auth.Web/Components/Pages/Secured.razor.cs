using Auth.Web.Services;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Auth.Web.Components.Pages
{
    public partial class Secured(IJSRuntime jsRuntime, TestApiService apiService)
    {
        private string UserName;
        private string response;

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            UserName = user.Identity.IsAuthenticated ? user.Identity.Name : "Guest";
        }

        private async Task DoApiRequest(MouseEventArgs e)
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (!user.Identity.IsAuthenticated)
            {
                // Redirect or prompt the user to sign in
                response = "User is not authenticated.";
            }
            else
            {
                response = await apiService.GetSomeNewDataAsync();
            }
        }
    }
}
