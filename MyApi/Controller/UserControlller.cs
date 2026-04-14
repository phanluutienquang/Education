
using MyEducation.MyApplication.Interfaces.ManageUser;
using MyEducation.MyApplication.Interfaces.Common;
using MyEducation.MyApplication.Interfaces.Graph;
using MyEducation.MyApplication.Models;
using MyEducation.MyDomain.Entities.Users;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUserClaims _userClaim;
    private readonly IGraphService _graphService;

    public UserController(IUserProfileRepository userProfileRepository, IUserClaims userClaim, 
    IGraphService graphService)
    {
        _userProfileRepository = userProfileRepository;
        _userClaim = userClaim;
        _graphService = graphService;
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

        await _userProfileRepository.UpdateUserProfile(existingProfile);
        return NoContent();
    }

        [HttpGet("b2c-users")]
        public async Task<ActionResult<List<AdB2CUserModel>>> GetB2CUsers()
        {
            var users = await _graphService.GetADB2CUsersAsync();
            return Ok(users);
        }

        
}