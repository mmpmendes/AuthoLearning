using Auth.Web.Services;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Auth.Web.Components.Pages
{
    public partial class Secured(TestApiService apiService
        , NavigationManager navigationManager)
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
                try
                {
                    response = await apiService.GetSomeNewDataAsync();
                    // ... process the data
                }
                catch (Exception)
                {
                    // User interaction required, redirect to Azure AD login
                    navigationManager.NavigateTo("https://login.microsoftonline.com/c5e7a4fd-a735-4a8e-a61f-8331bf752791/oauth2/v2.0/authorize?response_type=code&client_id=e05a913a-dce6-4f63-bc21-ce71ab7cc8f3&redirect_uri=https://localhost:5443/signin&scope=user.read%20openid%20offline_access&response_mode=form_post&state=somestate");
                }
                //catch (Exception ex)
                //{
                //    // Handle other exceptions
                //    response = ex.Message;
                //}
            }
        }
    }
}
