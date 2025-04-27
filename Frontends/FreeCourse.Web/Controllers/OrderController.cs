using FreeCourse.Web.Models.Baskets;
using FreeCourse.Web.Models.Orders;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Web.Controllers;

public class OrderController : Controller
{
    private readonly IBasketService _basketService;
    private readonly IOrderService _orderService;

    public OrderController(IBasketService basketService, IOrderService orderService)
    {
        _basketService = basketService;
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        BasketViewModel basket = await _basketService.GetAsync();

        ViewBag.basket = basket;

        return View(new CheckoutInfoInput());
    }

    [HttpPost]
    public async Task<IActionResult> Checkout(CheckoutInfoInput checkoutInfoInput)
    {
        var orderStatus = await _orderService.CreateOrderAsync(checkoutInfoInput);

        if (!orderStatus.IsSuccessful)
        {
            ViewBag.Error = orderStatus.Error;
            return View(checkoutInfoInput);
        }

        return RedirectToAction(nameof(SuccessfulCheckout), new { orderId = orderStatus.OrderId });
    }

    public IActionResult SuccessfulCheckout(int orderId)
    {
        ViewBag.orderId = orderId;
        return View();
    }
}
