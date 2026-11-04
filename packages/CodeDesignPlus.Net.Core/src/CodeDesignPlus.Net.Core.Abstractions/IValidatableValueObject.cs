namespace CodeDesignPlus.Net.Core.Abstractions;

/// <summary>
/// A value object whose constructor only hydrates and whose invariants are checked on demand.
/// </summary>
/// <remarks>
/// Mongo, the cache and the deserialization of events and requests rebuild a value object through its constructor, and a
/// read must never fail because a stored value would be rejected today. The invariants live in <see cref="EnsureValid"/>:
/// the factory methods call it, the aggregates may call it on every write, and the MediatR pipeline of
/// <c>CodeDesignPlus.Net.Microservice.Commons</c> calls it on every value object a request carries.
/// </remarks>
public interface IValidatableValueObject
{
    /// <summary>
    /// Checks the invariants of the value object, including the value objects it contains.
    /// </summary>
    /// <exception cref="Exception">Thrown with the error of the first invariant that does not hold.</exception>
    void EnsureValid();
}
