﻿namespace CodeDesignPlus.Net.Exceptions.Extensions;

public static class GuardExtensions
{
    public static string GetCode(this string message) => message.Split(":").First().Trim();

    public static string GetMessage(this string message) => message.Split(":").Last().Trim();
}