﻿using FreeCourse.Web.Exceptions;
using FreeCourse.Web.Services.Interfaces;
using System.Net.Http.Headers;

namespace FreeCourse.Web.Handler;

public class ClientCredentialTokenHandler : DelegatingHandler
{
    private readonly IClientCredentialTokenService _clientCredentialTokenService;

    public ClientCredentialTokenHandler(IClientCredentialTokenService clientCredentialTokenService)
    {
        _clientCredentialTokenService = clientCredentialTokenService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", await _clientCredentialTokenService.GetTokenAsync());

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            throw new UnAuthorizeException(request.Headers.Authorization + " "+ response.Content.ToString() + "" + response.RequestMessage);
        }

        return response;
    }
}
