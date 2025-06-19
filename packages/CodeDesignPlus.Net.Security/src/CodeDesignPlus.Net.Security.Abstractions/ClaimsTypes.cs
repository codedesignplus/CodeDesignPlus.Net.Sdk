namespace CodeDesignPlus.Net.Security.Abstractions;

/// <summary>
/// Contains the claim types that are used by Azure AD B2C.
/// </summary>
public static class ClaimTypes
{
    /// <summary>
    /// The version of the ID token, as defined by Azure AD B2C.
    /// </summary>
    public const string Version = "ver";
    /// <summary>
    /// This claim identifies the security token service (STS) that constructs and returns the token. It also identifies the Azure AD directory in which the user was authenticated. Your app should validate the issuer claim to ensure that the token came from the v2.0 endpoint. It also should use the GUID portion of the claim to restrict the set of tenants that can sign in to the app.
    /// </summary>
    public const string Issuer = "iss";
    /// <summary>
    /// This is the principal about which the token asserts information, such as the user of an app. This value is immutable and cannot be reassigned or reused. It can be used to perform authorization checks safely, such as when the token is used to access a resource. By default, the subject claim is populated with the object ID of the user in the directory. To learn more, see Azure Active Directory B2C: Token, session, and single sign-on configuration.
    /// </summary>
    public const string Subject = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
    /// <summary>
    /// An audience claim identifies the intended recipient of the token. For Azure AD B2C, the audience is your app's Application ID, as assigned to your app in the app registration portal. Your app should validate this value and reject the token if it does not match.
    /// </summary>
    public const string Audience = "aud";
    /// <summary>
    /// The expiration time claim is the time at which the token becomes invalid, represented in epoch time. Your app should use this claim to verify the validity of the token lifetime.
    /// </summary>
    public const string Expiration = "exp";
    /// <summary>
    /// A nonce is a strategy used to mitigate token replay attacks. Your app can specify a nonce in an authorization request by using the nonce query parameter. The value you provide in the request will be emitted unmodified in the nonce claim of an ID token only. This allows your app to verify the value against the value it specified on the request, which associates the app's session with a given ID token. Your app should perform this validation during the ID token validation process.
    /// </summary>
    public const string Nonce = "nonce";
    /// <summary>
    /// The time at which the token was issued, represented in epoch time.
    /// </summary>
    public const string Iat = "iat";
    /// <summary>
    /// This claim is the time at which a user last entered credentials, represented in epoch time.
    /// </summary>
    public const string AuthTime = "auth_time";
    /// <summary>
    /// The immutable identifier for the user account in the tenant. It can be used to perform authorization checks safely and as a key in database tables. This ID uniquely identifies the user across applications - two different applications signing in the same user will receive the same value in the oid claim. This means that it can be used when making queries to Microsoft online services, such as the Microsoft Graph. The Microsoft Graph will return this ID as the id property for a given user account.
    /// </summary>
    public const string ObjectIdentifier = "http://schemas.microsoft.com/identity/claims/objectidentifier";
    /// <summary>
    /// The city in which the user is located.
    /// </summary>
    public const string City = "city";
    /// <summary>
    /// The country in which the user is located.
    /// </summary>
    public const string Country = "country";
    /// <summary>
    /// The user's given name (also known as first name).
    /// </summary>
    public const string FirstName = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname";
    /// <summary>
    /// The user's surname (also known as last name).
    /// </summary>
    public const string LastName = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname";
    /// <summary>
    /// The user's full name in displayable form including all name parts, possibly including titles and suffixes.
    /// </summary>
    public const string Name = "name";
    /// <summary>
    /// The postal code of the user's address.
    /// </summary>
    public const string PostalCode = "postalCode";
    /// <summary>
    ///  street address where the user is located.
    /// </summary>
    public const string StreetAddress = "streetAddress";
    /// <summary>
    /// The state or province in which the user is located.
    /// </summary>
    public const string State = "state";
    /// <summary>
    ///  user's job title.
    /// </summary>
    public const string JobTitle = "jobTitle";
    /// <summary>
    /// Email addresses of the user. These are mutable and might change over time. Therefore, they are not suitable for identifying the user in other databases or applications. The oid or sub claim should be used instead.
    /// </summary>
    public const string Emails = "emails";
    /// <summary>
    /// This is the name of the policy that was used to acquire the token.
    /// </summary>
    public const string Tfp = "tfp";
    /// <summary>
    /// This claim is the time at which the token becomes valid, represented in epoch time. This is usually the same as the time the token was issued. Your app should use this claim to verify the validity of the token lifetime.
    /// </summary>
    public const string Nbf = "nbf";
    /// <summary>
    /// The user's roles.
    /// </summary>
    public const string Groups = "groups";

}
