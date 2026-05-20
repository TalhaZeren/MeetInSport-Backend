using MeetInSport.Application.Interface.Repositories;
using MeetInSport.Application.Interface.Services;
using MeetInSport.Application.Exceptions; // Assuming you have NotFoundException here
using MeetInSport.Domain.Entities;


namespace MeetInSport.Application.Services;

public class UserService : IUserService{

    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository){
        _userRepository = userRepository;
    }
    
    public async Task<string> UpdateAvatarAsync(Guid userId, string avatarUrl){
        
        var user = await _userRepository.GetByIdAsync(userId) ?? 
        throw new NotFoundException(nameof(User), userId);

        user.AvatarUrl = avatarUrl;
        user.UpdatedAt = DateTime.UtcNow;

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        return user.AvatarUrl;
    }

}