using Auth.Web.Components;
using Auth.Web.Services;

using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.AddRedisOutputCache("cache");



IEnumerable<string>? initialScopes = builder.Configuration["AzureAd:Scopes"]?.Split(' ');
builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, "AzureAd")
                .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
                //.AddMicrosoftGraph(builder.Configuration.GetSection("DownstreamApi"))
                .AddDownstreamApi("DownstreamApi", builder.Configuration.GetSection("DownstreamApi"))
                .AddInMemoryTokenCaches();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

// Add services to the container.
builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddMicrosoftIdentityConsentHandler();

builder.Services.AddServerSideBlazor()
                .AddMicrosoftIdentityConsentHandler();

builder.Services.AddControllersWithViews()
                .AddMicrosoftIdentityUI();



builder.Services.AddHttpClient<TestApiService>("TestApiService", client =>
    {
        client.BaseAddress = new("https+http://apiservice");
    });

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
