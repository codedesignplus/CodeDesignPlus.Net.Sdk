using C = CodeDesignPlus.Net.Core.Abstractions.Models.Criteria;

namespace CodeDesignPlus.Net.Core.Test.Models.Criteria;

public class CriteriaTest
{
    [Fact]
    public void Criteria_Filters_SetCorrectly()
    {
        // Arrange
        var criteria = new C.Criteria();
        var filters = "some filters";

        // Act
        criteria.Filters = filters;

        // Assert
        Assert.Equal(filters, criteria.Filters);
    }

    [Fact]
    public void Criteria_OrderBy_SetCorrectly()
    {
        // Arrange
        var criteria = new C.Criteria();
        var orderBy = "some field";

        // Act
        criteria.OrderBy = orderBy;

        // Assert
        Assert.Equal(orderBy, criteria.OrderBy);
    }

    [Fact]
    public void Criteria_OrderType_SetCorrectly()
    {
        // Arrange
        var criteria = new C.Criteria();
        var orderType = C.OrderTypes.Ascending;

        // Act
        criteria.OrderType = orderType;

        // Assert
        Assert.Equal(orderType, criteria.OrderType);
    }

    [Fact]
    public void Criteria_Limit_SetCorrectly()
    {
        // Arrange
        var criteria = new C.Criteria();
        var limit = 10;

        // Act
        criteria.Limit = limit;

        // Assert
        Assert.Equal(limit, criteria.Limit);
    }

    [Fact]
    public void Criteria_Skip_SetCorrectly()
    {
        // Arrange
        var criteria = new C.Criteria();
        var skip = 5;

        // Act
        criteria.Skip = skip;

        // Assert
        Assert.Equal(skip, criteria.Skip);
    }
}
