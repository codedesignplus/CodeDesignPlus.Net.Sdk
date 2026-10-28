using CodeDesignPlus.Net.gRpc.Clients.Services.Payment;
using Google.Protobuf;

namespace CodeDesignPlus.Net.gRpc.Clients.Test.Services.Payment;

/// <summary>
/// The payment contract changes while emitters and consumers are deployed separately: what an old client writes
/// must still be readable, and the numbers it used must not mean anything else now.
/// </summary>
public class PaymentContractTest
{
    /// <summary>
    /// An old client still sends the provider as the positional enum in field 11 (Payu = 1) and the card in field 10.
    /// The new contract reads the request and leaves the provider empty, so ms-payments resolves it by country.
    /// </summary>
    [Fact]
    public void InitiatePaymentRequest_PayloadFromOldClient_IsReadWithoutProvider()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();

        using var stream = new MemoryStream();
        var output = new CodedOutputStream(stream);
        output.WriteTag(1, WireFormat.WireType.LengthDelimited);
        output.WriteString(id);
        output.WriteTag(10, WireFormat.WireType.LengthDelimited);
        output.WriteBytes(ByteString.CopyFrom(0x0A, 0x0A, (byte)'C', (byte)'R', (byte)'E', (byte)'D', (byte)'I', (byte)'T', (byte)'C', (byte)'A', (byte)'R', (byte)'D'));
        output.WriteTag(11, WireFormat.WireType.Varint);
        output.WriteEnum(1);
        output.Flush();

        // Act
        var request = InitiatePaymentRequest.Parser.ParseFrom(stream.ToArray());

        // Assert
        Assert.Equal((id, false), (request.Id, request.HasProvider));
    }

    /// <summary>
    /// The numbers removed from the request are reserved: none of the current fields reuses them.
    /// </summary>
    [Theory]
    [InlineData(9)]
    [InlineData(10)]
    [InlineData(11)]
    public void InitiatePaymentRequest_RemovedFieldNumbers_AreNotReused(int fieldNumber)
    {
        // Act
        var field = InitiatePaymentRequest.Descriptor.FindFieldByNumber(fieldNumber);

        // Assert
        Assert.Null(field);
    }

    /// <summary>
    /// The provider travels as a text code, never as a position in a list.
    /// </summary>
    [Fact]
    public void InitiatePaymentRequest_Provider_IsATextCode()
    {
        // Act
        var field = InitiatePaymentRequest.Descriptor.FindFieldByName("provider");

        // Assert
        Assert.Equal(Google.Protobuf.Reflection.FieldType.String, field.FieldType);
    }
}
