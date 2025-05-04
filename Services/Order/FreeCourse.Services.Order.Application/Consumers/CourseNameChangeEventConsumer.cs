using FreeCourse.Services.Order.Infrastructure;
using FreeCourse.Shared.Messages;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace FreeCourse.Services.Order.Application.Consumers;

public class CourseNameChangeEventConsumer : IConsumer<CourseNameChangedEvent>
{
    private readonly OrderDbContext _orderDbContext;

    public CourseNameChangeEventConsumer(OrderDbContext orderDbContext)
    {
        _orderDbContext = orderDbContext;
    }

    public async Task Consume(ConsumeContext<CourseNameChangedEvent> context)
    {
        List<Domain.OrderAggregate.OrderItem> orderItems = await _orderDbContext.OrderItems.Where(x => x.ProductId == context.Message.CourseId).ToListAsync();

        if (orderItems.Any())
        {
            orderItems.ForEach(x => x.UpdateOrderItem(context.Message.UpdatedName, x.PictureUrl, x.Price));
            await _orderDbContext.SaveChangesAsync();
        }

        await _orderDbContext.SaveChangesAsync();
    }
}
