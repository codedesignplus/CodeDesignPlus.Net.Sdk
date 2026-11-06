namespace CodeDesignPlus.Net.Criteria;

/// <summary>
/// The errors of a malformed filter, in the range <b>9500</b>.
/// </summary>
/// <remarks>
/// The SDK numbers from 9000 so as not to collide with the layers of a microservice (1xx, 2xx, 3xx): 9000 the value
/// objects, 9200 the gRPC clients, 9300 the form validation, 9400 the security package and 9500 this one.
/// <para>
/// They all come from what the client wrote in <c>filters</c> or <c>orderBy</c>, so they are a bad request: the
/// microservice answers 400 (REST) or <c>InvalidArgument</c> (gRPC), with the message in the language of the request.
/// Before they were plain messages in English and the microservice answered 500 (Kappali pendings/403).
/// </para>
/// <para>
/// It is not called <c>Errors</c> because <see cref="CriteriaException"/> already has a property with that name.
/// </para>
/// </remarks>
public class CriteriaErrors : IErrorCodes
{
    /// <summary>
    /// A condition of the filter is incomplete or has no comparison operator.
    /// </summary>
    public static readonly Error InvalidCondition = new("9500");

    /// <summary>
    /// A condition follows the previous one without <c>|and|</c> or <c>|or|</c>. <c>{0}</c> is the property of the condition.
    /// </summary>
    public static readonly Error MissingLogicalOperator = new("9501");

    /// <summary>
    /// The property of a filter or of the order does not exist. <c>{0}</c> is the property.
    /// </summary>
    public static readonly Error UnknownProperty = new("9502");

    /// <summary>
    /// The value does not fit the type of the property. <c>{0}</c> is the value and <c>{1}</c> the property.
    /// </summary>
    public static readonly Error InvalidValue = new("9503");

    /// <summary>
    /// The operator does not apply to the type of the property. <c>{0}</c> is the operator and <c>{1}</c> the property.
    /// </summary>
    public static readonly Error UnsupportedOperator = new("9504");

    /// <summary>
    /// The filter is not valid, without a more precise cause.
    /// </summary>
    public static readonly Error InvalidFilter = new("9505");
}
