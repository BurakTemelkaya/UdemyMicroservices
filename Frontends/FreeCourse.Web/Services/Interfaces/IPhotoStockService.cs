using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models.PhotoStocks;

namespace FreeCourse.Web.Services.Interfaces;

public interface IPhotoStockService
{
    Task<PhotoViewModel> UploadPhotoAsync(IFormFile photo);
    Task<bool> DeletePhotoAsync(string photoUrl);
}
