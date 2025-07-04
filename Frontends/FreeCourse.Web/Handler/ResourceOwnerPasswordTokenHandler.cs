﻿using FreeCourse.Web.Exceptions;
using FreeCourse.Web.Services.Interfaces;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Net;
using System.Net.Http.Headers;

namespace FreeCourse.Web.Handler;

public class ResourceOwnerPasswordTokenHandler:DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;
    private readonly ILogger<ResourceOwnerPasswordTokenHandler> _logger;

    public ResourceOwnerPasswordTokenHandler(IHttpContextAccessor httpContextAccessor, IIdentityService identityService, ILogger<ResourceOwnerPasswordTokenHandler> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string? accessToken = await _httpContextAccessor.HttpContext!.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            TokenResponse tokenResponse = await _identityService.GetAccessTokenByRefreshTokenAsync();

            if (tokenResponse != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

                response = await base.SendAsync(request, cancellationToken);
            }
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new UnAuthorizeException();
        }

        return response;
    }
}
