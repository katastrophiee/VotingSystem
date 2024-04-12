using Blazored.LocalStorage;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using VotingSystem.API.Interfaces.Provider;
using VotingSystem.API.Providers;
using VotingSystem.API.Repository.DBContext;
using VotingSystem.Components;
using VotingSystem.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();
builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});

RequestLocalizationOptions GetLocalizationOptions()
{
    var cultures = builder.Configuration.GetSection("Cultures").GetChildren().ToDictionary(x => x.Key, x => x.Value);

    var supportedCultures = cultures.Keys.ToArray();

    var localizationOptions = new RequestLocalizationOptions()
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);

    return localizationOptions;
}

builder.Services.AddScoped<IAuthProvider, AuthProvider>();
builder.Services.AddScoped<ICustomerProvider, CustomerProvider>();
builder.Services.AddScoped<IAdminProvider, AdminProvider>();
builder.Services.AddScoped<IDocumentProvider, DocumentProvider>();
builder.Services.AddScoped<IElectionProvider, ElectionProvider>();
builder.Services.AddScoped<IVoteProvider, VoteProvider>();

builder.Services.AddDbContext<DBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("VotingSystem")));

builder.Services.AddHttpClient<IApiRequestService, ApiRequestService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:44389/api/");
    client.DefaultRequestHeaders.Add("Accept-Language", CultureInfo.CurrentCulture.Name);
});

builder.Services.AddBlazoredLocalStorage();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseAntiforgery();
app.UseRequestLocalization(GetLocalizationOptions());
app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
