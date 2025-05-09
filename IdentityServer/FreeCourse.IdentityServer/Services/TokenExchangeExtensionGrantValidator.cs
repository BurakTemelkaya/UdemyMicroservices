﻿using Duende.IdentityServer.Validation;

namespace FreeCourse.IdentityServer.Services;

public class TokenExchangeExtensionGrantValidator : IExtensionGrantValidator
{
    public string GrantType => "urn:ietf:params:oauth:grant-type:token-exchange";

    private readonly ITokenValidator _tokenValidator;

    public TokenExchangeExtensionGrantValidator(ITokenValidator tokenValidator)
    {
        _tokenValidator = tokenValidator;
    }

    public async Task ValidateAsync(ExtensionGrantValidationContext context)
    {
        string requestRaw = context.Request.Raw.ToString();

        string? token = context.Request.Raw.Get("subject_token");

        if (string.IsNullOrEmpty(token))
        {
            context.Result = new GrantValidationResult(Duende.IdentityServer.Models.TokenRequestErrors.InvalidRequest, "token missing");
            return;
        }

        var tokenValidateResult = await _tokenValidator.ValidateAccessTokenAsync(token);

        if (tokenValidateResult.IsError)
        {
            context.Result = new GrantValidationResult(Duende.IdentityServer.Models.TokenRequestErrors.InvalidGrant, "token invalid");
            return;
        }

        System.Security.Claims.Claim? subjectClaim = tokenValidateResult.Claims?.FirstOrDefault(x => x.Type == "sub");

        if (subjectClaim == null)
        {
            context.Result = new GrantValidationResult(Duende.IdentityServer.Models.TokenRequestErrors.InvalidGrant, "token must contain sub value");
            return;
        }

        context.Result = new GrantValidationResult(subjectClaim.Value, "access_token", tokenValidateResult.Claims);
    }
}
