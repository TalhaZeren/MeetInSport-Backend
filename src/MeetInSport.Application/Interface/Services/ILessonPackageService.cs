using MeetInSport.Application.DTOs.LessonPackage;

namespace MeetInSport.Application.Interface.Services;

public interface ILessonPackageService
{
    Task<LessonPackageResponseDto> CreatePackageAsync(CreateLessonPackageDto createLessonPackageDto, Guid currentUserId);
    Task<IEnumerable<LessonPackageResponseDto>> GetPackagesByCoachIdAsync(Guid coachId);
    Task DeletePackageAsync(Guid packageId, Guid userId);
    Task<LessonPackageResponseDto> UpdatePackageAsync(Guid packageId,UpdateLessonPackageDto UpdateLessonPackageDto,  Guid currentUserId);
    Task<LessonPackageResponseDto> GetPackageByIdAsync(Guid packageId); 


}

