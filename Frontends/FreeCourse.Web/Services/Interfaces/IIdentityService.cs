using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models;
using IdentityModel.Client;

namespace FreeCourse.Web.Services.Interfaces;

public interface IIdentityService
{
    Task<Response<bool>> SignInAsync(SigninInput signInInput);
    Task<TokenResponse> GetAccessTokenByRefreshTokenAsync();
    Task RevokeRefreshTokenAsync();
}
