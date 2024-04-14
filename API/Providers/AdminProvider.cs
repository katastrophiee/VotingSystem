using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests.Admin;
using VotingSystem.API.DTO.Responses;
using VotingSystem.API.DTO.Responses.Admin;
using VotingSystem.API.Interfaces.Provider;
using VotingSystem.API.Repository.DBContext;

namespace VotingSystem.API.Providers;

public class AdminProvider(DBContext dbContext, IStringLocalizer<AdminProvider> localizer) : IAdminProvider
{
    private readonly DBContext _dbContext = dbContext;
    private readonly IStringLocalizer<AdminProvider> _localizer = localizer;

    public async Task<Response<List<AdminGetVotersResponse>>> GetVoters(AdminGetVotersRequest request)
    {
        try
        {
            var admin = await _dbContext.Admin.FirstOrDefaultAsync(c => c.Id == request.AdminId);
            if (admin is null || admin.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoAdminFound"],
                    Description = $"{_localizer["NoAdminFoundWithId"]} {request.AdminId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var voters = await _dbContext.Voter.Where(c => 
                (request.VoterId == null || c.Id == request.VoterId) &&
                (request.Country == null || c.Country == request.Country) &&
                (request.IsCandidate == null || c.IsCandidate == request.IsCandidate) &&
                (request.IsVerified == null || c.IsVerified == request.IsVerified))
               .ToListAsync();

            var response = new List<AdminGetVotersResponse>();
            voters?.ForEach(voter => response.Add(new(voter)));

            return new(response);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorGetVoters"]} {request.AdminId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<AdminGetVoterResponse>> GetVoterDetails(int voterId, int adminId)
    {
        try
        {
            var admin = await _dbContext.Admin.FirstOrDefaultAsync(c => c.Id == adminId);
            if (admin is null || admin.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoAdminFound"],
                    Description = $"{_localizer["NoAdminFoundWithId"]} {adminId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var voter = await _dbContext.Voter.FirstOrDefaultAsync(c => c.Id == voterId);
            if (voter is null || voter.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoAdminFound"],
                    Description = $"{_localizer["NoAdminFoundWithId"]} {adminId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var response = new AdminGetVoterResponse(voter);

            var currentIdDocument = await _dbContext.Document.FirstOrDefaultAsync(c => c.VoterId == voterId && c.MostRecentId == true);
            if (currentIdDocument != null)
                response.CurrentIdDocument = currentIdDocument;

            return new(response);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorGetVoterDetails"]} {adminId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<bool>> AdminVerifyId(AdminVerifyIdRequest request)
    {
        try
        {
            var admin = await _dbContext.Admin.FirstOrDefaultAsync(c => c.Id == request.AdminId);
            if (admin is null || admin.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoAdminFound"],
                    Description = $"{_localizer["NoAdminFoundWithId"]} {request.AdminId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var document = await _dbContext.Document.FirstOrDefaultAsync(d => d.Id == request.DocumentId);

            if (document is null)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoDocumentFound"],
                    Description = $"{_localizer["NoDocumentFoundWithId"]} {request.DocumentId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            document.IdRejected = request.IsRejected;

            if (!request.IsRejected)
            {
                document.ExpiryDate = request.DocumentExpiryDate;

                var voter = await _dbContext.Voter.FirstOrDefaultAsync(c => c.Id == document.VoterId);
                if (voter is null || voter.Id == 0)
                    return new(new ErrorResponse()
                    {
                        Title = _localizer["NoVoterFound"],
                        Description = $"{_localizer["NoVoterFoundWithId"]} {document.VoterId}",
                        StatusCode = StatusCodes.Status404NotFound
                    });

                voter.IsVerified = true;
                _dbContext.Voter.Update(voter);
            }
            else
            {
                document.RejectionReason = request.RejectionReason;
                document.RejectedByAdminId = request.AdminId;
            }

            _dbContext.Document.Update(document);
            await _dbContext.SaveChangesAsync();

            return new(true);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorAdminVerifyId"]} {request.AdminId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<bool>> AddElection(AddElectionRequest request)
    {
        try
        {
            var admin = await _dbContext.Admin.FirstOrDefaultAsync(c => c.Id == request.AdminId);
            if (admin is null || admin.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoAdminFound"],
                    Description = $"{_localizer["NoAdminFoundWithId"]} {request.AdminId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            // check startdate is not more than today and check it is not more than the end date and end date is not today

            var election = new Election()
            {
                ElectionName = request.ElectionName,
                ElectionDescription = request.ElectionDescription,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Country = request.Country,
                ElectionType = request.ElectionType,
                ElectionOptions = request.ElectionOptions,
            };

            _dbContext.Election.Add(election);
            await _dbContext.SaveChangesAsync();

            return new(true);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorAddElection"]} {request.AdminId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<GetCandidateResponse>> GetCandidate(int voterId, int adminId)
    {
        try
        {
            var admin = await _dbContext.Voter.FirstOrDefaultAsync(c => c.Id == adminId);
            if (admin is null || admin.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoAdminFound"],
                    Description = $"{_localizer["NoAdminFoundWithId"]} {adminId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var voter = await _dbContext.Voter.FirstOrDefaultAsync(c => c.Id == voterId);
            if (voter is null || voter.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoVoterFound"],
                    Description = $"{_localizer["NoVoterFoundWithId"]} {voterId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            if (voter.IsCandidate == false)
                return new(new ErrorResponse()
                {
                    Title = _localizer["VoterNotCandidate"],
                    Description = $"{_localizer["VoterNotCandidateWithId"]} {voterId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var response = new GetCandidateResponse(voter);

            return new(response);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorGetCandidate"]} {adminId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }
}
