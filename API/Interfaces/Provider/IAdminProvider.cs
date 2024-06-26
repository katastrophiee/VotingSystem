﻿using VotingSystem.API.DTO.Requests.Admin;
using VotingSystem.API.DTO.Responses;
using VotingSystem.API.DTO.Responses.Admin;
using VotingSystem.API.Enums;

namespace VotingSystem.API.Interfaces.Provider;

public interface IAdminProvider
{
    Task<Response<IEnumerable<AdminGetVotersResponse>>> GetVoters(AdminGetVotersRequest request);

    Task<Response<AdminGetVoterResponse>> GetVoterDetails(int voterId, int adminId);

    Task<Response<bool>> AdminVerifyId(AdminVerifyIdRequest request);

    Task<Response<int>> AddElection(AdminAddElectionRequest request);

    Task<Response<AdminGetCandidateResponse>> GetCandidate(int voterId, int adminId);

    Task<Response<IEnumerable<AdminGetAdminResponse>>> GetAdmins(AdminGetAdminRequest request);

    Task<Response<AdminGetAdminResponse>> GetAdmin(int currentAdminId, int requestedAdminId);

    Task<Response<bool>> UpdateAdmin(AdminUpdateAdminRequest request);

    Task<Response<int>> AddTask(AdminAddTaskRequest request);

    Task<Response<IEnumerable<AdminGetTaskResponse>>> GetTasks(AdminGetTasksRequest request);

    Task<Response<AdminGetTaskResponse>> GetTask(int taskId, int adminId);

    Task<Response<bool>> DeleteTask(int taskId, int adminId);

    Task<Response<bool>> UpdateTask(AdminUpdateTaskRequest request);

    Task<Response<bool>> UpdateVoterDetails(AdminUpdateVoterDetailsRequest request);

    Task<Response<IEnumerable<AdminGetElectionResponse>>> GetElections(AdminGetElectionsRequest request);

    Task<Response<AdminGetElectionResponse>> GetElection(int electionId, int adminId);

    Task<Response<bool>> DeleteElection(int electionId, int adminId);

    Task<Response<bool>> UpdateElection(AdminUpdateElectionRequest request);

    Task<Response<IEnumerable<ElectionType>>> GetAvailableVotingSystems(UserCountry country, int adminId);

}
