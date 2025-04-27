using FreeCourse.Shared.Dtos;
using FreeCourse.Shared.Services;
using FreeCourse.Web.Models.Baskets;
using FreeCourse.Web.Models.FakePayment;
using FreeCourse.Web.Models.Orders;
using FreeCourse.Web.Services.Interfaces;

namespace FreeCourse.Web.Services;

public class OrderService : IOrderService
{
    private readonly IPaymentService _paymentService;
    private readonly HttpClient _httpClient;
    private readonly IBasketService _basketService;
    private readonly ISharedIdentityService _sharedIdentityService;

    public OrderService(HttpClient httpClient, IPaymentService paymentService, IBasketService basketService,ISharedIdentityService sharedIdentityService)
    {
        _httpClient = httpClient;
        _paymentService = paymentService;
        _basketService = basketService;
        _sharedIdentityService = sharedIdentityService;
    }

    public async Task<OrderCreatedViewModel> CreateOrderAsync(CheckoutInfoInput checkoutInfoInput)
    {
        BasketViewModel basket = await _basketService.GetAsync();

        PaymentInfoInput paymentInfoInput = new()
        {
            CardName = checkoutInfoInput.CardName,
            CardNumber = checkoutInfoInput.CardNumber,
            Expiration = checkoutInfoInput.Expiration,
            CVV = checkoutInfoInput.CVV,
            TotalPrice = basket.TotalPrice,
        };

        bool paymentResult = await _paymentService.ReceivePaymentAsync(paymentInfoInput);

        if (!paymentResult)
        {
            return new OrderCreatedViewModel
            {
                Error = "Ödeme alınamadı.",
                IsSuccessful = false,
            };
        }

        OrderCreateInput orderCreateInput = new()
        {
            BuyerId = _sharedIdentityService.GetUserId,
            Address = new AddressCreateInput
            {
                Province = checkoutInfoInput.Province,
                District = checkoutInfoInput.District,
                Street = checkoutInfoInput.Street,
                ZipCode = checkoutInfoInput.ZipCode,
                Line = checkoutInfoInput.Line,
            },
        };

        basket.BasketItems.ForEach(x =>
        {
            OrderItemCreateInput orderItem = new()
            {
                ProductId = x.CourseId,
                Price = x.GetCurrentPrice,
                PictureUrl = string.Empty,
                ProductName = x.CourseName,
            };

            orderCreateInput.OrderItems.Add(orderItem);
        });

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync<OrderCreateInput>("orders", orderCreateInput);

        if (!response.IsSuccessStatusCode)
        {
            return new OrderCreatedViewModel
            {
                Error = "Sipariş oluşturulamadı.",
                IsSuccessful = false,
            };
        }

        var responseString = await response.Content.ReadAsStringAsync();

        Response<OrderCreatedViewModel>? orderCreatedViewModel = await response.Content.ReadFromJsonAsync<Response<OrderCreatedViewModel>>();

        orderCreatedViewModel.Data.IsSuccessful = true;

        await _basketService.DeleteAsync();

        return orderCreatedViewModel.Data;

    }

    public async Task<List<OrderViewModel>> GetOrderAsync()
    {
        Response<List<OrderViewModel>>? response = await _httpClient.GetFromJsonAsync<Response<List<OrderViewModel>>>("orders");

        return response.Data;
    }

    public async Task SuspendOrderAsync(CheckoutInfoInput checkoutInfoInput)
    {
        throw new NotImplementedException();
    }
}
