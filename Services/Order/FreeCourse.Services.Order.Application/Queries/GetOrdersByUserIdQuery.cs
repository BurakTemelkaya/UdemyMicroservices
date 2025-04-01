using FreeCourse.Services.Order.Application.Dtos;
using MediatR;
using FreeCourse.Shared.Dtos;

namespace FreeCourse.Services.Order.Application.Queries;

public class GetOrdersByUserIdQuery : IRequest<Response<List<OrderDto>>>
{
    public string UserId { get; set; }
}
