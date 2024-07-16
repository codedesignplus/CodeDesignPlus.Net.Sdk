using IdentityServer4.Models;
using IdentityServer4.Test;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services
    .AddIdentityServer()
    .AddInMemoryClients(
    [
        new() {
            ClientId = "security-client",
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            ClientSecrets =
            {
                new Secret("mySecret".Sha256())
            },
            AllowedScopes = { "api1" }
        }
    ])
    .AddInMemoryApiScopes(
    [
        new ApiScope("api1", "Api-Demo")
    ])
    .AddInMemoryApiResources(
    [
        new ApiResource("api1", "Api-Demo")
        {
            Scopes = { "api1" }
        }
    ])
    .AddTestUsers(
    [
        new() {
            SubjectId = "1",
            Username = "test",
            Password = "password"
        }
    ])
    .AddDeveloperSigningCredential();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseIdentityServer();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
