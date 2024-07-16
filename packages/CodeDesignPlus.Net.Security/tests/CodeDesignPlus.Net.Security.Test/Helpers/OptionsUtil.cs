namespace CodeDesignPlus.Net.Security.Test.Helpers;

public static class OptionsUtil
{
    public static readonly SecurityOptions SecurityOptions = new()
    {
        Authority = "https://localhost:5001",
        Applications = ["CodeDesignPlus.Net.Security.Test"],
        CertificatePassword = "123456",
        CertificatePath = "certificate.pfx",
        IncludeErrorDetails = true,
        RequireHttpsMetadata = false,
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateLifetime = false,
        ValidIssuer = "https://localhost:5001",
        ValidAudiences = ["CodeDesignPlus.Net.Security.Test"]
    };

    public static readonly SecurityOptions SecurityOptionsAzure = new()
    {
        Authority = "https://codedesignplus.b2clogin.com/codedesignplus.onmicrosoft.com/B2C_1_signin/v2.0",
        IncludeErrorDetails = true,
        RequireHttpsMetadata = false,
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateLifetime = false,
        ValidIssuer = "https://codedesignplus.b2clogin.com/codedesignplus.onmicrosoft.com/B2C_1_signin/v2.0",
        ValidAudiences = ["7f2aea10-b63c-44ce-8580-78ab3e5189aa"]
    };

    public static readonly SecurityOptions SercurityOptionsLocalhost = new()
    {
        Authority = "http://localhost",
        IncludeErrorDetails = true,
        RequireHttpsMetadata = false,
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateLifetime = false,
        ValidIssuer = "http://localhost",
        ValidAudiences = ["api1"]
    };

}
