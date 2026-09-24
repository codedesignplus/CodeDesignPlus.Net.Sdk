using System;
using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Exceptions;

namespace CodeDesignPlus.Net.Microservice.Commons.Test.Helpers;

public class ErrorsFake : IErrorCodes
{
    public static readonly Error CustomError = new("101", "Custom error message");
}
