using System;
using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.Microservice.Commons.Test.Helpers;

public class ErrorsFake : IErrorCodes
{
    public const string CustomError = "101 : Custom error message";
}
