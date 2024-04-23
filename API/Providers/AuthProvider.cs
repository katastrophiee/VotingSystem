using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
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

public class AuthProvider(DBContext dbContext, IStringLocalizer<AuthProvider> localizer, IConfigurationSection jwtValues) : IAuthProvider
{
    private readonly DBContext _dbContext = dbContext;
    private readonly IStringLocalizer<AuthProvider> _localizer = localizer;
    private readonly IConfigurationSection _jwtValues = jwtValues;

    public async Task<Response<LoginResponse>> VoterLogin(LoginRequest request)
    {
        try
        {
            var voter = await _dbContext.Voter.FirstOrDefaultAsync(c => c.Username == request.Username);

            if (voter is null)
                return new(new ErrorResponse()
                {
                    Title = _localizer["InvalidLoginCredentials"],
                    Description = _localizer["UsernameOrPasswordIncorrect"],
                    StatusCode = StatusCodes.Status400BadRequest,
                });

            return await LoginAsVoter(voter, request.Password);
        }
        catch (Exception ex) 
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorVoterLogin"]} {request.Username}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    private async Task<Response<LoginResponse>> LoginAsVoter(Voter voter, string password)
    {
        var passwordSalt = voter.PasswordSalt;
        var pbkdf2HashedPassword = password.Pbkdf2HashString(ref passwordSalt);

        if (!string.Equals(pbkdf2HashedPassword, voter.Password))
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InvalidLoginCredentials"],
                Description = _localizer["UsernameOrPasswordIncorrect"],
                StatusCode = StatusCodes.Status400BadRequest,
            });
        }

        if (!voter.IsActive)
            return new(new ErrorResponse()
            {
                Title = _localizer["InactiveAccount"],
                Description = _localizer["AccountNotActive"],
                StatusCode = StatusCodes.Status401Unauthorized,
            });

        voter.LastLoggedIn = DateTime.UtcNow;

        _dbContext.Voter.Update(voter);
        await _dbContext.SaveChangesAsync();

        var accessToken = await GenerateAccessToken(voter.Id, false);

        return new(new LoginResponse()
        {
            UserId = voter.Id,
            AccessToken = accessToken,
            ExpiresIn = 30,
        });
    }

    public async Task<Response<LoginResponse>> CreateVoterAccount(CreateVoterAccountRequest request)
    {
        try
        {
            var existingVoters = await _dbContext.Voter
              .Where(v => v.Username == request.Username || v.Email == request.Email)
              .ToListAsync();
            
            if (existingVoters.Count != 0)
            {
                return new(new ErrorResponse()
                {
                    Title = _localizer["CredentialsAlreadyUsed"],
                    Description = _localizer["EmailOrUsernameAlreadyUsed"],
                    StatusCode = StatusCodes.Status400BadRequest,
                });
            }

            var passwordSalt = GenerateSalt();
            var pbkdf2HashedPassword = request.Password.Pbkdf2HashString(ref passwordSalt);

            var newVoter = new Voter()
            {
                Username = request.Username,
                Email = request.Email,
                Password = pbkdf2HashedPassword,
                PasswordSalt = passwordSalt,
                FirstName = "",
                LastName = "",
                Country = UserCountry.Unknown,
                NewUser = true,
                IsCandidate = false,
                IsActive = true,
                IsVerified = false,
                LastLoggedIn = DateTime.UtcNow,
            };

            _dbContext.Voter.Add(newVoter);
            await _dbContext.SaveChangesAsync();

            var voter = await _dbContext.Voter.FirstOrDefaultAsync(c => c.Username == request.Username);

            var roles = new UserRole()
            {
                UserId = voter.Id,
                RoleIds = [(int)Enums.Roles.Voter],
                IsAdmin = false
            };

            _dbContext.UserRole.Add(roles);
            await _dbContext.SaveChangesAsync();

            var accessToken = await GenerateAccessToken(voter.Id, false);

            return new(new LoginResponse()
            {
                UserId = voter.Id,
                AccessToken = accessToken,
                ExpiresIn = 30
            });
        }
        catch (Exception ex) 
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorCreateVoterAccount"]} {request.Username}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    private async Task<string> GenerateAccessToken(int userId, bool isAdmin)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var userRoles = await _dbContext.UserRole.Where(r => r.UserId == userId && r.IsAdmin == isAdmin).FirstOrDefaultAsync();

        foreach (var role in userRoles.RoleIds)
        {
            claims.Add(new Claim(ClaimTypes.Role, ((Enums.Roles)role).EnumDisplayName()));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtValues.GetValue<string>("Key") ?? ""));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtValues.GetValue<string>("Issuer"),
            audience: _jwtValues.GetValue<string>("Audience"),
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
                    Title = _localizer["InvalidLoginCredentials"],
                    Description = _localizer["UsernameOrPasswordIncorrect"],
                    StatusCode = StatusCodes.Status400BadRequest,
                });

            return await LoginAsAdmin(admin, request.Password);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorAdminLogin"]} {request.Username}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    private async Task<Response<LoginResponse>> LoginAsAdmin(Admin admin, string password)
    {
        var passwordSalt = admin.PasswordSalt;
        var pbkdf2HashedPassword = password.Pbkdf2HashString(ref passwordSalt);

        if (!string.Equals(pbkdf2HashedPassword, admin.Password))
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InvalidLoginCredentials"],
                Description = _localizer["UsernameOrPasswordIncorrect"],
                StatusCode = StatusCodes.Status400BadRequest,
            });
        }

        if (!admin.IsActive)
            return new(new ErrorResponse()
            {
                Title = _localizer["InactiveAccount"],
                Description = _localizer["AccountNotActive"],
                StatusCode = StatusCodes.Status401Unauthorized,
            });

        admin.LastLoggedIn = DateTime.UtcNow;

        _dbContext.Admin.Update(admin);
        await _dbContext.SaveChangesAsync();

        var accessToken = await GenerateAccessToken(admin.Id, true);

        return new(new LoginResponse()
        {
            UserId = admin.Id,
            AccessToken = accessToken,
            ExpiresIn = 30,
        });
    }

    public async Task<Response<LoginResponse>> CreateAdminAccount(AdminCreateAdminAccountRequest request)
    {
        try
        {
            var requestingAdmin = await _dbContext.Admin.FirstOrDefaultAsync(c => c.Id == request.RequestingAdminId);
            if (requestingAdmin is null || requestingAdmin.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoAdminFound"],
                    Description = $"{_localizer["NoAdminFoundWithId"]} {request.RequestingAdminId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var existingAdmin = await _dbContext.Admin
              .Where(v => v.Username == request.Username || v.Email == request.Email)
              .ToListAsync() ?? [];

            if (existingAdmin.Count != 0)
            {
                return new(new ErrorResponse()
                {
                    Title = _localizer["CredentialsAlreadyUsed"],
                    Description = _localizer["EmailOrUsernameAlreadyUsed"],
                    StatusCode = StatusCodes.Status400BadRequest,
                });
            }

            var passwordSalt = GenerateSalt();
            var pbkdf2HashedPassword = request.Password.Pbkdf2HashString(ref passwordSalt);

            var newAdmin = new Admin()
            {
                Username = request.Username,
                Email = request.Email,
                Password = pbkdf2HashedPassword,
                PasswordSalt = passwordSalt,
                DisplayName = request.DisplayName,
                Country = request.Country,
                IsActive = request.IsActive,
                LastLoggedIn = null,
            };

            _dbContext.Admin.Add(newAdmin);
            await _dbContext.SaveChangesAsync();

            var admin = await _dbContext.Admin.FirstOrDefaultAsync(c => c.Username == request.Username);
            if (admin is null || admin.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["ErrorWhenAddingAdmin"],
                    Description = $"{_localizer["ErrowWhenAddingAdminWithId"]}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var roles = new UserRole()
            {
                UserId = admin.Id,
                RoleIds = [(int)Enums.Roles.Admin],
                IsAdmin = true
            };

            _dbContext.UserRole.Add(roles);
            await _dbContext.SaveChangesAsync();

            var accessToken = await GenerateAccessToken(admin.Id, true);

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
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorCreateAdminAccount"]} {request.Username}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    } 

    public async Task<Response<LoginResponse>> UpdatePassword(UpdatePasswordRequest request)
    {
        try
        {
            var voter = await _dbContext.Voter.FirstOrDefaultAsync(c => c.Username == request.Username && c.Email == request.Email);
            if (voter is null)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoVoterFound"],
                    Description = $"{_localizer["NoVoterFoundWithUsername"]} {request.Username} {_localizer["AndEmail"]} {request.Email}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            if (request.Password != request.ConfirmPassword)
                return new(new ErrorResponse()
                {
                    Title = _localizer["PasswordsDoNotMatch"],
                    Description = _localizer["PasswordsDoNotMatchDescription"],
                    StatusCode = StatusCodes.Status400BadRequest
                });

            var passwordSalt = GenerateSalt();
            var pbkdf2HashedPassword = request.Password.Pbkdf2HashString(ref passwordSalt);
            
            voter.PasswordSalt = passwordSalt;
            voter.Password = pbkdf2HashedPassword;

            _dbContext.Voter.Update(voter);
            await _dbContext.SaveChangesAsync();

            return new(new LoginResponse()
            {
                UserId = voter.Id,
                AccessToken = await GenerateAccessToken(voter.Id, false),
                ExpiresIn = 30
            });
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorUpdatePassword"]} {request.Username}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }
}
