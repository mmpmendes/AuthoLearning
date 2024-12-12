using Microsoft.Identity.Client;
using Microsoft.Identity.Web;

using System.Net.Http.Headers;

namespace Auth.Web.Services
{
    public class TestApiService(HttpClient httpClient, ITokenAcquisition tokenAcquisition, IConfiguration configuration)
    {
        public async Task<string> GetSomeDataAsync(CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetAsync("api/Test", cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetSomeNewDataAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                IEnumerable<string>? initialScopes = configuration["AzureAd:Scopes"]?.Split(' ');

                // Acquire the token
                var accessToken = await tokenAcquisition.GetAccessTokenForUserAsync(initialScopes);

                // Add the token to the request
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // Call the API
                var response = await httpClient.GetAsync("api/Test", cancellationToken);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (MsalUiRequiredException)
            {
                throw;
            }
        }
    }
}
