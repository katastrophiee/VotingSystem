using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.Responses;

namespace VotingSystem.API.Interfaces.Provider;

public interface IDocumentProvider
{
    Task<Response<Document?>> GetCurrentVoterDocument(int voterId);

    Task<Response<IEnumerable<Document>>> GetVoterDocuments(int voterId);

    Task<Response<int>> UploadVoterDocument(Document document);
}
