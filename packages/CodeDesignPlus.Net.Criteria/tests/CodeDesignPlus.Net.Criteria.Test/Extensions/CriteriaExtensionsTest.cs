using CodeDesignPlus.Net.Core.Abstractions.Models.Criteria;
using CodeDesignPlus.Net.Criteria.Exceptions;
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
        var date = new LocalDate(2021, 1, 1).AtMidnight().InUtc().ToInstant();
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
    public void GetFilterExpression_OperatorEqualWithGuid_ReturnsNotNull()
    {
        // Arrange
        var code = Guid.Parse("9432915d-8418-4896-86f6-6d2e0d0160af");
        var orderExpected = this.orders.FirstOrDefault(x => x.Code == code);
        var criteria = new MC.Criteria { Filters = $"Code={code}" };

        // Act
        var filterExpression = criteria.GetFilterExpression<Order>();
        var order = this.orders.AsQueryable().FirstOrDefault(filterExpression);

        // Assert
        Assert.NotNull(filterExpression);
        Assert.NotNull(order);
        Assert.Equal(orderExpected, order);
    }

    [Fact]
    public void GetFilterExpression_InvalidProperty_ThrowsException()
    {
        // Arrange
        var criteria = new MC.Criteria { Filters = "InvalidProperty=Order 1" };

        // Act & Assert
        var exception = Assert.Throws<CriteriaException>(() => criteria.GetFilterExpression<Order>());

        Assert.Equal(CriteriaErrors.UnknownProperty.With("InvalidProperty").ToEnglish(), exception.Message);
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
    public void GetFilterExpression_ConditionsWithoutLogicalOperator_ThrowsCriteriaException()
    {
        // Arrange
        var criteria = new MC.Criteria { Filters = "Name=Order 1|Name!=Order 1" };

        // Act
        var exception = Assert.Throws<CriteriaException>(() => criteria.GetFilterExpression<Order>());

        // Assert
        Assert.Equal(CriteriaErrors.MissingLogicalOperator.With("Name").ToEnglish(), exception.Message);
    }

    [Fact]
    public void GetFilterExpression_ScopeAfterConditionWithoutLogicalOperator_ThrowsCriteriaException()
    {
        // Arrange
        var criteria = new MC.Criteria { Filters = "Name=Order 1|Total=200|and|Name=Order 2" };

        // Act & Assert
        Assert.Throws<CriteriaException>(() => criteria.GetFilterExpression<Order>());
    }

    [Fact]
    public void GetFilterExpression_FilterWithoutComparisonOperator_ThrowsInvalidCondition()
    {
        // Arrange
        var criteria = new MC.Criteria { Filters = "Name" };

        // Act
        var exception = Assert.Throws<CriteriaException>(() => criteria.GetFilterExpression<Order>());

        // Assert
        Assert.Equal(CriteriaErrors.InvalidCondition.Code, exception.Code);
    }

    [Fact]
    public void GetFilterExpression_ValueThatDoesNotFitTheType_ThrowsInvalidValue()
    {
        // Arrange
        var criteria = new MC.Criteria { Filters = "Total=abc" };

        // Act
        var exception = Assert.Throws<CriteriaException>(() => criteria.GetFilterExpression<Order>());

        // Assert
        Assert.Equal(CriteriaErrors.InvalidValue.With("abc", "Total").ToEnglish(), exception.Message);
    }

    [Fact]
    public void GetFilterExpression_InListValueThatDoesNotFitTheType_ThrowsInvalidValue()
    {
        // Arrange
        var criteria = new MC.Criteria { Filters = "Status@=Pending,Unknown" };

        // Act
        var exception = Assert.Throws<CriteriaException>(() => criteria.GetFilterExpression<Order>());

        // Assert
        Assert.Equal(CriteriaErrors.InvalidValue.With("Unknown", "Status").ToEnglish(), exception.Message);
    }

    [Fact]
    public void GetFilterExpression_OperatorNotSupportedByTheType_ThrowsUnsupportedOperator()
    {
        // Arrange
        var criteria = new MC.Criteria { Filters = "Total~=9" };

        // Act
        var exception = Assert.Throws<CriteriaException>(() => criteria.GetFilterExpression<Order>());

        // Assert
        Assert.Equal(CriteriaErrors.UnsupportedOperator.With("~=", "Total").ToEnglish(), exception.Message);
    }

    [Fact]
    public void GetSortByExpression_UnknownProperty_ThrowsUnknownProperty()
    {
        // Arrange
        var criteria = new MC.Criteria { OrderBy = "Missing" };

        // Act
        var exception = Assert.Throws<CriteriaException>(() => criteria.GetSortByExpression<Order>());

        // Assert
        Assert.Equal(CriteriaErrors.UnknownProperty.With("Missing").ToEnglish(), exception.Message);
    }

    [Fact]
    public void GetFilterExpression_ConditionFollowedByPartWithoutComparisonOperator_ThrowsInvalidCondition()
    {
        // Arrange
        // Before, "foo" vanished and the filter answered as Name=Order 1 (Kappali pendings/403).
        var criteria = new MC.Criteria { Filters = "Name=Order 1|foo" };

        // Act
        var exception = Assert.Throws<CriteriaException>(() => criteria.GetFilterExpression<Order>());

        // Assert
        Assert.Equal(CriteriaErrors.InvalidCondition.Code, exception.Code);
    }

    // With a text operator the value is the text the user typed: "null" is the word, not the absence of a value.
    [Theory]
    [InlineData("Name~=null")]
    [InlineData("Name^=null")]
    [InlineData("Name$=null")]
    public void GetFilterExpression_TextOperatorWithTheWordNull_SearchesTheText(string filters)
    {
        // Arrange
        var orders = OrdersData.GetOrders();
        orders[1].Name = "null";

        var criteria = new MC.Criteria { Filters = filters };

        // Act
        var result = orders.AsQueryable().Where(criteria.GetFilterExpression<Order>()).Select(x => x.Id);

        // Assert
        Assert.Equal([orders[1].Id], result);
    }

    // With = and != the word null keeps meaning "no value": it is how a screen asks for the records without a value.
    [Fact]
    public void GetFilterExpression_EqualsNull_ReturnsTheRecordsWithoutValue()
    {
        // Arrange
        var orders = OrdersData.GetOrders();
        orders[2].Name = null!;

        var criteria = new MC.Criteria { Filters = "Name=null" };

        // Act
        var result = orders.AsQueryable().Where(criteria.GetFilterExpression<Order>()).Select(x => x.Id);

        // Assert
        Assert.Equal([orders[2].Id], result);
    }

    [Fact]
    public void GetFilterExpression_TextOperatorOnANumber_ThrowsUnsupportedOperator()
    {
        // Arrange
        var criteria = new MC.Criteria { Filters = "Total^=null" };

        // Act
        var exception = Assert.Throws<CriteriaException>(() => criteria.GetFilterExpression<Order>());

        // Assert
        Assert.Equal(CriteriaErrors.UnsupportedOperator.With("^=", "Total").ToEnglish(), exception.Message);
    }

    [Fact]
    public void GetFilterExpression_OrFollowedByAnd_GroupsFromTheLeft()
    {
        // Arrange
        // Left-associative on purpose: clients append a scope with |and| at the end of a filter that may carry |or|.
        var orderExpected = this.orders.Where(x => (x.Name == "Order 1" || x.Name == "Order 2") && x.Total == 200);
        var criteria = new MC.Criteria { Filters = "Name=Order 1|or|Name=Order 2|and|Total=200" };

        // Act
        var filterExpression = criteria.GetFilterExpression<Order>();
        var order = this.orders.AsQueryable().Where(filterExpression).Select(x => x.Name);

        // Assert
        Assert.Equal(orderExpected.Select(x => x.Name), order);
    }

    [Fact]
    public void GetFilterExpression_AndFollowedByOr_GroupsFromTheLeft()
    {
        // Arrange
        var orderExpected = this.orders.Where(x => (x.Total == 90 && x.Name == "Order 2") || x.Total == 300);
        var criteria = new MC.Criteria { Filters = "Total=90|and|Name=Order 2|or|Total=300" };

        // Act
        var filterExpression = criteria.GetFilterExpression<Order>();
        var order = this.orders.AsQueryable().Where(filterExpression).Select(x => x.Name);

        // Assert
        Assert.Equal(orderExpected.Select(x => x.Name), order);
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