using VotingSystem.API.DTO.Requests.Admin;
using VotingSystem.API.DTO.Responses;
using VotingSystem.API.DTO.Responses.Admin;

namespace VotingSystem.API.Interfaces.Provider;

public interface IAdminProvider
{
    Task<Response<List<AdminGetVotersResponse>>> GetCustomers(AdminGetCustomersRequest request);

    Task<Response<AdminGetCustomerResponse>> GetCustomerDetails(int customerId, int adminId);

    Task<Response<bool>> AdminVerifyId(AdminVerifyIdRequest request);
}
