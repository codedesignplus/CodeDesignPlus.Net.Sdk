namespace CodeDesignPlus.Net.Core.Abstractions;

/// <summary>
/// Represents an item with a unique identifier and a value.
/// </summary>
/// <typeparam name="TValue">The type of the value associated with the item.</typeparam>
/// <param name="Id">The unique identifier of the item.</param>
/// <param name="Value">The value associated with the item.</param>
public record Item<TValue>(Guid Id, TValue Value);