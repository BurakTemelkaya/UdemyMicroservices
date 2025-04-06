using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FreeCourse.Web.Controllers;

public class AuthController : Controller
{
    private readonly IIdentityService _identityService;

    public AuthController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public IActionResult SignIn()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SignIn(SigninInput signinInput)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        Response<bool> response = await _identityService.SignInAsync(signinInput);

        if (response.IsSuccessful)
        {
            return RedirectToAction(nameof(Index), "Home");
        }

        foreach (string error in response.Errors!)
        {
            ModelState.AddModelError(String.Empty, error);
        }

        return View();
    }
}
