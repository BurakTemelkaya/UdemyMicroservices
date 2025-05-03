using FreeCourse.Services.Order.Infrastructure;
using FreeCourse.Shared.Messages;
using MassTransit;

namespace FreeCourse.Services.Order.Application.Consumers;

public class CreateOrderMessageCommandConsumer : IConsumer<CreateOrderMessageCommand>
{
    private readonly OrderDbContext _orderDbContext;

    public CreateOrderMessageCommandConsumer(OrderDbContext orderDbContext)
    {
        _orderDbContext = orderDbContext;
    }

    public async Task Consume(ConsumeContext<CreateOrderMessageCommand> context)
    {
        Domain.OrderAggregate.Address newAdress = new(context.Message.Province, context.Message.District, context.Message.Street, context.Message.ZipCode, context.Message.Line);

        Domain.OrderAggregate.Order newOrder = new(context.Message.BuyerId, newAdress);

        context.Message.OrderItems.ForEach(x =>
        {
            newOrder.AddOrderItem(x.ProductId, x.ProductName, x.PictureUrl, x.Price);
        });

        await _orderDbContext.AddAsync(newOrder);

        await _orderDbContext.SaveChangesAsync();
    }
}