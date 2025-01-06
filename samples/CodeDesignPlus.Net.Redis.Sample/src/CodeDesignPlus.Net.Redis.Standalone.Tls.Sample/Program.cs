using CodeDesignPlus.Net.Redis.Abstractions;
using CodeDesignPlus.Net.Redis.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var serviceCollection = new ServiceCollection();

serviceCollection.AddLogging();
serviceCollection.AddRedis(configuration);

var serviceProvider = serviceCollection.BuildServiceProvider();

var factory = serviceProvider.GetRequiredService<IRedisFactory>();

var service = factory.Create(FactoryConst.RedisCore);

var item = new {
    Id = 1,
    Name = "CodeDesignPlus"
};

await service.Database.StringSetAsync("item", JsonConvert.SerializeObject(item));

var result = await service.Database.StringGetAsync("item");

Console.WriteLine(result);

// Requisitos
/* 
    <None Update="server/Certificates/client.pfx">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
*/