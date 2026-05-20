namespace MeetInSport.Application.Interface.Services;

public interface IUserService{
    Task<string> UpdateAvatarAsync(Guid userId, string avatarUrl);
}