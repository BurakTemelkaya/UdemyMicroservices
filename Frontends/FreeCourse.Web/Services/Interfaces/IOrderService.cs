using FreeCourse.Web.Models.Orders;

namespace FreeCourse.Web.Services.Interfaces;

public interface IOrderService
{
    /// <summary>
    /// Senkron iletişim - direk olarak microServisine istek yapılacak.
    /// </summary>
    /// <param name="checkoutInfoInput"></param>
    /// <returns></returns>
    Task<OrderCreatedViewModel> CreateOrderAsync(CheckoutInfoInput checkoutInfoInput);
    /// <summary>
    /// Asenkron iletişim - sipariş bilgileri rabbitMQ'ya gönderilecek.
    /// </summary>
    /// <param name="checkoutInfoInput"></param>
    /// <returns></returns>
    Task<OrderSuspendViewModel> SuspendOrderAsync(CheckoutInfoInput checkoutInfoInput);
    Task<List<OrderViewModel>> GetOrderAsync();
}
