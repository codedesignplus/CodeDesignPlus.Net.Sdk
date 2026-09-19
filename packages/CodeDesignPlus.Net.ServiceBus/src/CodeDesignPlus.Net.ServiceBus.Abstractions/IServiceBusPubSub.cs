namespace CodeDesignPlus.Net.ServiceBus.Abstractions;

/// <summary>
/// Marker interface for the Azure Service Bus implementation of <see cref="IMessage"/>.
/// </summary>
/// <remarks>
/// Permite resolver el transporte concreto sin perder el registro generico de <see cref="IMessage"/>,
/// igual que hacen el resto de transportes del SDK.
/// </remarks>
public interface IServiceBusPubSub : IMessage
{
}
