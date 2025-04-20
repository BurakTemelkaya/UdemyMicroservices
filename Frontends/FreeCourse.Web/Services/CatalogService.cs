using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Helpers;
using FreeCourse.Web.Models;
using FreeCourse.Web.Models.Catalog;
using FreeCourse.Web.Models.PhotoStocks;
using FreeCourse.Web.Services.Interfaces;

namespace FreeCourse.Web.Services;

public class CatalogService : ICatalogService
{
    
    private readonly HttpClient _httpClient;
    private readonly IPhotoStockService _photoStockService;
    private readonly PhotoHelper _photoHelper;

    public CatalogService(HttpClient httpClient, IPhotoStockService photoStockService,PhotoHelper photoHelper)
    {
        _httpClient = httpClient;
        _photoStockService = photoStockService;
        _photoHelper = photoHelper;
    }

    public async Task<bool> CreateCourseAsync(CourseCreateInput courseCreateInput)
    {
        PhotoViewModel resultPhoto = await _photoStockService.UploadPhotoAsync(courseCreateInput.PhotoFormFile);

        if (resultPhoto != null)
        {
            courseCreateInput.Picture = resultPhoto.Url;
        }

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync<CourseCreateInput>("courses", courseCreateInput);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteCourseAsync(string courseId)
    {
        HttpResponseMessage response = await _httpClient.DeleteAsync($"courses/{courseId}");

        return response.IsSuccessStatusCode;
    }

    public async Task<List<CategoryViewModel>> GetAllCategoryAsync()
    {
        HttpResponseMessage response = await _httpClient.GetAsync("categories");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        Response<List<CategoryViewModel>>? responeSuccess = await response.Content.ReadFromJsonAsync<Response<List<CategoryViewModel>>>();

        return responeSuccess.Data;
    }

    public async Task<List<CourseViewModel>> GetAllCourseAsync()
    {
        //http:localhost:5000/services/catalog/courses
        HttpResponseMessage response = await _httpClient.GetAsync("courses");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        Response<List<CourseViewModel>>? responeSuccess = await response.Content.ReadFromJsonAsync<Response<List<CourseViewModel>>>();

        responeSuccess!.Data!.ForEach(x =>
        {
            x.StockPictureUrl = _photoHelper.GetPhotoStockUrl(x.Picture);
        });

        return responeSuccess.Data;
    }

    public async Task<List<CourseViewModel>> GetAllCourseByUserIdAsync(string userId)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"courses/GetAllByUserId/{userId}");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        Response<List<CourseViewModel>>? responeSuccess = await response.Content.ReadFromJsonAsync<Response<List<CourseViewModel>>>();

        responeSuccess!.Data!.ForEach(x =>
        {
            x.StockPictureUrl = _photoHelper.GetPhotoStockUrl(x.Picture);
        });

        return responeSuccess.Data;
    }

    public async Task<CourseViewModel> GetByCourseIdAsync(string courseId)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"courses/{courseId}");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        Response<CourseViewModel>? responeSuccess = await response.Content.ReadFromJsonAsync<Response<CourseViewModel>>();

        responeSuccess.Data.StockPictureUrl = _photoHelper.GetPhotoStockUrl(responeSuccess.Data.Picture);       

        return responeSuccess.Data;
    }

    public async Task<bool> UpdateCourseAsync(CourseUpdateInput courseUpdateInput)
    {
        PhotoViewModel resultPhoto = await _photoStockService.UploadPhotoAsync(courseUpdateInput.PhotoFormFile);

        if (resultPhoto != null)
        {
            await _photoStockService.DeletePhotoAsync(courseUpdateInput.Picture);
            courseUpdateInput.Picture = resultPhoto.Url;
        }

        HttpResponseMessage response = await _httpClient.PutAsJsonAsync<CourseUpdateInput>("courses", courseUpdateInput);

        return response.IsSuccessStatusCode;
    }
}
