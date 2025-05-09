﻿namespace FreeCourse.Web.Models.Orders;

public class OrderCreateInput
{
    public OrderCreateInput()
    {
        OrderItems = [];
    }

    public string BuyerId { get; set; }
    public List<OrderItemCreateInput> OrderItems { get; set; }
    public AddressCreateInput Address { get; set; }
}
