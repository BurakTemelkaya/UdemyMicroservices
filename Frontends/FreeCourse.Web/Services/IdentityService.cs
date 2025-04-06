using Duende.IdentityModel.Client;
using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Globalization;
using System.Security.Claims;
using System.Text.Json;

namespace FreeCourse.Web.Services;

public class IdentityService : IIdentityService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ClientSettings _clientSettings;
    private readonly ServiceApiSettings _serviceApiSettings;

    public IdentityService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IOptions<ClientSettings> clientSettings, IOptions<ServiceApiSettings> serviceApiSettings)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _clientSettings = clientSettings.Value;
        _serviceApiSettings = serviceApiSettings.Value;
    }

    public async Task<TokenResponse> GetAccessTokenByRefreshTokenAsync()
    {
        DiscoveryDocumentResponse disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = _serviceApiSettings.IdentityBaseUri,
            Policy = new DiscoveryPolicy
            {
                RequireHttps = false
            }
        });

        if (disco.IsError)
        {
            throw disco.Exception!;
        }

        string? refreshToken = await _httpContextAccessor.HttpContext!.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new Exception("Refresh token is not found");
        }

        RefreshTokenRequest refreshTokenRequest = new()
        {
            ClientId = _clientSettings.WebClientForUser.ClientId,
            ClientSecret = _clientSettings.WebClientForUser.ClientSecret,
            Address = disco.TokenEndpoint,
            RefreshToken = refreshToken
        };

        TokenResponse token = await _httpClient.RequestRefreshTokenAsync(refreshTokenRequest);

        if (token.IsError)
        {
            return null;
        }

        AuthenticationProperties authenticationProperties = new();

        List<AuthenticationToken> authenticationTokens = new()
        {
            new() {
                Name =  OpenIdConnectParameterNames.AccessToken,
                Value = token.AccessToken!
            },
            new()
            {
                Name = OpenIdConnectParameterNames.RefreshToken,
                Value = token.RefreshToken!
            },
            new()
            {
                Name = OpenIdConnectParameterNames.ExpiresIn,
                Value = DateTime.Now.AddSeconds(token.ExpiresIn).ToString("O",CultureInfo.InvariantCulture)
            },
        };

        AuthenticateResult authenticationResult = await _httpContextAccessor.HttpContext!.AuthenticateAsync();

        AuthenticationProperties? properties = authenticationResult.Properties;

        properties!.StoreTokens(authenticationTokens);

        await _httpContextAccessor.HttpContext!.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, authenticationResult.Principal!, properties);

        return token;
    }

    public async Task RevokeRefreshTokenAsync()
    {
        DiscoveryDocumentResponse disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = _serviceApiSettings.IdentityBaseUri,
            Policy = new DiscoveryPolicy
            {
                RequireHttps = false
            }
        });

        if (disco.IsError)
        {
            throw disco.Exception!;
        }

        string? refreshToken = await _httpContextAccessor.HttpContext!.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new Exception("Refresh token is not found");
        }

        TokenRevocationRequest tokenRevocationRequest = new()
        {
            ClientId = _clientSettings.WebClientForUser.ClientId,
            ClientSecret = _clientSettings.WebClientForUser.ClientSecret,
            Address = disco.RevocationEndpoint,
            Token = refreshToken,
            TokenTypeHint = OpenIdConnectParameterNames.RefreshToken
        };

        TokenRevocationResponse tokenRevocationResponse = await _httpClient.RevokeTokenAsync(tokenRevocationRequest);

        if (tokenRevocationResponse.IsError)
        {
            throw new Exception(tokenRevocationResponse.Error);
        }
    }

    public async Task<Response<bool>> SignInAsync(SigninInput signInInput)
    {
        DiscoveryDocumentResponse disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = _serviceApiSettings.IdentityBaseUri,
            Policy = new DiscoveryPolicy
            {
                RequireHttps = false
            }
        });

        if (disco.IsError)
        {
            throw disco.Exception!;
        }

        PasswordTokenRequest passwordTokenRequest = new()
        {
            Address = disco.TokenEndpoint,
            ClientId = _clientSettings.WebClientForUser.ClientId,
            ClientSecret = _clientSettings.WebClientForUser.ClientSecret,
            UserName = signInInput.Email,
            Password = signInInput.Password,
        };

        TokenResponse token = await _httpClient.RequestPasswordTokenAsync(passwordTokenRequest);

        if (token.IsError)
        {
            string responseContent = await token.HttpResponse!.Content.ReadAsStringAsync();

            ErrorDto errorDto = JsonSerializer.Deserialize<ErrorDto>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;

            return Response<bool>.Fail(errorDto.Errors, StatusCodes.Status400BadRequest);
        }

        UserInfoRequest userInfoRequest = new()
        {
            Token = token.AccessToken,
            Address = disco.UserInfoEndpoint
        };

        UserInfoResponse userInfo = await _httpClient.GetUserInfoAsync(userInfoRequest);

        if (userInfo.IsError)
        {
            throw userInfo.Exception!;
        }

        ClaimsIdentity claimsIdentity = new(userInfo.Claims, CookieAuthenticationDefaults.AuthenticationScheme, "name", "role");

        ClaimsPrincipal claimsPrincipal = new(claimsIdentity);

        AuthenticationProperties authenticationProperties = new();

        authenticationProperties.StoreTokens(
        [
            new AuthenticationToken
            {
                Name =  OpenIdConnectParameterNames.AccessToken,
                Value = token.AccessToken
            },
            new AuthenticationToken
            {
                Name = OpenIdConnectParameterNames.RefreshToken,
                Value = token.RefreshToken
            },
            new AuthenticationToken
            {
                Name = OpenIdConnectParameterNames.ExpiresIn,
                Value = DateTime.Now.AddSeconds(token.ExpiresIn).ToString("O",CultureInfo.InvariantCulture)
            },
        ]);

        authenticationProperties.IsPersistent = signInInput.IsRemember;

        await _httpContextAccessor.HttpContext!.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authenticationProperties);

        return Response<bool>.Success(StatusCodes.Status200OK);
    }
}
