using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.DbModels.Admin;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Requests.Admin;
using VotingSystem.API.DTO.Responses;
using VotingSystem.API.Enums;
using VotingSystem.API.Interfaces.Provider;
using VotingSystem.API.Repository.DBContext;

namespace VotingSystem.API.Providers;

//https://learn.microsoft.com/en-us/aspnet/web-api/overview/security/individual-accounts-in-web-api
//https://learn.microsoft.com/en-us/aspnet/aspnet/overview/owin-and-katana/an-overview-of-project-katana

public class AuthProvider(DBContext dbContext) : IAuthProvider
{
    private readonly DBContext _dbContext = dbContext;

    //Move all to config
    private readonly string SecretKey = "Q3J5cHRvZ3JhcGhpY2FsbHlTZWN1cmVSYW5kb21TdHJpbmc=";
    private readonly string Issuer = "LocalVotingSystemApp_v1.0";
    private readonly string Audience = "LocalVotingSystem";

    public async Task<Response<LoginResponse>> CustomerLogin(LoginRequest request)
    {
        try
        {
            var customer = await _dbContext.Customer.FirstOrDefaultAsync(c => c.Username == request.Username);

            if (customer is null)
                return new(new ErrorResponse()
                {
                    Title = "Invalid Login Credentials",
                    Description = $"The username or password is incorrect.",
                    StatusCode = StatusCodes.Status400BadRequest,
                });

            return await LoginAsCustomer(customer, request.Password);
        }
        catch (Exception ex) 
        {
            return new(new ErrorResponse()
            {
                Title = "Internal Server Error",
                Description = $"An unknown error occured when trying to login for user {request.Username}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    private async Task<Response<LoginResponse>> LoginAsCustomer(Customer customer, string password)
    {
        var passwordSalt = customer.PasswordSalt;
        var pbkdf2HashedPassword = Pbkdf2HashString(password, ref passwordSalt);

        if (!string.Equals(pbkdf2HashedPassword, customer.Password))
        {
            return new(new ErrorResponse()
            {
                Title = "Invalid Login Credentials",
                Description = $"The username or password is incorrect.",
                StatusCode = StatusCodes.Status400BadRequest,
            });
        }

        if (!customer.IsActive)
            return new(new ErrorResponse()
            {
                Title = "Inactive Account",
                Description = $"Your account is not active",
                StatusCode = StatusCodes.Status401Unauthorized,
            });


        customer.LastLoggedIn = DateTime.UtcNow;

        _dbContext.Customer.Update(customer);
        await _dbContext.SaveChangesAsync();

        var accessToken = GenerateAccessToken(customer.Id);

        return new(new LoginResponse()
        {
            UserId = customer.Id,
            AccessToken = accessToken,
            ExpiresIn = 30,
        });
    }

    public async Task<Response<LoginResponse>> CreateCustomerAccount(CreateCustomerAccountRequest request)
    {
        try
        {
            var existingCustomers = await _dbContext.Customer
              .Where(v => v.Username == request.Username || v.Email == request.Email)
              .ToListAsync();
            
            if (existingCustomers.Count != 0)
            {
                return new(new ErrorResponse()
                {
                    Title = "Credentials Already Used",
                    Description = $"The email or username is already in use",
                    StatusCode = StatusCodes.Status400BadRequest,
                });
            }

            var passwordSalt = GenerateSalt();
            var pbkdf2HashedPassword = Pbkdf2HashString(request.Password, ref passwordSalt);

            var newCustomer = new Customer()
            {
                Username = request.Username,
                Email = request.Email,
                Password = pbkdf2HashedPassword,
                PasswordSalt = passwordSalt,
                FirstName = "",
                LastName = "",
                Country = CustomerCountry.Unknown,
                NewUser = true,
                IsCandidate = false,
                IsActive = true,
                IsVerified = false,
                LastLoggedIn = DateTime.UtcNow,
            };

            _dbContext.Customer.Add(newCustomer);
            await _dbContext.SaveChangesAsync();

            var customer = await _dbContext.Customer.FirstOrDefaultAsync(c => c.Username == request.Username);

            var accessToken = GenerateAccessToken(customer.Id);

            return new(new LoginResponse()
            {
                UserId = customer.Id,
                AccessToken = accessToken,
                ExpiresIn = 30
            });
        }
        catch (Exception ex) 
        {
            return new(new ErrorResponse()
            {
                Title = "Internal Server Error",
                Description = $"An unknown error occured when trying to create account for user {request.Username}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    private static string Pbkdf2HashString(string password, ref string salt)
    {
        const int SaltSize = 128 / 8;

        if (string.IsNullOrEmpty(salt))
        {
            var newSalt = new byte[SaltSize];
            RandomNumberGenerator.Fill(newSalt);

            salt = Encoding.UTF8.GetString(newSalt);
        }

        var saltBytes = Encoding.UTF8.GetBytes(salt);
        var key = KeyDerivation.Pbkdf2(password, saltBytes, KeyDerivationPrf.HMACSHA256, 100000, SaltSize);
        return Convert.ToBase64String(key);
    }

    private string GenerateAccessToken(int userId)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string GenerateSalt(int size = 32)
    {
        var buff = new byte[size];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(buff);

        return Convert.ToBase64String(buff);
    }

    public async Task<Response<LoginResponse>> AdminLogin(LoginRequest request)
    {
        try
        {
            var admin = await _dbContext.Admin.FirstOrDefaultAsync(c => c.Username == request.Username);

            if (admin is null)
                return new(new ErrorResponse()
                {
                    Title = "Invalid Login Credentials",
                    Description = $"The username or password is incorrect.",
                    StatusCode = StatusCodes.Status400BadRequest,
                });

            return await LoginAsAdmin(admin, request.Password);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = "Internal Server Error",
                Description = $"An unknown error occured when trying to login for admin {request.Username}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    private async Task<Response<LoginResponse>> LoginAsAdmin(Admin admin, string password)
    {
        var passwordSalt = admin.PasswordSalt;
        var pbkdf2HashedPassword = Pbkdf2HashString(password, ref passwordSalt);

        if (!string.Equals(pbkdf2HashedPassword, admin.Password))
        {
            return new(new ErrorResponse()
            {
                Title = "Invalid Login Credentials",
                Description = $"The username or password is incorrect.",
                StatusCode = StatusCodes.Status400BadRequest,
            });
        }

        admin.LastLoggedIn = DateTime.UtcNow;

        _dbContext.Admin.Update(admin);
        await _dbContext.SaveChangesAsync();

        var accessToken = GenerateAccessToken(admin.Id);

        return new(new LoginResponse()
        {
            UserId = admin.Id,
            AccessToken = accessToken,
            ExpiresIn = 30,
        });
    }

    public async Task<Response<LoginResponse>> CreateAdminAccount(CreateAdminAccountRequest request)
    {
        try
        {
            var existingAdmin = await _dbContext.Admin
              .Where(v => v.Username == request.Username || v.Email == request.Email)
              .ToListAsync();

            if (existingAdmin.Count != 0)
            {
                return new(new ErrorResponse()
                {
                    Title = "Credentials Already Used",
                    Description = $"The email or username is already in use",
                    StatusCode = StatusCodes.Status400BadRequest,
                });
            }

            var passwordSalt = GenerateSalt();
            var pbkdf2HashedPassword = Pbkdf2HashString(request.Password, ref passwordSalt);

            var newAdmin = new Admin()
            {
                Username = request.Username,
                Email = request.Email,
                Password = pbkdf2HashedPassword,
                PasswordSalt = passwordSalt,
                DisplayName = request.DisplayName,
                Country = request.Country,
                IsActive = true,
                LastLoggedIn = DateTime.UtcNow,
            };

            _dbContext.Admin.Add(newAdmin);
            await _dbContext.SaveChangesAsync();

            var admin = await _dbContext.Admin.FirstOrDefaultAsync(c => c.Username == request.Username);

            var accessToken = GenerateAccessToken(admin.Id);

            return new(new LoginResponse()
            {
                UserId = admin.Id,
                AccessToken = accessToken,
                ExpiresIn = 30
            });
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = "Internal Server Error",
                Description = $"An unknown error occured when trying to create account for admin {request.Username}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }
}
