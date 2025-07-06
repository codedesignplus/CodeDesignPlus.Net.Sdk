using CodeDesignPlus.Net.Core.Abstractions.Models.Pager;

namespace CodeDesignPlus.Net.Core.Test.Models.Pager;

public class PaginationTest
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        // Arrange
        var data = new List<string> { "a", "b", "c" };
        long totalCount = 100;
        int limit = 10;
        int skip = 20;

        // Act
        var pagination = new Pagination<string>(data, totalCount, limit, skip);

        // Assert
        Assert.Equal(data, pagination.Data);
        Assert.Equal(totalCount, pagination.TotalCount);
        Assert.Equal(limit, pagination.Limit);
        Assert.Equal(skip, pagination.Skip);
    }

    [Fact]
    public void Create_ShouldSetProperties_WithAllParameters()
    {
        // Arrange
        var data = new List<int> { 1, 2, 3 };
        long totalCount = 50;
        int? limit = 5;
        int? skip = 10;

        // Act
        var pagination = Pagination<int>.Create(data, totalCount, limit, skip);

        // Assert
        Assert.Equal(data, pagination.Data);
        Assert.Equal(totalCount, pagination.TotalCount);
        Assert.Equal(limit, pagination.Limit);
        Assert.Equal(skip, pagination.Skip);
    }

    [Fact]
    public void Create_ShouldUseDefaultLimitAndSkip_WhenNull()
    {
        // Arrange
        var data = new List<int> { 1, 2 };
        long totalCount = 20;

        // Act
        var pagination = Pagination<int>.Create(data, totalCount, null, null);

        // Assert
        Assert.Equal(data, pagination.Data);
        Assert.Equal(totalCount, pagination.TotalCount);
        Assert.Equal(10, pagination.Limit); // default
        Assert.Equal(0, pagination.Skip);   // default
    }

    [Fact]
    public void Data_ShouldBeEnumerable()
    {
        // Arrange
        var data = Enumerable.Range(1, 3);
        var pagination = new Pagination<int>(data, 3, 3, 0);

        // Act & Assert
        Assert.True(pagination.Data.SequenceEqual(data));
    }
}
