using FreeCourse.Services.Basket.Services;
using FreeCourse.Shared.Messages;
using MassTransit;

namespace FreeCourse.Services.Basket.Consumers;

public class CourseNameChangeEventConsumer : IConsumer<CourseNameChangedEvent>
{
    private readonly IBasketService _basketService;

    public CourseNameChangeEventConsumer(IBasketService basketService)
    {
        _basketService = basketService;
    }

    public async Task Consume(ConsumeContext<CourseNameChangedEvent> context)
    {
        Shared.Dtos.Response<ICollection<Dtos.BasketDto>> basketList = await _basketService.GetAllBasketAsync();

        if (basketList.Data != null)
        {
            foreach (Dtos.BasketDto basket in basketList.Data)
            {
                Dtos.BasketItemDto? courseItem = basket.BasketItems?.FirstOrDefault(x => x.CourseId == context.Message.CourseId);
                if (courseItem != null)
                {
                    courseItem.CourseName = context.Message.UpdatedName;
                    await _basketService.SaveOrUpdateAsync(basket);
                }
            }
        }
    }
}
