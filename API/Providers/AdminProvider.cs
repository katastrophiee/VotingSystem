using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Security.Cryptography;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.DbModels.Admin;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests.Admin;
using VotingSystem.API.DTO.Responses;
using VotingSystem.API.DTO.Responses.Admin;
using VotingSystem.API.Enums;
using VotingSystem.API.Interfaces.Provider;
using VotingSystem.API.Repository.DBContext;

namespace VotingSystem.API.Providers;

public class AdminProvider(DBContext dbContext, IStringLocalizer<AdminProvider> localizer) : IAdminProvider
{
    private readonly DBContext _dbContext = dbContext;
    private readonly IStringLocalizer<AdminProvider> _localizer = localizer;

    public async Task<Response<IEnumerable<AdminGetVotersResponse>>> GetVoters(AdminGetVotersRequest request)
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
               .ToListAsync() ?? [];

            var response = voters.Select(v => new AdminGetVotersResponse(v));

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
                    Title = _localizer["NoVoterFound"],
                    Description = $"{_localizer["NoVoterFoundWithId"]} {voterId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var response = new AdminGetVoterResponse(voter);

            var currentIdDocument = await _dbContext.Document.FirstOrDefaultAsync(c => c.VoterId == voterId && c.MostRecentId == true);
            if (currentIdDocument is not null)
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
                if (request.DocumentExpiryDate is null)
                    return new(new ErrorResponse()
                    {
                        Title = _localizer["NoExpiryDate"],
                        Description = $"{_localizer["NoExpiryDateDescription"]} {request.DocumentId}",
                        StatusCode = StatusCodes.Status404NotFound
                    });

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

    public async Task<Response<int>> AddElection(AdminAddElectionRequest request)
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

            if (string.IsNullOrEmpty(request.ElectionName))
            {
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoElectionName"],
                    Description = $"{_localizer["NoElectionNameWithId"]} {request.AdminId}",
                    StatusCode = StatusCodes.Status404NotFound
                });
            }

            if (string.IsNullOrEmpty(request.ElectionDescription))
            {
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoElectionDescription"],
                    Description = $"{_localizer["NoElectionDescriptionWithId"]} {request.AdminId}",
                    StatusCode = StatusCodes.Status404NotFound
                });
            }

            if (request.StartDate <= DateTime.Now
                || request.StartDate >= request.EndDate
                || request.EndDate <= DateTime.Now)
            {
                return new(new ErrorResponse()
                {
                    Title = _localizer["InvalidStartOrEndDate"],
                    Description = $"{_localizer["InvalidStartOrEndDateWithId"]} {request.AdminId}",
                    StatusCode = StatusCodes.Status404NotFound
                });
            }

            if (request.ElectionOptions.Count == 0)
            {
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoElectionOptions"],
                    Description = $"{_localizer["NoElectionOptionsWithId"]} {request.AdminId}",
                    StatusCode = StatusCodes.Status404NotFound
                });
            }

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

            var addedElection = await _dbContext.Election.FirstOrDefaultAsync(c => c.ElectionName == request.ElectionName && c.StartDate == request.StartDate && c.EndDate == request.EndDate);
            if (addedElection is null || addedElection.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoElectionFound"],
                    Description = $"{_localizer["NoElectionFoundWithId"]} {request.AdminId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            return new(addedElection.Id);
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

    public async Task<Response<AdminGetCandidateResponse>> GetCandidate(int voterId, int adminId)
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

            var response = new AdminGetCandidateResponse(voter);

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

    public async Task<Response<IEnumerable<AdminGetAdminResponse>>> GetAdmins(AdminGetAdminRequest request)
    {
        try
        {
            var admin = await _dbContext.Admin.FirstOrDefaultAsync(c => c.Id == request.RequestingAdminId);
            if (admin is null || admin.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoAdminFound"],
                    Description = $"{_localizer["NoAdminFoundWithId"]} {request.RequestingAdminId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var admins = await _dbContext.Admin.Where(a =>
                a.Id != request.RequestingAdminId &&
               (request.AdminId == null || a.Id == request.AdminId) &&
               (request.Country == null || a.Country == request.Country) &&
               (request.IsActive == null || a.IsActive == request.IsActive))
              .ToListAsync() ?? [];

            var response = admins.Select(a => new AdminGetAdminResponse(a));

            return new(response);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorGetAdmins"]} {request.RequestingAdminId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<AdminGetAdminResponse>> GetAdmin(int currentAdminId, int requestedAdminId)
    {
        try
        {
            var currentAdmin = await _dbContext.Admin.FirstOrDefaultAsync(c => c.Id == currentAdminId);
            if (currentAdmin is null || currentAdmin.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoAdminFound"],
                    Description = $"{_localizer["NoAdminFoundWithId"]} {currentAdminId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var admin = await _dbContext.Admin.FirstOrDefaultAsync(c => c.Id == requestedAdminId);
            if (admin is null || admin.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoAdminFound"],
                    Description = $"{_localizer["NoAdminFoundWithId"]} {requestedAdminId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            return new(new AdminGetAdminResponse(admin));
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorGetAdmin"]} {currentAdminId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<bool>> UpdateAdmin(AdminUpdateAdminRequest request)
    {
        try
        {
            var currentAdmin = await _dbContext.Admin.FirstOrDefaultAsync(c => c.Id == request.AdminId);
            if (currentAdmin is null || currentAdmin.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoAdminFound"],
                    Description = $"{_localizer["NoAdminFoundWithId"]} {request.AdminId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var adminToUpdate = await _dbContext.Admin.FirstOrDefaultAsync(c => c.Id == request.UpdatingAdminId);
            if (adminToUpdate is null || adminToUpdate.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoAdminFound"],
                    Description = $"{_localizer["NoAdminFoundWithId"]} {request.UpdatingAdminId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            if (request.DisplayName != null)
                adminToUpdate.DisplayName = request.DisplayName;

            if (request.Email != null)
                adminToUpdate.Email = request.Email;

            if (request.Password != null)
            {
                string salt = adminToUpdate.PasswordSalt;
                var pbkdf2HashedPassword = request.Password.Pbkdf2HashString(ref salt);

                adminToUpdate.Password = pbkdf2HashedPassword;
            }

            if (request.Country != null)
                adminToUpdate.Country = request.Country.Value;

            if (request.IsActive != adminToUpdate.IsActive)
                adminToUpdate.IsActive = request.IsActive;

            _dbContext.Admin.Update(adminToUpdate);
            await _dbContext.SaveChangesAsync();

            return new(true);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorUpdateAdmin"]} {request.AdminId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<int>> AddTask(AdminAddTaskRequest request)
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

            var newTask = new AdminTask
            {
                CreatedByAdminId = admin.Id,
                ForVoterId = request.ForVoterId,
                ForAdminId = request.ForAdminId,
                Name = request.Name,
                Description = request.Description,
                TaskStatus = Enums.TaskStatus.New,
                CreatedAt = DateTime.Now,
                LastEdited = null,
                AdditionalNotes = null
            };

            _dbContext.AdminTask.Add(newTask);
            await _dbContext.SaveChangesAsync();

            var addedTask = await _dbContext.AdminTask.FirstOrDefaultAsync(c => c.CreatedByAdminId == admin.Id && c.Name == request.Name && c.CreatedAt == newTask.CreatedAt);
            if (addedTask is null || addedTask.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoTaskFound"],
                    Description = $"{_localizer["NoTaskFoundWithId"]} {request.AdminId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            return new(addedTask.Id);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorAddTask"]} {request.AdminId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<IEnumerable<AdminGetTaskResponse>>> GetTasks(AdminGetTasksRequest request)
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

            var tasks = await _dbContext.AdminTask.Where(c =>
                (request.TaskId == null || c.Id == request.TaskId) &&
                (request.ForVoterId == null || c.ForVoterId == request.ForVoterId) &&
                (request.ForAdminId == null || c.ForAdminId == request.ForAdminId) &&
                (request.Name == null || c.Name.Contains(request.Name)) &&
                (request.TaskStatus == null || c.TaskStatus == request.TaskStatus) &&
                (request.AssignedToAdminId == null || c.AssignedToAdminId == request.AssignedToAdminId))
                .ToListAsync() ?? [];

            var response = tasks.Select(t => new AdminGetTaskResponse(t));

            return new(response);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorGetTasks"]} {request.AdminId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<AdminGetTaskResponse>> GetTask(int taskId, int adminId)
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

            var task = await _dbContext.AdminTask.Where(c => c.Id == taskId).FirstOrDefaultAsync();
            if (task is null || task.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoTaskFound"],
                    Description = $"{_localizer["NoTaskFoundWithId"]} {taskId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            return new(new AdminGetTaskResponse(task));
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorGetTask"]} {taskId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<bool>> DeleteTask(int taskId, int adminId)
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

            var task = await _dbContext.AdminTask.Where(c => c.Id == taskId).FirstOrDefaultAsync();
            if (task is null || task.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoTaskFound"],
                    Description = $"{_localizer["NoTaskFoundWithId"]} {taskId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            _dbContext.AdminTask.Remove(task);
            await _dbContext.SaveChangesAsync();

            return new(true);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorDeleteTask"]} {adminId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<bool>> UpdateTask(AdminUpdateTaskRequest request)
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

            var task = await _dbContext.AdminTask.Where(c => c.Id == request.TaskId).FirstOrDefaultAsync();
            if (task is null || task.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoTaskFound"],
                    Description = $"{_localizer["NoTaskFoundWithId"]} {request.TaskId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            if (request.ForVoterId != null)
                task.ForVoterId = request.ForVoterId;

            if (request.ForAdminId != null)
                task.ForAdminId = request.ForAdminId;

            if (request.Name != null)
                task.Name = request.Name;

            if (request.Description != null)
                task.Description = request.Description;

            if (request.TaskStatus != null)
                task.TaskStatus = request.TaskStatus.Value;

            if (request.AssignedToAdminId != null)
                task.AssignedToAdminId = request.AssignedToAdminId;

            if (request.AdditionalNotes != null)
                task.AdditionalNotes = request.AdditionalNotes + "\n";

            task.LastEdited = DateTime.Now;

            _dbContext.AdminTask.Update(task);
            await _dbContext.SaveChangesAsync();

            return new(true);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorUpdateTask"]} {request.TaskId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<bool>> UpdateVoterDetails(AdminUpdateVoterDetailsRequest request)
    {
        try
        {
            var admin = await _dbContext.Admin.FirstOrDefaultAsync(a => a.Id == request.AdminId);
            if (admin is null || admin.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoAdminFound"],
                    Description = $"{_localizer["NoAdminFoundWithId"]} {request.AdminId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var voter = await _dbContext.Voter.Where(v => v.Id == request.VoterId).FirstOrDefaultAsync();
            if (voter is null || voter.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoVoterFound"],
                    Description = $"{_localizer["NoVoterFoundWithId"]} {request.VoterId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            if (request.Email != null)
                voter.Email = request.Email;

            if (request.Password != null)
            {
                string newSalt = GenerateSalt();
                voter.PasswordSalt = newSalt;

                var pbkdf2HashedPassword = request.Password.Pbkdf2HashString(ref newSalt);

                voter.Password = pbkdf2HashedPassword;
            }

            if (request.FirstName != null)
                voter.FirstName = request.FirstName;

            if (request.LastName != null)
                voter.LastName = request.LastName;

            if (request.Address != null)
                voter.Address = request.Address;

            if (request.DateOfBirth != null)
                voter.DateOfBirth = request.DateOfBirth.Value;

            if (request.Country != null)
                voter.Country = request.Country.Value;

            if (request.NewUser != null)
                voter.NewUser = request.NewUser.Value;

            if (request.IsVerified != null)
                voter.IsVerified = request.IsVerified.Value;

            if (request.IsCandidate != null)
            {
                if (request.IsCandidate is false)
                {
                    voter.IsCandidate = false;
                    voter.CandidateName = null;
                    voter.CandidateDescription = null;
                }
                else
                {
                    if (request.CandidateName == null || request.CandidateDescription == null)
                        return new(new ErrorResponse()
                        {
                            Title = _localizer["NoCandidateDetailsProvided"],
                            Description = $"{_localizer["NoCandidateDetailsProvidedWithId"]} {request.VoterId}",
                            StatusCode = StatusCodes.Status404NotFound
                        });

                    voter.IsCandidate = true;
                    voter.CandidateName = request.CandidateName;
                    voter.CandidateDescription = request.CandidateDescription;
                }

            }

            if (request.IsActive != null)
                voter.IsActive = request.IsActive.Value;

            _dbContext.Voter.Update(voter);
            await _dbContext.SaveChangesAsync();

            return new(true);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorUpdateVoterDetails"]} {request.AdminId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    private static string GenerateSalt(int size = 32)
    {
        var buff = new byte[size];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(buff);

        return Convert.ToBase64String(buff);
    }

    public async Task<Response<IEnumerable<AdminGetElectionResponse>>> GetElections(AdminGetElectionsRequest request)
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

            var elections = await _dbContext.Election.Where(c =>
                (request.ElectionId == null || c.Id == request.ElectionId) &&
                (request.ElectionName == null || c.ElectionName.Contains(request.ElectionName)) &&
                (request.StartDate == null || c.StartDate == request.StartDate) &&
                (request.EndDate == null || c.EndDate == request.EndDate) &&
                (request.Country == null || c.Country == request.Country) &&
                (request.ElectionType == null || c.ElectionType == request.ElectionType))
                .ToListAsync() ?? [];

            var response = elections.Select(e => new AdminGetElectionResponse(e));

            return new(response);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorGetElections"]} {request.AdminId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<AdminGetElectionResponse>> GetElection(int electionId, int adminId)
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

            var election = await _dbContext.Election.Where(c => c.Id == electionId).FirstOrDefaultAsync();
            if (election is null || election.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoElectionFound"],
                    Description = $"{_localizer["NoElectionFoundWithId"]} {electionId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var response = new AdminGetElectionResponse(election);

            return new(response);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorGetElection"]} {adminId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<bool>> DeleteElection(int electionId, int adminId)
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

            var election = await _dbContext.Election.Where(c => c.Id == electionId).FirstOrDefaultAsync();
            if (election is null || election.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoElectionFound"],
                    Description = $"{_localizer["NoElectionFoundWithId"]} {election}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            _dbContext.Election.Remove(election);
            await _dbContext.SaveChangesAsync();

            return new(true);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorDeleteElection"]} {adminId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<bool>> UpdateElection(AdminUpdateElectionRequest request)
    {
        try
        {
            var admin = await _dbContext.Admin.FirstOrDefaultAsync(a => a.Id == request.AdminId);
            if (admin is null || admin.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoAdminFound"],
                    Description = $"{_localizer["NoAdminFoundWithId"]} {request.AdminId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var election = await _dbContext.Election.Where(v => v.Id == request.ElectionId).FirstOrDefaultAsync();
            if (election is null || election.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoElectionFound"],
                    Description = $"{_localizer["NoTaskFoundWithId"]} {request.AdminId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            if (request.ElectionName != null)
                election.ElectionName = request.ElectionName;

            if (request.ElectionDescription != null)
                election.ElectionDescription = request.ElectionDescription;

            if (request.StartDate != null)
                election.StartDate = request.StartDate.Value;

            if (request.EndDate != null)
                election.EndDate = request.EndDate.Value;

            if (request.Country != null)
                election.Country = request.Country.Value;

            if (request.ElectionType != null)
                election.ElectionType = request.ElectionType.Value;

            if (request.ElectionOptions == null || request.ElectionOptions.Count == 0)
            {
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoElectionOptions"],
                    Description = $"{_localizer["NoElectionOptionsWithId"]} {request.AdminId}",
                    StatusCode = StatusCodes.Status404NotFound
                });
            }

            election.ElectionOptions = request.ElectionOptions;

            _dbContext.Election.Update(election);
            await _dbContext.SaveChangesAsync();

            return new(true);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorUpdateElection"]} {request.AdminId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<IEnumerable<ElectionType>>> GetAvailableVotingSystems(UserCountry country, int adminId)
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

            if (country == 0 || country == UserCountry.Unknown)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoCountryFound"],
                    Description = $"{_localizer["NoCountryFoundWithId"]} {adminId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var countryElectionTypes = new Dictionary<UserCountry, List<ElectionType>>
            {
                { UserCountry.England, new List<ElectionType> { ElectionType.GeneralElection_FPTP, ElectionType.ParliamentaryElection_FPTP, ElectionType.LocalGovernmentElection_FPTP } },
                { UserCountry.Wales, new List<ElectionType> { ElectionType.GeneralElection_FPTP, ElectionType.ParliamentaryElection_FPTP, ElectionType.LocalGovernmentElection_FPTP } },
                { UserCountry.NorthernIreland, new List<ElectionType> { ElectionType.Election_STV } },
                { UserCountry.Scotland, new List<ElectionType> { ElectionType.Election_STV } },
                { UserCountry.Australia, new List<ElectionType> { ElectionType.Election_Preferential } },
                { UserCountry.Ireland, new List<ElectionType> { ElectionType.Election_Preferential } },
                { UserCountry.UnitedStates, new List<ElectionType> { ElectionType.Election_Preferential } },
            };

            if (countryElectionTypes.TryGetValue(country, out var electionTypes))
            {
                return new Response<IEnumerable<ElectionType>>(electionTypes);
            }
            else
            {
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoElectionTypesFound"],
                    Description = $"{_localizer["NoElectionTypesFoundForCountry"]} {country.LocalisedEnumDisplayName(_localizer)}",
                    StatusCode = StatusCodes.Status404NotFound
                });
            }
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorGetAvailableVotingSystems"]} {adminId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

}
