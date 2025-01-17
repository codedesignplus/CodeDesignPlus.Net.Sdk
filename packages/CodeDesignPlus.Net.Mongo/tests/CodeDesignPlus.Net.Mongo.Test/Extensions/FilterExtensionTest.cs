using CodeDesignPlus.Net.Mongo.Extensions;
using CodeDesignPlus.Net.Mongo.Test.Helpers.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace CodeDesignPlus.Net.Mongo.Test.Extensions;
public class FilterExtensionTests
{

    [Fact]
    public void BuildFilter_AggregateRoot_ValidTenant_ReturnsFilterWithTenant()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var filter = Builders<UserAggregate>.Filter.Empty;

        // Act
        var result = filter.BuildFilter(tenantId);

        // Assert
        var expectedFilter = Builders<UserAggregate>.Filter.And(
            Builders<UserAggregate>.Filter.Empty,
            Builders<UserAggregate>.Filter.Eq(e => e.Tenant, tenantId)
        );

        Assert.Equal(expectedFilter.ToJson(), result.ToJson());
    }

    [Fact]
    public void BuildFilter_AggregateRoot_EmptyTenant_ThrowsMongoException()
    {
        // Arrange
        var tenantId = Guid.Empty;
        var filter = Builders<UserAggregate>.Filter.Empty;

        // Act & Assert
        Assert.Throws<Mongo.Exceptions.MongoException>(() => filter.BuildFilter(tenantId));
    }


    [Fact]
    public void BuildFilter_NonAggregateRoot_ReturnsOriginalFilter()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var filter = Builders<ProductEntity>.Filter.Empty;
        var renderArgs = new RenderArgs<ProductEntity>();
        // Act
        var result = filter.BuildFilter(tenantId);

        // Assert
        Assert.Equal(filter.Render(renderArgs), result.Render(renderArgs));
    }

    [Fact]
    public void ToFilterDefinition_ExpressionWithAggregation_ReturnsBsonDocumentFilterDefinition()
    {
        // Arrange
        Expression<Func<UserAggregate, bool>> expression = x => x.Name == "Test";

        // Act
        var result = expression.ToFilterDefinition(true);
        var filter = ((BsonDocumentFilterDefinition<UserAggregate>)result).Document.ToString();

        // Assert
        Assert.IsType<BsonDocumentFilterDefinition<UserAggregate>>(result);
        Assert.Equal("{ \"$eq\" : [\"$$entity.Name\", \"Test\"] }", filter);
    }

    [Fact]
    public void ToFilterDefinition_ExpressionWithoutAggregation_ReturnsBsonDocumentFilterDefinition()
    {
        // Arrange
        Expression<Func<UserAggregate, bool>> expression = x => x.Name == "Test";

        // Act
        var result = expression.ToFilterDefinition();
        var filter = ((BsonDocumentFilterDefinition<UserAggregate>)result).Document.ToString();

        // Assert
        Assert.IsType<BsonDocumentFilterDefinition<UserAggregate>>(result);
        Assert.Equal("{ \"Name\" : { \"$eq\" : \"Test\" } }", filter);
    }
}