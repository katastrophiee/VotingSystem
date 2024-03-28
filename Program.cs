using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VotingSystem.API.Interfaces.Provider;
using VotingSystem.API.Providers;
using VotingSystem.API.Repository.DBContext;
using VotingSystem.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 5;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.SignIn.RequireConfirmedEmail = false;
})
    .AddEntityFrameworkStores<DBContext>();

//Controller dependency injection
builder.Services.AddControllers();

//Provider dependency injection
builder.Services.AddScoped<IAuthProvider, AuthProvider>();
builder.Services.AddScoped<ICustomerProvider, CustomerProvider>();

//Repository dependency injection
builder.Services.AddDbContext<DBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("VotingSystem")));

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
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});


app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
