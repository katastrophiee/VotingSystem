using Blazored.LocalStorage;
using Microsoft.EntityFrameworkCore;
using VotingSystem.API.Interfaces.Provider;
using VotingSystem.API.Providers;
using VotingSystem.API.Repository.DBContext;
using VotingSystem.Components;
using VotingSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//Controller dependency injection
builder.Services.AddControllers();

//Provider dependency injection
builder.Services.AddScoped<IAuthProvider, AuthProvider>();
builder.Services.AddScoped<ICustomerProvider, CustomerProvider>();
builder.Services.AddScoped<IAdminProvider, AdminProvider>();

//Repository dependency injection
builder.Services.AddDbContext<DBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("VotingSystem")));

builder.Services.AddHttpClient<IApiRequestService, ApiRequestService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:44389/api/");
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
app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
