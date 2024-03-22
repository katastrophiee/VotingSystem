namespace VotingSystem.API.DTO.ErrorHandling
{
    public class ErrorResponse
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public ErrorCode ErrorCode { get; set; }

        public int StatusCode { get; set; }

        public Exception? Exception { get; set; }

        public ErrorResponse(ErrorCode errorCode, string? description = null, Exception? exception = null)
        {
            Title = errorCode.EnumDisplayName();
            Description = description ?? errorCode.EnumDescription();
            ErrorCode = errorCode;
            Exception = exception;

            switch (errorCode)
            {
                case ErrorCode.InternalServerError:
                    StatusCode = StatusCodes.Status500InternalServerError;
                    break;
                case ErrorCode.CustomerNotFound:
                    StatusCode = StatusCodes.Status404NotFound;
                    break;
                case ErrorCode.NoModelPassedIn:
                    StatusCode = StatusCodes.Status400BadRequest;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(errorCode), errorCode, null);
            }
        }
    }
}
