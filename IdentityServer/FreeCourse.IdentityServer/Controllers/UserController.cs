using FreeCourse.IdentityServer.Dtos;
using FreeCourse.IdentityServer.Models;
using FreeCourse.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using static Duende.IdentityServer.IdentityServerConstants;

namespace FreeCourse.IdentityServer.Controllers;

[Authorize(LocalApi.PolicyName)]
[Route("api/[controller]/[action]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }


    [HttpPost]
    public async Task<IActionResult> SignUp(SignUpDto signUpDto)
    {
        ApplicationUser user = new()
        {
            UserName = signUpDto.UserName,
            Email = signUpDto.Email,
            City = signUpDto.City
        };

        IdentityResult result = await _userManager.CreateAsync(user, signUpDto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(Response<NoContent>.Fail([.. result.Errors.Select(x => x.Description)], StatusCodes.Status400BadRequest));
        }

        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetUser()
    {
        string? userId = User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;

        if (userId == null)
        {
            return BadRequest();
        }

        ApplicationUser? user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return BadRequest();
        }

        return Ok(new { Id = user.Id, UserName = user.UserName, Email = user.Email, City = user.City });
    }
}
