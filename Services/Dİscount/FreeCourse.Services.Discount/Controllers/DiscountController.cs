using FreeCourse.Services.Discount.Models;
using FreeCourse.Services.Discount.Services;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.Dtos;
using FreeCourse.Shared.Services;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Services.Discount.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DiscountController : CustomBaseController
{
    private readonly IDiscountService _discountService;
    private readonly ISharedIdentityService _sharedIdentityService;

    public DiscountController(IDiscountService discountService,ISharedIdentityService sharedIdentityService)
    {
        _discountService = discountService;
        _sharedIdentityService = sharedIdentityService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return CreateActionResultInstance(await _discountService.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        Response<Models.Discount> discount = await _discountService.GetByIdAsync(id);
        return CreateActionResultInstance(discount);
    }

    [HttpGet]
    [Route("/api/[controller]/[action]/{code}")]
    public async Task<IActionResult> GetByCode(string code)
    {
        string userId = _sharedIdentityService.GetUserId;
        Response<Models.Discount> discount = await _discountService.GetByCodeAndUserIdAsync(code, userId);
        return CreateActionResultInstance(discount);
    }

    [HttpPost]
    public async Task<IActionResult> Save(Models.Discount discount)
    {
        Response<NoContent> discountResponse = await _discountService.SaveAsync(discount);
        return CreateActionResultInstance(discountResponse);
    }

    [HttpPut]
    public async Task<IActionResult> Update(Models.Discount discount)
    {
        Response<NoContent> discountResponse = await _discountService.UpdateAsync(discount);
        return CreateActionResultInstance(discountResponse);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        Response<NoContent> discountResponse = await _discountService.DeleteAsync(id);
        return CreateActionResultInstance(discountResponse);
    }
}
