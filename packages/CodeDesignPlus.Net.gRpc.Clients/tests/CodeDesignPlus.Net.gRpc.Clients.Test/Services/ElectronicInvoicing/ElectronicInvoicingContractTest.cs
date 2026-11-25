using CodeDesignPlus.Net.gRpc.Clients.Services.ElectronicInvoicing;
using Google.Protobuf;

namespace CodeDesignPlus.Net.gRpc.Clients.Test.Services.ElectronicInvoicing;

/// <summary>
/// The client copy of <c>electronicinvoicing.proto</c> changes only its C# namespace: the route and the field numbers are
/// the server's, or the calls would reach no method or be read with another meaning.
/// </summary>
public class ElectronicInvoicingContractTest
{
    /// <summary>
    /// package and service form the route /ElectronicInvoicing.ElectronicInvoicing/IssueDocument the server listens on.
    /// </summary>
    [Fact]
    public void Service_FullName_IsTheServerRoute()
    {
        // Act
        var service = ElectronicinvoicingReflection.Descriptor.Services.Single();

        // Assert
        Assert.Equal("ElectronicInvoicing.ElectronicInvoicing", service.FullName);
    }

    /// <summary>
    /// The scope of an issue travels in the numbers the server reads: issuer_tenant is 2 and platform_issuer is 3.
    /// </summary>
    [Theory]
    [InlineData(1, "id")]
    [InlineData(2, "issuer_tenant")]
    [InlineData(3, "platform_issuer")]
    [InlineData(4, "type")]
    [InlineData(5, "reference")]
    [InlineData(6, "content")]
    [InlineData(7, "preceded_by")]
    public void IssueDocumentRequest_FieldNumbers_MatchTheServer(int number, string name)
    {
        // Act
        var field = IssueDocumentRequest.Descriptor.FindFieldByNumber(number);

        // Assert
        Assert.Equal(name, field?.Name);
    }

    /// <summary>
    /// The cash register of a point of sale ticket travels in field 10 of the content, after the fields a caller written
    /// before it already sends.
    /// </summary>
    [Theory]
    [InlineData(8, "billing_reference")]
    [InlineData(9, "correction_concept_code")]
    [InlineData(10, "point_of_sale")]
    public void DocumentContent_FieldNumbers_MatchTheServer(int number, string name)
    {
        // Act
        var field = DocumentContent.Descriptor.FindFieldByNumber(number);

        // Assert
        Assert.Equal(name, field?.Name);
    }

    /// <summary>
    /// The six texts of the cash register travel in the numbers the server reads.
    /// </summary>
    [Theory]
    [InlineData(1, "register_code")]
    [InlineData(2, "register_type")]
    [InlineData(3, "location_name")]
    [InlineData(4, "location_address")]
    [InlineData(5, "cashier_name")]
    [InlineData(6, "sale_code")]
    public void PointOfSale_FieldNumbers_MatchTheServer(int number, string name)
    {
        // Act
        var field = PointOfSale.Descriptor.FindFieldByNumber(number);

        // Assert
        Assert.Equal(name, field?.Name);
    }

    /// <summary>
    /// A content without the cash register is still a whole content: a caller that issues an invoice sends none.
    /// </summary>
    [Fact]
    public void DocumentContent_WithoutPointOfSale_SaysItIsAbsent()
    {
        // Arrange
        var content = DocumentContent.Parser.ParseFrom(new DocumentContent { DueDate = "2026-11-19" }.ToByteArray());

        // Assert
        Assert.Null(content.PointOfSale);
    }

    /// <summary>
    /// A document type is a position on the wire: the client's numbers are the server's.
    /// </summary>
    [Theory]
    [InlineData(DocumentType.Invoice, 1)]
    [InlineData(DocumentType.CreditNote, 2)]
    [InlineData(DocumentType.DebitNote, 3)]
    [InlineData(DocumentType.PosTicket, 4)]
    [InlineData(DocumentType.PosAdjustmentNote, 5)]
    public void DocumentType_Value_MatchesTheServer(DocumentType type, int expected)
    {
        // Assert
        Assert.Equal(expected, (int)type);
    }

    /// <summary>
    /// The history of the fiscal data managed from outside the issuer travels in the numbers the server reads (Kappali
    /// pendings/752).
    /// </summary>
    [Theory]
    [InlineData(1, "tenant")]
    [InlineData(2, "time_zone_id")]
    [InlineData(3, "managed_responsibilities")]
    [InlineData(4, "entries")]
    public void SetManagedFiscalProfileRequest_FieldNumbers_MatchTheServer(int number, string name)
    {
        // Act
        var field = SetManagedFiscalProfileRequest.Descriptor.FindFieldByNumber(number);

        // Assert
        Assert.Equal(name, field?.Name);
    }

    [Theory]
    [InlineData(1, "effective_from")]
    [InlineData(2, "tax_regime_code")]
    [InlineData(3, "tax_responsibilities")]
    public void ManagedFiscalProfileEntry_FieldNumbers_MatchTheServer(int number, string name)
    {
        // Act
        var field = ManagedFiscalProfileEntry.Descriptor.FindFieldByNumber(number);

        // Assert
        Assert.Equal(name, field?.Name);
    }
}
