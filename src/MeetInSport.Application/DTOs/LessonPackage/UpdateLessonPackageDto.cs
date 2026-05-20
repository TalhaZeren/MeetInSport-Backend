using MeetInSport.Domain.Enum;
namespace MeetInSport.Application.DTOs.LessonPackage;


public class UpdateLessonPackageDto { 
 
    public string PackageName { get; set; } = string.Empty;
    public string PackageDescription { get; set; } = string.Empty;
    public decimal DurationInMinutes { get; set; }
    public decimal PackagePrice { get; set; }
    public List<string> Requirements { get; set; } = new List<string>();
    public LocationType LocationType { get; set; }
    public LessonModel LessonModel { get; set; }
    public string? CoverImageUrl { get; set; }
    public bool IsActive { get; set; } // seperates from the CreateLessonPackageDto
    public int ExpirationDays { get; set; }
}


