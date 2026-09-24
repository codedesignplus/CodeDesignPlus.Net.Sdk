using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Exceptions;

namespace CodeDesignPlus.Net.Microservice.Domain;

public class Errors : IErrorCodes
{
    public static readonly Error IdOrderIsInvalid = new("100");
    public static readonly Error TenantIsInvalid = new("101");

    public static readonly Error ClientIsNull = new("102");
    public static readonly Error IdClientIsInvalid = new("103");
    public static readonly Error NameClientIsInvalid = new("104");

    public static readonly Error ProductIsNull = new("105");
    public static readonly Error IdProductIsInvalid = new("106");
    public static readonly Error NameProductIsInvalid = new("107");
    public static readonly Error PriceProductIsInvalid = new("108");
    public static readonly Error QuantityProductIsInvalid = new("109");

    public static readonly Error ProductNotFound = new("110");
    public static readonly Error OrderAlreadyCompleted = new("111");
    public static readonly Error OrderAlreadyCancelled = new("112");
}
