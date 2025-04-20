namespace FreeCourse.Web.Services.Interfaces;

public interface IClientCredentialTokenService
{
    Task<String> GetTokenAsync();
}
