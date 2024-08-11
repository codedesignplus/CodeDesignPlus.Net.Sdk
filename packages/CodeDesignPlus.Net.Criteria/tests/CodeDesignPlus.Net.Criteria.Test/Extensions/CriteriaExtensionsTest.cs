using CodeDesignPlus.Net.Core.Abstractions.Models.Criteria;
using CodeDesignPlus.Net.Criteria.Test.Helpers;

namespace CodeDesignPlus.Net.Criteria.Test.Extensions;

public class CriteriaExtensionsTest
{
    private readonly List<Order> orders = OrdersData.GetOrders();

    [Fact]
    public void GetFilterExpression_ReturnsTrue_WhenCriteriaFiltersIsNull()
    {
        // Arrange
        var criteria = new MC.Criteria { Filters = null };

        // Act
        var filterExpression = criteria.GetFilterExpression<object>();

        // Assert
        Assert.True(filterExpression.Compile().Invoke(new object()));
    }

    [Fact]
    public void GetFilterExpression_ReturnsTrue_WhenCriteriaFiltersIsEmpty()
    {
        // Arrange
        var criteria = new MC.Criteria { Filters = "" };

        // Act
        var filterExpression = criteria.GetFilterExpression<object>();

        // Assert
        Assert.True(filterExpression.Compile().Invoke(new object()));
    }


    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void GetSortByExpression_ReturnsNull_WhenCriteriaOrderByIsNull(string? orderBy)
    {
        // Arrange
        var criteria = new MC.Criteria { OrderBy = orderBy };

        // Act
        var sortByExpression = criteria.GetSortByExpression<object>();

        // Assert
        Assert.Null(sortByExpression);
    }

    [Fact]
    public void GetSortByExpression_ReturnsNotNull_WhenCriteriaOrderByIsNotNull()
    {
        // Arrange
        var criteria = new MC.Criteria { OrderBy = "Name" };

        // Act
        var sortByExpression = criteria.GetSortByExpression<Order>();

        // Assert
        Assert.NotNull(sortByExpression);
    }

    [Fact]
    public void GetSortByExpression_ReturnsNotNull_WhenCriteriaOrderByIsNotNullAndDescending()
    {
        // Arrange
        var criteria = new MC.Criteria { OrderBy = "Name", OrderType = OrderTypes.Descending };

        // Act
        var sortByExpression = criteria.GetSortByExpression<Order>();

        // Assert
        Assert.NotNull(sortByExpression);
    }

    [Fact]
    public void GetFilterExpression_OperatorEqual_ReturnsNotNull()
    {
        // Arrange
        var orderExpected = this.orders.FirstOrDefault(x => x.Name == "Order 1");
        var criteria = new MC.Criteria { Filters = "Name=Order 1" };

        // Act
        var filterExpression = criteria.GetFilterExpression<Order>();
        var order = this.orders.AsQueryable().FirstOrDefault(filterExpression);

        // Assert
        Assert.NotNull(filterExpression);
        Assert.NotNull(order);
        Assert.Equal(orderExpected, order);
    }

    [Fact]
    public void GetFilterExpression_OperatorStartWith_ReturnsNotNull()
    {
        // Arrange
        var orderExpected = this.orders.Where(x => x.Name!.StartsWith("Order"));
        var criteria = new MC.Criteria { Filters = "Name^=Order" };

        // Act
        var filterExpression = criteria.GetFilterExpression<Order>();
        var order = this.orders.AsQueryable().Where(filterExpression);

        // Assert
        Assert.NotNull(filterExpression);
        Assert.NotNull(order);
        Assert.Equal(orderExpected, order);
    }

    [Fact]
    public void GetFilterExpression_OperatorEndWith_ReturnsNotNull()
    {
        // Arrange
        var orderExpected = this.orders.FirstOrDefault(x => x.Name!.EndsWith('1'));
        var criteria = new MC.Criteria { Filters = "Name$=1" };

        // Act
        var filterExpression = criteria.GetFilterExpression<Order>();
        var order = this.orders.AsQueryable().FirstOrDefault(filterExpression);

        // Assert
        Assert.NotNull(filterExpression);
        Assert.NotNull(order);
        Assert.Equal(orderExpected, order);
    }

    [Fact]
    public void GetFilterExpression_OperatorContains_ReturnsNotNull()
    {
        // Arrange
        var orderExpected = this.orders.Where(x => x.Name!.Contains("Order"));
        var criteria = new MC.Criteria { Filters = "Name~=der" };

        // Act
        var filterExpression = criteria.GetFilterExpression<Order>();
        var order = this.orders.AsQueryable().Where(filterExpression);

        // Assert
        Assert.NotNull(filterExpression);
        Assert.NotNull(order);
        Assert.Equal(orderExpected, order);
    }

    [Fact]
    public void GetFilterExpression_OperatorGreaterThan_ReturnsNotNull()
    {
        // Arrange
        var orderExpected = this.orders.Where(x => x.Total > 100);
        var criteria = new MC.Criteria { Filters = "Total>100" };

        // Act
        var filterExpression = criteria.GetFilterExpression<Order>();
        var order = this.orders.AsQueryable().Where(filterExpression);

        // Assert
        Assert.NotNull(filterExpression);
        Assert.NotNull(order);
        Assert.Equal(orderExpected, order);
    }

    [Fact]
    public void GetFilterExpression_OperatorGreaterThanOrEqual_ReturnsNotNull()
    {
        // Arrange
        var orderExpected = this.orders.Where(x => x.Total >= 100);
        var criteria = new MC.Criteria { Filters = "Total>=100" };

        // Act
        var filterExpression = criteria.GetFilterExpression<Order>();
        var order = this.orders.AsQueryable().Where(filterExpression);

        // Assert
        Assert.NotNull(filterExpression);
        Assert.NotNull(order);
        Assert.Equal(orderExpected, order);
    }

    [Fact]
    public void GetFilterExpression_OperatorLessThan_ReturnsNotNull()
    {
        // Arrange
        var orderExpected = this.orders.Where(x => x.Total < 100);
        var criteria = new MC.Criteria { Filters = "Total<100" };

        // Act
        var filterExpression = criteria.GetFilterExpression<Order>();
        var order = this.orders.AsQueryable().Where(filterExpression);

        // Assert
        Assert.NotNull(filterExpression);
        Assert.NotNull(order);
        Assert.Equal(orderExpected, order);
    }

    [Fact]
    public void GetFilterExpression_OperatorLessThanOrEqual_ReturnsNotNull()
    {
        // Arrange
        var orderExpected = this.orders.Where(x => x.Total <= 100);
        var criteria = new MC.Criteria { Filters = "Total<=100" };

        // Act
        var filterExpression = criteria.GetFilterExpression<Order>();
        var order = this.orders.AsQueryable().Where(filterExpression);

        // Assert
        Assert.NotNull(filterExpression);
        Assert.NotNull(order);
        Assert.Equal(orderExpected, order);
    }

    [Fact]
    public void GetFilterExpression_OperatorEqualWithDateTime_ReturnsNotNull()
    {
        // Arrange
        var date = new DateTime(2021, 1, 1);
        var orderExpected = this.orders.FirstOrDefault(x => x.CreatedAt == date);
        var criteria = new MC.Criteria { Filters = $"CreatedAt={date}" };

        // Act
        var filterExpression = criteria.GetFilterExpression<Order>();
        var order = this.orders.AsQueryable().FirstOrDefault(filterExpression);

        // Assert
        Assert.NotNull(filterExpression);
        Assert.NotNull(order);
        Assert.Equal(orderExpected, order);
    }

    [Fact]
    public void GetFilterExpression_InvalidOperator_ThrowsException()
    {
        // Arrange
        var criteria = new MC.Criteria { Filters = "Name!=Order 1" };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => criteria.GetFilterExpression<Order>());

        Assert.Equal("Instance property 'Name!' is not defined for type 'CodeDesignPlus.Net.Criteria.Test.Helpers.Models.Order' (Parameter 'propertyName')", exception.Message);
    }

    [Fact]
    public void GetFilterExpression_OperatorLoginAnd_ReturnsNotNull()
    {
        // Arrange
        var orderExpected = this.orders.Where(x => x.Name == "Order 1" && x.Total == 90);
        var criteria = new MC.Criteria { Filters = "Name=Order 1|and|Total=90" };

        // Act
        var filterExpression = criteria.GetFilterExpression<Order>();
        var order = this.orders.AsQueryable().Where(filterExpression);

        // Assert
        Assert.NotNull(filterExpression);
        Assert.NotNull(order);
        Assert.Equal(orderExpected, order);
    }

    [Fact]
    public void GetFilterExpression_OperatorLoginOr_ReturnsNotNull()
    {
        // Arrange
        var orderExpected = this.orders.Where(x => x.Name == "Order 1" || x.Total == 200);
        var criteria = new MC.Criteria { Filters = "Name=Order 1|or|Total=200" };

        // Act
        var filterExpression = criteria.GetFilterExpression<Order>();
        var order = this.orders.AsQueryable().Where(filterExpression);

        // Assert
        Assert.NotNull(filterExpression);
        Assert.NotNull(order);
        Assert.Equal(orderExpected, order);
    }

    [Fact]
    public void GetFilterExpression_ClientSubEntity_ReturnsNotNull()
    {
        // Arrange
        var orderExpected = this.orders.Where(x => x.Client!.Name == "Client 1.1");
        var criteria = new MC.Criteria { Filters = "Client.Name=Client 1.1" };

        // Act
        var filterExpression = criteria.GetFilterExpression<Order>();
        var order = this.orders.AsQueryable().Where(filterExpression);

        // Assert
        Assert.NotNull(filterExpression);
        Assert.NotNull(order);
        Assert.Equal(orderExpected, order);
    }

}