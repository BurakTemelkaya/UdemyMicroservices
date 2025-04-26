using FreeCourse.Shared.Services;
using FreeCourse.Web.Models.Catalog;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FreeCourse.Web.Controllers;

[Authorize]
public class CoursesController : Controller
{
    private readonly ICatalogService _catalogService;
    private readonly ISharedIdentityService _sharedIdentityService;

    public CoursesController(ICatalogService catalogService, ISharedIdentityService sharedIdentityService)
    {
        _catalogService = catalogService;
        _sharedIdentityService = sharedIdentityService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _catalogService.GetAllCourseByUserIdAsync(_sharedIdentityService.GetUserId));
    }

    public async Task<IActionResult> Create()
    {
        var categories = await _catalogService.GetAllCategoryAsync();
        ViewBag.categoryList = new SelectList(categories, "Id", "Name");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CourseCreateInput courseCreateInput)
    {
        var categories = await _catalogService.GetAllCategoryAsync();
        ViewBag.categoryList = new SelectList(categories, "Id", "Name");

        if (!ModelState.IsValid)
        {
            return View(courseCreateInput);
        }

        courseCreateInput.UserId = _sharedIdentityService.GetUserId;
        courseCreateInput.Picture = string.Empty;

        var result = await _catalogService.CreateCourseAsync(courseCreateInput);
        if (result)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.");

        return View(courseCreateInput);
    }

    public async Task<IActionResult> Update(string id)
    {
        CourseViewModel course = await _catalogService.GetByCourseIdAsync(id);

        if (course == null)
        {
            //mesaj göster
            return RedirectToAction(nameof(Index));
        }

        List<CategoryViewModel> categories = await _catalogService.GetAllCategoryAsync();
        ViewBag.categoryList = new SelectList(categories, "Id", "Name", course.CategoryId);

        var courseUpdateInput = new CourseUpdateInput
        {
            Id = course.Id,
            Name = course.Name,
            Description = course.Description,
            Price = course.Price,
            Picture = course.Picture,
            CategoryId = course.CategoryId,
            Feature = new FeatureViewModel
            {
                Duration = course.Feature.Duration
            },
            UserId = course.UserId
        };

        return View(courseUpdateInput);
    }

    [HttpPost]
    public async Task<IActionResult> Update(CourseUpdateInput courseUpdateInput)
    {
        var categories = await _catalogService.GetAllCategoryAsync();
        ViewBag.categoryList = new SelectList(categories, "Id", "Name", courseUpdateInput.CategoryId);

        if (!ModelState.IsValid)
        {
            return View(courseUpdateInput);
        }

        courseUpdateInput.UserId = _sharedIdentityService.GetUserId;

        var result = await _catalogService.UpdateCourseAsync(courseUpdateInput);
        if (result)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.");

        return View(courseUpdateInput);
    }

    public async Task<IActionResult> Delete(string id)
    {
        await _catalogService.DeleteCourseAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
