using CodeDesignPlus.Net.Microservice.Commons.EntryPoints.gRpc.Interceptors;
using CodeDesignPlus.Net.Microservice.gRPC.Sample.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc(x => {
    x.Interceptors.Add<ErrorInterceptor>();
});


builder.Services.AddGrpcReflection();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
