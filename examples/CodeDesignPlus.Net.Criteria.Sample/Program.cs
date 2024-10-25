// See https://aka.ms/new-console-template for more information
using CodeDesignPlus.Net.Core.Abstractions.Models.Criteria;
using CodeDesignPlus.Net.Criteria.Extensions;
using CodeDesignPlus.Net.Criteria.Sample;
using CodeDesignPlus.Net.Criteria.Sample.Models;

Console.WriteLine("This example shows how to use the Criteria class to filter a list of orders.");

var expressionAllOrders = "Name~=Order|and|Total>100|client.email$=outlook.com";

var criteria = new Criteria()
{
    Filters = expressionAllOrders,
    OrderBy = "Total",
    OrderType = OrderTypes.Descending
};

var lamda = criteria.GetFilterExpression<Order>();
var sort = criteria.GetSortByExpression<Order>();

var orders = OrdersData.GetOrders().AsQueryable().Where(lamda);

if(criteria.OrderType == OrderTypes.Ascending)
    orders = orders.OrderBy(sort);
else
    orders = orders.OrderByDescending(sort);

foreach (var order in orders)
{
    Console.WriteLine($"Order: {order.Name}");
}
