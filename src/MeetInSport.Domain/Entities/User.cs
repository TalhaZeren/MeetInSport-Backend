
namespace MeetInSport.Domain.Entities
{
    using MeetInSport.Domain.Common;

    public class User : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public int RoleId { get; set; } // The class definiton of the Role is in the down below.
        public string? PhoneNumber { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime KvkkAcceptedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsEmailVerified { get; set; } = false;
        public virtual Role Role { get; set; } = null!; 
        public virtual Coach? CoachProfile { get; set; } // 1 to 1 
        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>(); // 1 to N 
        public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    }
}