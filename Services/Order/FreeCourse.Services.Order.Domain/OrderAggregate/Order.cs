using FreeCourse.Services.Order.Domain.Core;

namespace FreeCourse.Services.Order.Domain.OrderAggregate;

/// <summary>
/// EF Core Features
/// --Owned Types
/// --Shadow Property
/// --Backing Field
/// </summary>
public class Order : Entity, IAggregateRoot
{
    public DateTime CreatedDate { get; private set; }
    public Address Address { get; private set; }
    public string BuyerId { get; private set; }

    private readonly List<OrderItem> _orderItems;

    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;

    public Order()
    {
        
    }

    public Order(string buyerId, Address address)
    {
        _orderItems = [];
        CreatedDate = DateTime.Now;
        BuyerId = buyerId;
        Address = address;
    }

    public void AddOrderItem(string productId, string productName, string pictureUrl, Decimal price)
    {
        bool existProduct = _orderItems.Any(x => x.ProductId == productId);
        if (!existProduct)
        {
            _orderItems.Add(new OrderItem(productId, productName, pictureUrl, price));
        }
    }

    public decimal GetTotalPrice => _orderItems.Sum(x => x.Price);

}
