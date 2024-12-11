namespace Auth.Web.Services
{
    public class TestApiService(HttpClient httpClient)
    {
        public string GetSomeDataAsync(CancellationToken cancellationToken = default)
        {
            var response = httpClient.GetAsync("api/Test");
            return response.Result.ToString();
        }
    }
}
