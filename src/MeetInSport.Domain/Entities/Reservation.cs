using System.ComponentModel;
using MeetInSport.Domain.Common;
using MeetInSport.Domain.Enum;

namespace MeetInSport.Domain.Entities
{
    public class Reservation : BaseEntity
    {
        public Guid StudentId { get; set; }
        public Guid CoachId { get; set; }
        public Guid PackageId { get; set; }

        public DateTime ScheduledAt { get; set; }
        public LocationType LocationType { get; set; }
        public ReservationStatus Status { get; set; } = ReservationStatus.Pending; // all new reservations start as pending
        public string? Notes { get; set; }

        public DateTime? CancelledAt { get; set; }
        public string? CancelReason { get; set; }

        public virtual User Student { get; set; } = null!;
        public virtual Coach Coach { get; set; } = null!;
        public virtual LessonPackage Package { get; set; } = null!;
        public virtual Payment? Payment { get; set; }
    }
}