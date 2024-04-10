using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.Responses;

namespace VotingSystem.API.Interfaces.Provider;

public interface IDocumentProvider
{
    Task<Response<Document>> GetCurrentCustomerDocument(int customerId);

    Task<Response<List<Document>>> GetCustomerDocuments(int customerId);

    Task<Response<bool>> PostUploadCustomerDocument(Document document);
}
