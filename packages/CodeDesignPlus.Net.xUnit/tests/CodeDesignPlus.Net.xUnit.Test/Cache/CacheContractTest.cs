using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.xUnit.Cache;
using Newtonsoft.Json;

namespace CodeDesignPlus.Net.xUnit.Test.Cache;

/// <summary>
/// El inspector del cache encuentra lo que Newtonsoft escribe y no sabe leer, y deja pasar lo que sí vuelve
/// (pendings/378 y pendings/381).
/// </summary>
public class CacheContractTest
{
    [Fact]
    public void FindLostProperties_PrivateSettersOutsideTheConstructor_ListsThem()
    {
        // Act
        var lost = CacheContract.FindLostProperties(typeof(PrivateSetterDto));

        // Assert
        Assert.Equal(["PrivateSetterDto.To", "PrivateSetterDto.Subject"], lost);
    }

    [Fact]
    public void FindLostProperties_PrivateSetterMissingFromTheJsonConstructorOfANestedValue_ListsIt()
    {
        // Act
        var lost = CacheContract.FindLostProperties(typeof(DocumentDto));

        // Assert
        Assert.Equal(["Payment.CreditBalanceAmount"], lost);
    }

    [Fact]
    public void FindLostProperties_PublicSettersInitConstructorAndComputed_LosesNothing()
    {
        // Act
        var lost = CacheContract.FindLostProperties(typeof(HealthyDto));

        // Assert
        Assert.Empty(lost);
    }

    [Fact]
    public void FindLostProperties_AbstractElement_SaysItCannotBeBuilt()
    {
        // Act
        var lost = CacheContract.FindLostProperties(typeof(AbstractElementDto));

        // Assert
        Assert.Equal(["Shape: no constructor Newtonsoft can use"], lost);
    }

    [Fact]
    public void FindLostPropertiesInDtos_Assembly_ReportsEveryDtoThatLosesSomething()
    {
        // Act
        var lost = CacheContract.FindLostPropertiesInDtos(typeof(CacheContractTest).Assembly);

        // Assert
        Assert.Equal(["AbstractElementDto → Shape: no constructor Newtonsoft can use", "DocumentDto → Payment.CreditBalanceAmount", "PrivateSetterDto → PrivateSetterDto.To", "PrivateSetterDto → PrivateSetterDto.Subject"], lost);
    }

    [Fact]
    public void RoundTrip_PrivateSetters_ComeBackEmpty()
    {
        // Arrange
        var dto = PrivateSetterDto.Create("ana@kappali.com", "Cuenta de cobro");

        // Act
        var read = CacheContract.RoundTrip(dto);

        // Assert
        Assert.Equal((dto.Id, (string?)null), (read.Id, read.To));
    }

    public class PrivateSetterDto : IDtoBase
    {
        public Guid Id { get; set; }

        public string? To { get; private set; }

        public string? Subject { get; private set; }

        public static PrivateSetterDto Create(string to, string subject) => new() { Id = Guid.NewGuid(), To = to, Subject = subject };
    }

    public class DocumentDto : IDtoBase
    {
        public Guid Id { get; set; }

        public List<Payment> Payments { get; set; } = [];
    }

    public class Payment
    {
        [JsonConstructor]
        private Payment(long amount)
        {
            this.Amount = amount;
        }

        public long Amount { get; private set; }

        public long? CreditBalanceAmount { get; private set; }

        [JsonProperty]
        public List<long> Allocations { get; private set; } = [];
    }

    public class HealthyDto : IDtoBase
    {
        public Guid Id { get; set; }

        public Line Line { get; init; } = new(1, 2);

        public Dictionary<string, Line> ByCode { get; set; } = [];

        public NodaTime.Instant At { get; set; }
    }

    public class Line(long quantity, long price)
    {
        public long Quantity { get; private set; } = quantity;

        public long Price { get; private set; } = price;

        public long Subtotal => this.Quantity * this.Price;
    }

    public class AbstractElementDto : IDtoBase
    {
        public Guid Id { get; set; }

        public List<Shape> Shapes { get; set; } = [];
    }

    public abstract class Shape
    {
        public string? Name { get; set; }
    }
}
