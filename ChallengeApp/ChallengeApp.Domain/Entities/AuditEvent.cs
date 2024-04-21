using ChallengeApp.Domain.Common;

namespace ChallengeApp.Domain.Entities;

public class AuditEvent : BaseEntity
{
    public string? EventType { get; set; }
    public string? JsonData { get; set; }
    public DateTime? InsertedDate { get; set; }
    public DateTime? LastUpdatedDate { get; set; }
    public string? RequestUrl { get; set; }
    public string? User { get; set; }

}