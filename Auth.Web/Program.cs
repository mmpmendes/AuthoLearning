using Auth.Web.Components;
using Auth.Web.Services;

using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

IEnumerable<string>? initialScopes = builder.Configuration["DownstreamApi:Scopes"]?.Split(' ');

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.AddRedisOutputCache("cache");

builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, "AzureAd")
                .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
                .AddDownstreamApi("DownstreamApi"
                                , builder.Configuration.GetSection("DownstreamApi"))
                .AddInMemoryTokenCaches();

// Add services to the container.
builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddMicrosoftIdentityConsentHandler();

builder.Services.AddHttpClient<TestApiService>("Test", client =>
    {
        client.BaseAddress = new("https+http://apiservice");
    });

builder.Services.AddScoped<TestApiService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

//app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseOutputCache();

app.MapStaticAssets();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
