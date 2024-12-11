using Auth.Web.Components;
using Auth.Web.Services;

using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.AddRedisOutputCache("cache");

//builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
//    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

IEnumerable<string>? initialScopes = builder.Configuration["DownstreamApi:Scopes"]?.Split(' ');
builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, "AzureAd")
                .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
//.AddMicrosoftGraph(builder.Configuration.GetSection("DownstreamApi"))
//.AddDownstreamApi("DownstreamApi", builder.Configuration.GetSection("DownstreamApi"))
                .AddInMemoryTokenCaches();

// Add services to the container.
builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddMicrosoftIdentityConsentHandler();

builder.Services.AddHttpClient<TestApiService>("Test", client =>
    {
        client.BaseAddress = new("https+http://apiservice");
    });
//    .AddHttpMessageHandler<MicrosoftIdentityAppAuthenticationMessageHandler>();
//builder.Services.AddTransient<MicrosoftIdentityAppAuthenticationMessageHandler>();

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
