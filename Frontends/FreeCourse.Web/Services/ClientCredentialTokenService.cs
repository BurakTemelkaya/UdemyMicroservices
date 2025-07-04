﻿using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using IdentityModel.Client;

namespace FreeCourse.Web.Services;

public class ClientCredentialTokenService : IClientCredentialTokenService
{
    private readonly ServiceApiSettings _serviceApiSettings;
    private readonly ClientSettings _clientSettings;
    private readonly IMemoryCache _memoryCache;
    private readonly HttpClient _httpClient;

    private const string TokenCacheKey = "ClientCredentialsToken";

    public ClientCredentialTokenService(IOptions<ServiceApiSettings> serviceApiSettings, IOptions<ClientSettings> clientSettings,IMemoryCache memoryCache, HttpClient httpClient)
    {
        _serviceApiSettings = serviceApiSettings.Value;
        _clientSettings = clientSettings.Value;
        _memoryCache = memoryCache;
        _httpClient = httpClient;
    }

    public async Task<string> GetTokenAsync()
    {
        if (_memoryCache.TryGetValue(TokenCacheKey, out string? accessToken))
        {
            return accessToken!;
        }

        DiscoveryDocumentResponse disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = _serviceApiSettings.IdentityBaseUri,
            Policy = new DiscoveryPolicy
            {
                RequireHttps = false,
                ValidateIssuerName = false
            }
        });

        if (disco == null)
        {
            throw new Exception($"disco is empty");
        }

        if (disco.IsError)
        {
            var errorDetails = $"""
                Discovery error:
                - ErrorType: {disco.ErrorType}
                - Error: {disco.Error}
                - StatusCode: {disco.HttpStatusCode}
                - Raw: {disco.Raw}
            """;

            // Log veya Exception fırlat
            

            throw disco.Exception ?? throw new Exception(errorDetails);
        }


        ClientCredentialsTokenRequest clientCredentialTokenRequest = new ClientCredentialsTokenRequest
        {
            ClientId = _clientSettings.WebClient.ClientId,
            ClientSecret = _clientSettings.WebClient.ClientSecret,
            Address = disco.TokenEndpoint,
        };

        TokenResponse newToken = await _httpClient.RequestClientCredentialsTokenAsync(clientCredentialTokenRequest);

        if (newToken.IsError)
        {
            throw newToken.Exception!;
        }

        _memoryCache.Set(TokenCacheKey, newToken.AccessToken, TimeSpan.FromSeconds(newToken.ExpiresIn));

        return newToken.AccessToken!;
    }
}
