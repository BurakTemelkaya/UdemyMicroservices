using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models.PhotoStocks;
using FreeCourse.Web.Services.Interfaces;

namespace FreeCourse.Web.Services;

public class PhotoStockService : IPhotoStockService
{
    private readonly HttpClient _httpClient;

    public PhotoStockService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> DeletePhotoAsync(string photoUrl)
    {
        HttpResponseMessage response = await _httpClient.DeleteAsync($"photos?photoUrl={photoUrl}");

        return response.IsSuccessStatusCode;
    }

    public async Task<PhotoViewModel> UploadPhotoAsync(IFormFile photo)
    {
        if (photo == null || photo.Length == 0)
        {
            return null;
        }

        string randomFileName = $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";

        using MemoryStream ms = new();

        await photo.CopyToAsync(ms);

        MultipartFormDataContent multipartContent = new()
        {
            { new ByteArrayContent(ms.ToArray()), "photo", randomFileName }
        };

        HttpResponseMessage response = await _httpClient.PostAsync("photos", multipartContent);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        Response<PhotoViewModel>? responseSuccess = await response.Content.ReadFromJsonAsync<Response<PhotoViewModel>>();

        return responseSuccess.Data;
    }
}
