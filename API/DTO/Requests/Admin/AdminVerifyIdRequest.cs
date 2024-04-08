namespace VotingSystem.API.DTO.Requests.Admin;

public class AdminVerifyIdRequest
{
    public int AdminId { get; set; }

    public int DocumentId { get; set; }

    public DateTime? DocumentExpiryDate { get; set; }

    public bool IsRejected { get; set; }

    public string? RejectionReason { get; set; }
}
