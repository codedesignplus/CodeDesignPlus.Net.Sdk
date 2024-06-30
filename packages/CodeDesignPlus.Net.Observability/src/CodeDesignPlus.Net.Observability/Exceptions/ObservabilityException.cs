﻿using System.Runtime.Serialization;

namespace CodeDesignPlus.Net.Observability.Exceptions;

/// <summary>
/// The exception that is thrown when an error occurs within CodeDesignPlus.Net.Observability. 
/// </summary>
public class ObservabilityException : Exception
{
    /// <summary>
    /// Contains the errors
    /// </summary>
    public IEnumerable<string> Errors { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservabilityException"/> class.
    /// </summary>
    public ObservabilityException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservabilityException"/> class.
    /// </summary>
    /// <param name="errors">The custom errors</param>
    public ObservabilityException(IEnumerable<string> errors)
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservabilityException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ObservabilityException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservabilityException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="errors">The custom errors</param>
    public ObservabilityException(string message, IEnumerable<string> errors) : base(message)
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservabilityException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic)
    /// if no inner exception is specified.
    /// </param>
    public ObservabilityException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservabilityException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="errors">The custom errors</param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic)
    /// if no inner exception is specified.
    /// </param>
    public ObservabilityException(string message, IEnumerable<string> errors, Exception innerException) : base(message, innerException)
    {
        this.Errors = errors;
    }
}