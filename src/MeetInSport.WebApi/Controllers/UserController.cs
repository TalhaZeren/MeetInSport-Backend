using System.Security.Claims;
using MeetInSport.Application.DTOs.User;
using MeetInSport.Application.Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetInSport.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/users")]
public class UserController : ControllerBase{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPut("avatar")]
    public async Task<IActionResult> UpdateAvatar([FromBody] UpdateAvatarDto updateAvatarDto){
         var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
        {
            return Unauthorized(new { message = "Invalid token claims." });
        }
         
        var newUrl = await _userService.UpdateAvatarAsync(userId, updateAvatarDto.AvatarUrl);
        return Ok(new {avatarUrl = newUrl});
         
    }
}