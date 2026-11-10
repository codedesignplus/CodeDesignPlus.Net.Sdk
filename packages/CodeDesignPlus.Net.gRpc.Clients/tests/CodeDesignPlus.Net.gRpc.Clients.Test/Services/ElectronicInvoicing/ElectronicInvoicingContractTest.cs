using CodeDesignPlus.Net.gRpc.Clients.Services.ElectronicInvoicing;

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
}
