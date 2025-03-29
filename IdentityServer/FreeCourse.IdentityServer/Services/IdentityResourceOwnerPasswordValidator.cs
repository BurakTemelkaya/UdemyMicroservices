using Duende.IdentityModel;
using Duende.IdentityServer.Validation;
using FreeCourse.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;

namespace FreeCourse.IdentityServer.Services;

public class IdentityResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityResourceOwnerPasswordValidator(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
    {
        ApplicationUser? existUser = await _userManager.FindByEmailAsync(context.UserName);

        if (existUser == null)
        {
            Dictionary<string, object> errors = [];

            errors.Add("errors", new List<string> { "Email or password is wrong." });

            context.Result.CustomResponse = errors;

            return;
        }

        bool passwordCheck = await _userManager.CheckPasswordAsync(existUser, context.Password);

        if (!passwordCheck)
        {
            Dictionary<string, object> errors = [];

            errors.Add("errors", new List<string> { "Email or password is wrong." });

            context.Result.CustomResponse = errors;

            return;
        }

        context.Result = new GrantValidationResult(existUser.Id, OidcConstants.AuthenticationMethods.Password);
    }
}
