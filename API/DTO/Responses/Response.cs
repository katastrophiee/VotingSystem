using VotingSystem.API.DTO.ErrorHandling;

namespace VotingSystem.API.DTO.Responses;

public class Response<T>
{
    public T Data { get; set; }

    public ErrorResponse Error { get; set; }

    public Response(T data)
    {
        Data = data;
    }

    public Response(ErrorResponse error)
    {
        Error = error;
    }
}
