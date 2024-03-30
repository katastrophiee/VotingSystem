using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Responses;
using VotingSystem.API.Interfaces.Provider;
using VotingSystem.API.Repository.DBContext;

namespace VotingSystem.API.Providers;

//https://learn.microsoft.com/en-us/aspnet/web-api/overview/security/individual-accounts-in-web-api
//https://learn.microsoft.com/en-us/aspnet/aspnet/overview/owin-and-katana/an-overview-of-project-katana

public class AuthProvider(
    DBContext dbContext) : IAuthProvider
{
    private readonly DBContext _dbContext = dbContext;

    //Move all to config
    private readonly string SecretKey = "Q3J5cHRvZ3JhcGhpY2FsbHlTZWN1cmVSYW5kb21TdHJpbmc=";
    private readonly string Issuer = "LocalVotingSystemApp_v1.0";
    private readonly string Audience = "LocalVotingSystem";
    //private readonly SignInManager<IdentityUser> _signInManager = signInManager;
    //private readonly UserManager<IdentityUser> _userManager = userManager;

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
                Description = $"An unknown error occured when trying to login",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    private async Task<Response<LoginResponse>> LoginAsCustomer(Customer customer, string password)
    {
        var passwordSalt = customer.PasswordSalt;
        var pbkdf2HashedPassword = Pbkdf2HashString(password, ref passwordSalt);

        //if (!string.Equals(pbkdf2HashedPassword, customer.Password))
        //{
        //    return new(new ErrorResponse()
        //    {
        //        Title = "Invalid Login Credentials",
        //        Description = $"The username or password is incorrect.",
        //        StatusCode = StatusCodes.Status400BadRequest,
        //    });
        //}

        if (!customer.IsActive)
            return new(new ErrorResponse()
            {
                Title = "Inactive Account",
                Description = $"Your account is not active",
                StatusCode = StatusCodes.Status401Unauthorized,
            });

        //if (!customer.IsVerified)
        //    return new(new ErrorResponse()
        //    {
        //        Title = "Account Not Verified",
        //        Description = $"Your account has not been verified yet.",
        //        StatusCode = StatusCodes.Status403Forbidden,
        //    });

        customer.LastLoggedIn = DateTime.UtcNow;

        var accessToken = GenerateAccessToken(customer);

        //var result = await _signInManager.PasswordSignInAsync(customer.Username, pbkdf2HashedPassword, false, false);

        //if (result.Succeeded)
        //{
            //localStorage.setItem("authToken", yourToken);
            //LocalRedirect("~/");
            return new(new LoginResponse()
            {
                UserId = customer.Id,
                //AccessToken = accessToken,
                //ExpiresIn = 30,
            });
        //}
        //else
        //{
        //    return new(new ErrorResponse()
        //    {
        //        Title = "Invalid Login Credentials",
        //        Description = $"The username or password is incorrect.",
        //        StatusCode = StatusCodes.Status400BadRequest,
        //    });
        //}
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

    private string GenerateAccessToken(Customer customer)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, customer.Id.ToString()),
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


    public async Task<Response<LoginResponse>> CreateAccount(CreateAccountRequest request)
    {
        //Make salt
        //passwordSalt = string.Empty;
        //pbkdf2HashedPassword = Pbkdf2HashString(password, ref passwordSalt);

        //member.Password = pbkdf2HashedPassword;
        //member.PasswordSalt = passwordSalt;
        //_memberRepository.Update(member);
        //await _memberRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        //var identity = new IdentityUser { UserName = request.Username, Email = request.Email };

        return null;
    }
}
