using CodeDesignPlus.Net.Core.Abstractions.Models.Criteria;
using CodeDesignPlus.Net.Criteria.Extensions;
using CodeDesignPlus.Net.Criteria.Sample;
using CodeDesignPlus.Net.Criteria.Sample.Models;

var data = OrdersData.GetOrders();

Console.WriteLine("=== CodeDesignPlus.Net.Criteria — Sample ===\n");
Console.WriteLine($"Total orders in dataset: {data.Count}\n");

// 1. Contains (~=) + AND + Greater Than
Console.WriteLine("--- 1. Contains (~=) + AND + Greater Than (>) ---");
Console.WriteLine("    Expression: Name~=Order|and|Total>100|client.email$=outlook.com");
var criteria1 = new Criteria
{
    Filters = "Name~=Order|and|Total>100|client.email$=outlook.com",
    OrderBy = "Total",
    OrderType = OrderTypes.Descending
};

var expr1 = criteria1.GetFilterExpression<Order>();
var result1 = data.AsQueryable().Where(expr1).ToList();
foreach (var order in result1)
    Console.WriteLine($"    -> {order.Name} (Total: {order.Total}, Status: {order.Status})");

// 2. Not Equal (!=)
Console.WriteLine("\n--- 2. Not Equal (!=) ---");
Console.WriteLine("    Expression: Status!=Cancelled");
var criteria2 = new Criteria { Filters = "Status!=Cancelled" };
var expr2 = criteria2.GetFilterExpression<Order>();
var result2 = data.AsQueryable().Where(expr2).ToList();
foreach (var order in result2)
    Console.WriteLine($"    -> {order.Name} (Status: {order.Status})");

// 3. In operator (@=)
Console.WriteLine("\n--- 3. In Operator (@=) ---");
Console.WriteLine("    Expression: Status@=Pending,Processing");
var criteria3 = new Criteria { Filters = "Status@=Pending,Processing" };
var expr3 = criteria3.GetFilterExpression<Order>();
var result3 = data.AsQueryable().Where(expr3).ToList();
foreach (var order in result3)
    Console.WriteLine($"    -> {order.Name} (Status: {order.Status})");

// 4. Null equality (=null)
Console.WriteLine("\n--- 4. Null Equality (=null) ---");
Console.WriteLine("    Expression: Description=null");
var criteria4 = new Criteria { Filters = "Description=null" };
var expr4 = criteria4.GetFilterExpression<Order>();
var result4 = data.AsQueryable().Where(expr4).ToList();
foreach (var order in result4)
    Console.WriteLine($"    -> {order.Name} (Description: {order.Description ?? "null"})");

// 5. Null inequality (!=null)
Console.WriteLine("\n--- 5. Null Inequality (!=null) ---");
Console.WriteLine("    Expression: Description!=null");
var criteria5 = new Criteria { Filters = "Description!=null" };
var expr5 = criteria5.GetFilterExpression<Order>();
var result5 = data.AsQueryable().Where(expr5).ToList();
foreach (var order in result5)
    Console.WriteLine($"    -> {order.Name} (Description: {order.Description})");

// 6. Boolean filter
Console.WriteLine("\n--- 6. Boolean Filter ---");
Console.WriteLine("    Expression: IsActive=true");
var criteria6 = new Criteria { Filters = "IsActive=true" };
var expr6 = criteria6.GetFilterExpression<Order>();
var result6 = data.AsQueryable().Where(expr6).ToList();
foreach (var order in result6)
    Console.WriteLine($"    -> {order.Name} (IsActive: {order.IsActive})");

// 7. StartsWith (^=)
Console.WriteLine("\n--- 7. StartsWith (^=) ---");
Console.WriteLine("    Expression: Name^=Order 1");
var criteria7 = new Criteria { Filters = "Name^=Order 1" };
var expr7 = criteria7.GetFilterExpression<Order>();
var result7 = data.AsQueryable().Where(expr7).ToList();
foreach (var order in result7)
    Console.WriteLine($"    -> {order.Name}");

// 8. EndsWith ($=)
Console.WriteLine("\n--- 8. EndsWith ($=) ---");
Console.WriteLine("    Expression: Name$=3");
var criteria8 = new Criteria { Filters = "Name$=3" };
var expr8 = criteria8.GetFilterExpression<Order>();
var result8 = data.AsQueryable().Where(expr8).ToList();
foreach (var order in result8)
    Console.WriteLine($"    -> {order.Name}");

// 9. Combined: In + Boolean + Not Equal
Console.WriteLine("\n--- 9. Combined: In (@=) + AND + Boolean ---");
Console.WriteLine("    Expression: Status@=Pending,Processing,Completed|and|IsActive=true");
var criteria9 = new Criteria { Filters = "Status@=Pending,Processing,Completed|and|IsActive=true" };
var expr9 = criteria9.GetFilterExpression<Order>();
var result9 = data.AsQueryable().Where(expr9).ToList();
foreach (var order in result9)
    Console.WriteLine($"    -> {order.Name} (Status: {order.Status}, IsActive: {order.IsActive})");

// 10. Find by Id (Guid)
Console.WriteLine("\n--- 10. Find By Id (Guid =) ---");
var firstOrder = data.First();
Console.WriteLine($"    Expression: Id={firstOrder.Id}");
var criteria10 = new Criteria { Filters = $"Id={firstOrder.Id}" };
var expr10 = criteria10.GetFilterExpression<Order>();
var result10 = data.AsQueryable().Where(expr10).FirstOrDefault();
Console.WriteLine($"    -> {result10?.Name}");

// 11. OR operator
Console.WriteLine("\n--- 11. OR Operator ---");
Console.WriteLine("    Expression: Total<100|or|Total>250");
var criteria11 = new Criteria { Filters = "Total<100|or|Total>250" };
var expr11 = criteria11.GetFilterExpression<Order>();
var result11 = data.AsQueryable().Where(expr11).ToList();
foreach (var order in result11)
    Console.WriteLine($"    -> {order.Name} (Total: {order.Total})");

Console.WriteLine("\n=== All operators demonstrated successfully! ===");
Console.WriteLine("\nSupported operators:");
Console.WriteLine("  =   Equal");
Console.WriteLine("  !=  Not Equal");
Console.WriteLine("  ~=  Contains (case-insensitive in MongoDB)");
Console.WriteLine("  ^=  StartsWith");
Console.WriteLine("  $=  EndsWith");
Console.WriteLine("  @=  In (comma-separated values)");
Console.WriteLine("  <   Less Than");
Console.WriteLine("  >   Greater Than");
Console.WriteLine("  <=  Less Than or Equal");
Console.WriteLine("  >=  Greater Than or Equal");
Console.WriteLine("\nSpecial values:");
Console.WriteLine("  null   — matches null/empty fields");
Console.WriteLine("  true/false — boolean literals (culture-invariant)");
Console.WriteLine("\nLogical operators:");
Console.WriteLine("  |and|  — logical AND");
Console.WriteLine("  |or|   — logical OR");
