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
        //1. yol senktron iletişim
        //var orderStatus = await _orderService.CreateOrderAsync(checkoutInfoInput);

        //2. yol asenkron iletişim
        OrderSuspendViewModel orderSuspend = await _orderService.SuspendOrderAsync(checkoutInfoInput);

        if (!orderSuspend.IsSuccessful)
        {
            ViewBag.Error = orderSuspend.Error;
            return View(checkoutInfoInput);
        }

        //1. yol senktron iletişim
        //return RedirectToAction(nameof(SuccessfulCheckout), new { orderId = orderSuspend.OrderId });

        //2. yol asenkron iletişim
        return RedirectToAction(nameof(SuccessfulCheckout), new { orderId = new Random().Next(1, 1000) });
    }

    public IActionResult SuccessfulCheckout(int orderId)
    {
        ViewBag.orderId = orderId;
        return View();
    }

    public async Task<IActionResult> CheckoutHistory()
    {
        return View(await _orderService.GetOrderAsync());
    }
}
