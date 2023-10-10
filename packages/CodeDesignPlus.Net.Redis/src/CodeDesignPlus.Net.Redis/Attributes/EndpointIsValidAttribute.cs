using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CodeDesignPlus.Net.Redis;

/// <summary>
/// Check if endpoint is valid
/// </summary>
public partial class EndpointIsValidAttribute : ValidationAttribute
{
    /// <summary>
    /// Matches valid IP addresses
    /// </summary>
    private const string ValidIpAddressRegex = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5]):((6553[0-5])|(655[0-2][0-9])|(65[0-4][0-9]{2})|(6[0-4][0-9]{3})|([1-5][0-9]{4})|([0-5]{0,5})|([0-9]{1,4}))$";

    /// <summary>
    /// Matches valid Hostname. Valid as per (RFC 1123 <see cref="https://datatracker.ietf.org/doc/html/rfc1123"/>)
    /// </summary>
    private const string ValidHostnameRegex = @"(?=^.{4,253}$)(^((?!-)[a-zA-Z0-9-]{0,62}[a-zA-Z0-9]\.)+[a-zA-Z]{2,63}):((6553[0-5])|(655[0-2][0-9])|(65[0-4][0-9]{2})|(6[0-4][0-9]{3})|([1-5][0-9]{4})|([0-5]{0,5})|([0-9]{1,4}))$";

    /// <summary>
    /// Determines whether the specified value of the object is valid.
    /// </summary>
    /// <param name="value">The value of the object to validate.</param>
    /// <returns>true if the specified value is valid; otherwise, false.</returns>
    public override bool IsValid(object value)
    {
        var endpoints = (List<string>)value;

        var validIpAddressRegex = CheckIp();
        var validHostnameRegex = CheckHostname();

        foreach (string endpoint in endpoints)
        {
            if (!validIpAddressRegex.IsMatch(endpoint) && !validHostnameRegex.IsMatch(endpoint))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Checks if the input matches an IP address format.
    /// </summary>
    /// <returns>A Regex instance for validating an IP address.</returns>
    /// <remarks>
    /// The regex checks for valid IPv4 address formats followed by a port number.
    /// </remarks>
    [GeneratedRegex("^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5]):((6553[0-5])|(655[0-2][0-9])|(65[0-4][0-9]{2})|(6[0-4][0-9]{3})|([1-5][0-9]{4})|([0-5]{0,5})|([0-9]{1,4}))$")]
    private static partial Regex CheckIp();

    /// <summary>
    /// Checks if the input matches a valid hostname format.
    /// </summary>
    /// <returns>A Regex instance for validating a hostname.</returns>
    /// <remarks>
    /// The regex checks for valid hostname formats followed by a port number.
    /// </remarks>
    [GeneratedRegex("(?=^.{4,253}$)(^((?!-)[a-zA-Z0-9-]{0,62}[a-zA-Z0-9]\\.)+[a-zA-Z]{2,63}):((6553[0-5])|(655[0-2][0-9])|(65[0-4][0-9]{2})|(6[0-4][0-9]{3})|([1-5][0-9]{4})|([0-5]{0,5})|([0-9]{1,4}))$")]
    private static partial Regex CheckHostname();
}