using VotingSystem.API.Enums;

namespace VotingSystem.API.DTO.DbModels;

public class Document
{
    public int Id { get; set; }

    public int VoterId { get; set; }

    public byte[] FileContent { get; set; }

    public string FileName { get; set; }

    public string MimeType { get; set; }

    public DocumentType DocumentType { get; set; }

    public DateTime UploadedDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public bool IsVerified { get; set; }

    public bool MostRecentId { get; set; }

    public bool? IdRejected { get; set; }

    public string? RejectionReason { get; set; }

    public int? RejectedByAdminId { get; set; }
}
