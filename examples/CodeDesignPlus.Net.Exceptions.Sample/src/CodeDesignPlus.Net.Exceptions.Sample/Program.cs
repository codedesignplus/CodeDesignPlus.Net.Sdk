// See https://aka.ms/new-console-template for more information
using CodeDesignPlus.Net.Exceptions;
using CodeDesignPlus.Net.Exceptions.Extensions;
using CodeDesignPlus.Net.Exceptions.Guards;
using CodeDesignPlus.Net.Exceptions.Sample;

Console.WriteLine("This is a sample project to show how to use the CodeDesignPlus.Net.Exceptions library. \n");

// Use the clause guard to validate the input parameters
string? stringIsNull = null;

try
{
    Console.WriteLine(" \n1. This is a sample to  DomainGuard");

    DomainGuard.IsNull(stringIsNull, ErrorsDomain.Unknown);
}
catch (CodeDesignPlusException ex)
{
    Console.WriteLine("Layer: " + ex.Layer);
    Console.WriteLine("Code: " + ex.Code);
    Console.WriteLine("Message: " + ex.Message);
}


try
{
    Console.WriteLine(" \n2. This is a sample to  ApplicationGuard");

    ApplicationGuard.IsNull(stringIsNull, ErrorsDomain.Unknown);
}
catch (CodeDesignPlusException ex)
{
    Console.WriteLine("Layer: " + ex.Layer);
    Console.WriteLine("Code: " + ex.Code);
    Console.WriteLine("Message: " + ex.Message);
}

try
{
    Console.WriteLine("\n3. This is a sample to InfrastructureGuard");

    InfrastructureGuard.IsNull(stringIsNull, ErrorsDomain.Unknown);
}
catch (CodeDesignPlusException ex)
{
    Console.WriteLine("Layer: " + ex.Layer);
    Console.WriteLine("Code: " + ex.Code);
    Console.WriteLine("Message: " + ex.Message);
}

// Get the code and message of the error
Console.WriteLine("\n4. This is a sample to get the code and message of the error");

var code = ErrorsDomain.Unknown.GetCode();
var message = ErrorsDomain.Unknown.GetMessage();

Console.WriteLine("Code: " + code);
Console.WriteLine("Message: " + message);