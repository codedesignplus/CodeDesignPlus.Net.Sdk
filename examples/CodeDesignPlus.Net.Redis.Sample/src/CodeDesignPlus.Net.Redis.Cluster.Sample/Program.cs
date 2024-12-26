using CodeDesignPlus.Net.Redis.Abstractions;
using CodeDesignPlus.Net.Redis.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

Console.WriteLine("Hello Redis!");

Console.WriteLine("Wait 6 seconds to start the connection with the Redis server.");

await Task.Delay(6000);

Console.WriteLine("Start connection with the Redis server.");

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var serviceCollection = new ServiceCollection();

serviceCollection.AddLogging();
serviceCollection.AddRedis(configuration);

var serviceProvider = serviceCollection.BuildServiceProvider();

var factory = serviceProvider.GetRequiredService<IRedisServiceFactory>();

var service = factory.Create(FactoryConst.RedisCore);

var item = new {
    Id = 1,
    Name = "CodeDesignPlus"
};

var database =  service.Database;

await service.Database.StringSetAsync("item", JsonConvert.SerializeObject(item));

var result = await service.Database.StringGetAsync("item");

Console.WriteLine(result);

/*
Ejecutar con docker compose, dado que al depurar desde local, el multiplexer resuelve la ip interna del contenedor a la cual el host no tiene acceso, dando 
erro de timeout al intentar conectarse al contenedor de redis.
*/