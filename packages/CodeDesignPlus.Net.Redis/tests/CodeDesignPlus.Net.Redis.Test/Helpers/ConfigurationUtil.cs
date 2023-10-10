namespace CodeDesignPlus.Net.Redis.Test.Helpers;

public static class ConfigurationUtil
{
    public static readonly RedisOptions RedisOptions = new()
    {
        AbortOnConnectFail = false,
        AllowAdmin = true,
        AsyncTimeout = 10000,
        Certificate = @"C:\certificate.pfx",
        ChannelPrefix = "Redis - ",
        CheckCertificateRevocation = true,
        ClientName = "CodeDesignPlus.Redis.Client",
        ConfigCheckSeconds = 120,
        ConnectRetry = 4,
        ConnectTimeout = 10000,
        DefaultDatabase = 1,
        HighPrioritySocketThreads = false,
        Password = "mypassword",
        PasswordCertificate = "certpassword",
        ResolveDns = true,
        ServiceName = "mymaster",
        Ssl = true,
        SslHost = "redis-server-1.certificate.com",
        SyncTimeout = 10000,
        User = "myuser"
    };

    public static IConfiguration GetConfiguration()
    {
        return GetConfiguration(new AppSettings()
        {
            Redis = RedisOptions
        });
    }

    public static IConfiguration GetConfiguration(object? appSettings = null)
    {
        var json = JsonSerializer.Serialize(appSettings);

        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        return new ConfigurationBuilder().AddJsonStream(memoryStream).Build();
    }

}
