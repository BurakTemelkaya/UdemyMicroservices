
using Duende.IdentityModel.Client;

namespace FreeCourse.Gateway.DelegateHandlers;

public class TokenExchangeDelegateHandler : DelegatingHandler
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private string _accessToken;

    public TokenExchangeDelegateHandler(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    private async Task<string> GetTokenAsync(string requestToken)
    {
        if (!string.IsNullOrEmpty(_accessToken))
        {
            return _accessToken;
        }

        DiscoveryDocumentResponse disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = _configuration["IdentityServerURL"],
            Policy = new DiscoveryPolicy
            {
                RequireHttps = false
            }
        });

        if (disco.IsError)
        {
            throw disco.Exception!;
        }

        TokenExchangeTokenRequest tokenExchangeTokenRequest = new()
        {
            Address = disco.TokenEndpoint,
            ClientId = _configuration["ClientId"]!,
            ClientSecret = _configuration["ClientSecret"]!,
            GrantType = _configuration["TokenGrantType"]!,
            SubjectToken = requestToken,
            SubjectTokenType = "urn:ietf:params:oauth:grant-type:access-token",
            Scope = "openid discount_fullpermission payment_fullpermission"
        };

        TokenResponse tokenResponse = await _httpClient.RequestTokenExchangeTokenAsync(tokenExchangeTokenRequest);

        if (tokenResponse.IsError)
        {
            throw tokenResponse.Exception!;
        }

        _accessToken = tokenResponse.AccessToken!;

        return _accessToken;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string? requestToken = request.Headers.Authorization?.Parameter;

        if (string.IsNullOrEmpty(requestToken))
        {
            throw new Exception("request token is empty");
        }

        string newToken = await GetTokenAsync(requestToken);

        request.SetBearerToken(newToken);

        return await base.SendAsync(request, cancellationToken);
    }
}
