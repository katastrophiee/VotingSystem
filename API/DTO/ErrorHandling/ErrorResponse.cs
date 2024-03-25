namespace VotingSystem.API.DTO.ErrorHandling
{
    public class ErrorResponse
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public int? StatusCode { get; set; }

        public string AdditionalDetails { get; set; }
    }
}
