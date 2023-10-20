using System.Security.Authentication;
using CodeDesignPlus.Net.xUnit.Helpers;
using StackExchange.Redis;

namespace CodeDesignPlus.Net.Redis.Test.Options;

public class InstanceTest
{
    private const string FakeConnectionString = "abortConnect=true,allowAdmin=false,channelPrefix=MyAppPrefix,checkCertificateRevocation=true,connectRetry=3,connectTimeout=6000,configChannel=MyConfigChannel,configCheckSeconds=90,defaultDatabase=0,keepAlive=120,name=MyRedisClient,password=MySuperSecretPassword,user=RedisUser,proxy=None,resolveDns=true,serviceName=MyService,ssl=false,sslHost=mysslhost.com,sslProtocols=Tls12|Tls13,syncTimeout=6000,asyncTimeout=6000,tiebreaker=MyTieBreaker,version=2.0,tunnel=http://myproxyserver.com,setlib=true";

    [Fact]
    public void CreateConfiguration_AssignsValues_Correctly()
    {
        // Arrange
        var instance = new Instance
        {
            ConnectionString = FakeConnectionString,
            HighPrioritySocketThreads = true,
            Certificate = "myCertificate.pfx",
            PasswordCertificate = "password123"
        };

        // Act
        var result = instance.CreateConfiguration();
        bool hasBothProtocols = (result.SslProtocols & (SslProtocols.Tls12 | SslProtocols.Tls13)) == (SslProtocols.Tls12 | SslProtocols.Tls13);

        // Assert
        Assert.True(result.AbortOnConnectFail);
        Assert.False(result.AllowAdmin);
        Assert.Equal("MyAppPrefix", result.ChannelPrefix);
        Assert.True(result.CheckCertificateRevocation);
        Assert.Equal(3, result.ConnectRetry);
        Assert.Equal(6000, result.ConnectTimeout);
        Assert.Equal("MyConfigChannel", result.ConfigurationChannel);
        Assert.Equal(90, result.ConfigCheckSeconds);
        Assert.Equal(0, result.DefaultDatabase);
        Assert.Equal(120, result.KeepAlive);
        Assert.Equal("MyRedisClient", result.ClientName);
        Assert.Equal("MySuperSecretPassword", result.Password);
        Assert.Equal("RedisUser", result.User);
        Assert.True(result.Proxy == Proxy.None);
        Assert.True(result.ResolveDns);
        Assert.Equal("MyService", result.ServiceName);
        Assert.False(result.Ssl);
        Assert.Equal("mysslhost.com", result.SslHost);
        Assert.True(hasBothProtocols);
        Assert.Equal(6000, result.SyncTimeout);
        Assert.Equal(6000, result.AsyncTimeout);
        Assert.Equal("MyTieBreaker", result.TieBreaker);
        Assert.Equal(new Version(2, 0), result.DefaultVersion);
        Assert.NotNull(result.SocketManager);
        Assert.Equal("RedisInstance", result.SocketManager.Name);
    }

    [Fact]
    public void ConnectionString_IsRequired_Failed()
    {
        // Arrange
        var instance = new Instance();

        // Act
        var results = instance.Validate();

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, x => x.ErrorMessage!.Equals("The ConnectionString field is required.") && x.MemberNames.Contains(nameof(Instance.ConnectionString)));
    }

    [Fact]
    public void ConnectionString_InvalidFormat_Failed()
    {
        // Arrange
        var instance = new Instance
        {
            ConnectionString = "abortConnect=true;invalidParameter=1234;"
        };

        // Act
        var results = instance.Validate();

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, x => x.ErrorMessage!.Equals("Invalid connection string format.") && x.MemberNames.Contains(nameof(Instance.ConnectionString)));
    }

}
