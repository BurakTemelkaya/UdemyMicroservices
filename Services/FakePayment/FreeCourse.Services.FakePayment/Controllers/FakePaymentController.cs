using FreeCourse.Services.FakePayment.Models;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.Dtos;
using FreeCourse.Shared.Messages;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Services.FakePayment.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FakePaymentController : CustomBaseController
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public FakePaymentController(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }

    [HttpPost]
    public async Task<IActionResult> ReceivePayment(PaymentDto paymentDto)
    {
        ISendEndpoint sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:create-order-service"));

        CreateOrderMessageCommand createOrderMessageCommand = new()
        {
            BuyerId = paymentDto.Order.BuyerId,
            District = paymentDto.Order.Address.District,
            Province = paymentDto.Order.Address.Province,
            Street = paymentDto.Order.Address.Street,
            ZipCode = paymentDto.Order.Address.ZipCode,
            Line = paymentDto.Order.Address.Line,
            OrderItems = [.. paymentDto.Order.OrderItems.Select(x => new OrderItem
            {
                ProductId = x.ProductId,
                Price = x.Price,
                PictureUrl = x.PictureUrl,
                ProductName = x.ProductName
            })]
        };

        await sendEndpoint.Send<CreateOrderMessageCommand>(createOrderMessageCommand);
        
        return CreateActionResultInstance(Shared.Dtos.Response<NoContent>.Success(StatusCodes.Status204NoContent));
    }
}
