﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.DbModels.Admin;
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

    public async Task<Response<bool>> AddElection(AdminAddElectionRequest request)
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
              .ToListAsync();

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

            if (request.IsActive != null)
                adminToUpdate.IsActive = request.IsActive.Value;

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

    public async Task<Response<bool>> AddTask(AdminAddTaskRequest request)
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

            return new(true);
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
                (request.Name == null || c.Name == request.Name) &&
                (request.TaskStatus == null || c.TaskStatus == request.TaskStatus) &&
                (request.AssignedToAdminId == null || c.AssignedToAdminId == request.AssignedToAdminId))
               .ToListAsync();

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
                    Description = $"{_localizer["NoTaskFoundWithId"]} {request.AdminId}",
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

            _dbContext.AdminTask.Update(task);
            await _dbContext.SaveChangesAsync();

            return new(true);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorUpdateTask"]} {request.AdminId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }
}
