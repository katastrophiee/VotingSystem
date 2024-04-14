using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;
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
builder.Services.AddScoped<IVoterProvider, VoterProvider>();
builder.Services.AddScoped<IAdminProvider, AdminProvider>();
builder.Services.AddScoped<IDocumentProvider, DocumentProvider>();
builder.Services.AddScoped<IElectionProvider, ElectionProvider>();
builder.Services.AddScoped<IVoteProvider, VoteProvider>();

builder.Services.AddDbContext<DBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("VotingSystem")));

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "LocalVotingSystemApp_v1.0",
            ValidateAudience = true,
            ValidAudience = "LocalVotingSystem",
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Q3J5cHRvZ3JhcGhpY2FsbHlTZWN1cmVSYW5kb21TdHJpbmc=")),
            ValidateIssuerSigningKey = true,
        };
    });

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
app.UseAuthentication();
app.UseAuthorization();
app.UseRequestLocalization(GetLocalizationOptions());
app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
