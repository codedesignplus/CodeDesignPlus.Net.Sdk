﻿// This is a simple example of how to use the library CodeDesignPlus.Net.Exceptions
using CodeDesignPlus.Net.Exceptions;
using CodeDesignPlus.Net.Exceptions.Guards;
using CodeDesignPlus.Net.Exceptions.Sample.Errors;

string? user = null;

// This is a simple example of how to use the library CodeDesignPlus.Net.Exceptions with the DomainGuard
try
{
    DomainGuard.IsNullOrEmpty(user, ErrorDomain.UserIsRequired);
}
catch (CodeDesignPlusException ex)
{
    Console.WriteLine(ex.Message);
    Console.WriteLine(ex.Code);
    Console.WriteLine(ex.Layer);
}

// This is a simple example of how to use the library CodeDesignPlus.Net.Exceptions with the ApplicationGuard
try
{
    ApplicationGuard.IsNullOrEmpty(user, ErrorApplication.UserIsRequired);
}
catch (CodeDesignPlusException ex)
{
    Console.WriteLine(ex.Message);
    Console.WriteLine(ex.Code);
    Console.WriteLine(ex.Layer);
}

// This is a simple example of how to use the library CodeDesignPlus.Net.Exceptions with the InfrastructureGuard
try
{
    InfrastructureGuard.IsNullOrEmpty(user, ErrorInfrastructure.UserIsRequired);
}
catch (CodeDesignPlusException ex)
{
    Console.WriteLine(ex.Message);
    Console.WriteLine(ex.Code);
    Console.WriteLine(ex.Layer);
}
