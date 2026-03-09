
using MyEducation.Application.Interfaces.ManageUser;
using Microsoft.AspNetCore.Mvc;
using MyEducation.Domain.Entities;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserProfileRepository _userProfileRepository;

    public UserController(IUserProfileRepository userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserProfile>> GetUserProfile(int id)
    {
        var userProfile = await _userProfileRepository.GetUserProfileById(id);
        if (userProfile == null)
        {
            return NotFound();
        }
        return Ok(userProfile);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUserProfile(int id, UserProfile updatedProfile)
    {
        if (id != updatedProfile.UserId)
        {
            return BadRequest();
        }

        var existingProfile = await _userProfileRepository.GetUserProfileById(id);
        if (existingProfile == null)
        {
            return NotFound();
        }

        existingProfile.DisplayName = updatedProfile.DisplayName;
        existingProfile.FirstName = updatedProfile.FirstName;
        existingProfile.LastName = updatedProfile.LastName;
        existingProfile.Email = updatedProfile.Email;
        existingProfile.DateOfBirth = updatedProfile.DateOfBirth;

        await _userProfileRepository.UpdateUserProfile(existingProfile);
        return NoContent();
    }
}