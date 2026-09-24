using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Exceptions;

namespace CodeDesignPlus.Net.Microservice.Domain;

public class Errors : IErrorCodes
{
    public static readonly Error IdOrderIsInvalid = new("100", "Id is invalid.");
    public static readonly Error TenantIsInvalid = new("101", "Tenant is invalid.");

    public static readonly Error ClientIsNull = new("102", "Client is null.");
    public static readonly Error IdClientIsInvalid = new("103", "Id client is invalid.");
    public static readonly Error NameClientIsInvalid = new("104", "Name client is invalid.");

    public static readonly Error ProductIsNull = new("105", "Product is null.");
    public static readonly Error IdProductIsInvalid = new("106", "Id product is invalid.");
    public static readonly Error NameProductIsInvalid = new("107", "Name product is invalid.");
    public static readonly Error PriceProductIsInvalid = new("108", "Price product is invalid.");
    public static readonly Error QuantityProductIsInvalid = new("109", "Quantity product is invalid.");

    public static readonly Error ProductNotFound = new("110", "Product not found in the order.");
    public static readonly Error OrderAlreadyCompleted = new("111", "Order already completed.");
    public static readonly Error OrderAlreadyCancelled = new("112", "Order already cancelled.");
}
