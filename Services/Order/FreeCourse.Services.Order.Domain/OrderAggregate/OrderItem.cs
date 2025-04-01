using FreeCourse.Services.Order.Domain.Core;

namespace FreeCourse.Services.Order.Domain.OrderAggregate;

public class OrderItem : Entity
{
    public string ProductId { get; private set; }
    public string ProductName { get; private set; }
    public string PictureUrl { get; private set; }
    public Decimal Price { get; private set; }

    public OrderItem()
    {
        
    }

    public OrderItem(string productId, string productName, string pictureUrl, Decimal price)
    {
        ProductId = productId;
        ProductName = productName;
        PictureUrl = pictureUrl;
        Price = price;
    }

    public void UpdateOrderItem(string productName, string pictureUrl, Decimal price)
    {
        ProductName = productName;
        PictureUrl = pictureUrl;
        Price = price;
    }

    protected IEnumerable<object> GetEqualityComponents()
    {
        yield return ProductId;
        yield return ProductName;
        yield return PictureUrl;
        yield return Price;
    }
}
