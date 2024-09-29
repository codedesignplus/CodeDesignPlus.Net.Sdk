using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.Microservice.Domain;

public class Errors : IErrorCodes
{
    public const string IdOrderIsInvalid = "100 : Id is invalid.";
    public const string TenantIsInvalid = "101 : Tenant is invalid.";

    public const string ClientIsNull = "102 : Client is null.";
    public const string IdClientIsInvalid = "103 : Id client is invalid.";
    public const string NameClientIsInvalid = "104 : Name client is invalid.";

    public const string ProductIsNull = "105 : Product is null.";
    public const string IdProductIsInvalid = "106 : Id product is invalid.";
    public const string NameProductIsInvalid = "107 : Name product is invalid.";
    public const string PriceProductIsInvalid = "108 : Price product is invalid.";
    public const string QuantityProductIsInvalid = "109 : Quantity product is invalid.";

    public const string ProductNotFound = "110 : Product not found in the order.";
    public const string OrderAlreadyCompleted = "111 : Order already completed.";
    public const string OrderAlreadyCancelled = "112 : Order already cancelled.";
}
