using FreeCourse.Services.PhotoStock.Dtos;
using FreeCourse.Shared.ControllerBases;
using Microsoft.AspNetCore.Mvc;
using FreeCourse.Shared.Dtos;

namespace FreeCourse.Services.PhotoStock.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PhotosController : CustomBaseController
{
    [HttpPost]
    public async Task<IActionResult> PhotoSave(IFormFile photo, CancellationToken cancellationToken)
    {
        if (photo == null || photo.Length == 0)
        {
            return CreateActionResultInstance(Response<PhotoDto>.Fail("photo is empty", StatusCodes.Status400BadRequest));
        }

        string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/photos", photo.FileName);

        using FileStream stream = new(path, FileMode.Create);

        await photo.CopyToAsync(stream, cancellationToken);

        PhotoDto photoDto = new() { Url = photo.FileName };

        return CreateActionResultInstance(Response<PhotoDto>.Success(photoDto, StatusCodes.Status201Created));
    }

    [HttpDelete]
    public IActionResult PhotoDelete(string photoUrl)
    {
        string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/photos", photoUrl);

        if (!System.IO.File.Exists(path))
        {
            return CreateActionResultInstance(Response<NoContent>.Fail("photo not found", StatusCodes.Status404NotFound));
        }
        else
        {
            System.IO.File.Delete(path);
            return CreateActionResultInstance(Response<NoContent>.Success(StatusCodes.Status204NoContent));
        }
    }
}
