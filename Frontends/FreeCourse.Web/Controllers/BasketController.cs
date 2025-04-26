using FreeCourse.Web.Models.Baskets;
using FreeCourse.Web.Models.Discount;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Web.Controllers;

[Authorize]
public class BasketController : Controller
{
    private readonly ICatalogService _catalogService;
    private readonly IBasketService _basketService;

    public BasketController(ICatalogService catalogService, IBasketService basketService)
    {
        _catalogService = catalogService;
        _basketService = basketService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _basketService.GetAsync());
    }

    public async Task<IActionResult> AddBasketItem(string courseId)
    {
        var course = await _catalogService.GetByCourseIdAsync(courseId);

        BasketItemViewModel basketItem = new()
        {
            CourseId = course.Id,
            CourseName = course.Name,
            Price = course.Price,
        };

        await _basketService.AddBasketItemAsync(basketItem);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> RemoveBasketItem(string courseId)
    {
        await _basketService.RemoveBasketItemAsync(courseId);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> ApplyDiscount(DiscountApplyInput discountApplyInput)
    {
        if (!ModelState.IsValid)
        {
            TempData["discountError"] = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).First();
            return RedirectToAction(nameof(Index));
        }

        bool discountStatus = await _basketService.ApplyDiscountAsync(discountApplyInput.Code);

        TempData["discountStatus"] = discountStatus;

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> CancelApplyDiscount()
    {
        await _basketService.CancelApplyDiscountAsync();

        return RedirectToAction(nameof(Index));
    }
}
