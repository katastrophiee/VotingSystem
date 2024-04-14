using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.Responses;

namespace VotingSystem.API.Interfaces.Provider;

public interface IDocumentProvider
{
    Task<Response<Document>> GetCurrentVoterDocument(int voterId);

    Task<Response<List<Document>>> GetVoterDocuments(int voterId);

    Task<Response<bool>> PostUploadVoterDocument(Document document);
}
