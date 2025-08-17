using System;
using System.Configuration;
using api.Services;
using Api.Services;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Api.Startup))]

namespace Api;

public class Startup : FunctionsStartup
{
    private static readonly IConfigurationRoot Configuration = new ConfigurationBuilder()
        .SetBasePath(Environment.CurrentDirectory)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddUserSecrets<Startup>()
        .AddEnvironmentVariables()
        .Build();

    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddSingleton<IConfiguration>(Configuration);
        builder.Services.AddScoped<IAppInfoService, AppInfoService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IListItemService, ListItemService>();

        builder.Services.AddSingleton(_ => GetCosmosClient());
    }

    private static CosmosClient GetCosmosClient()
    {
        string endpoint = Configuration["CosmosDb:EndpointUrl"];
        if (string.IsNullOrEmpty(endpoint) || endpoint.StartsWith("Put actual key in secrets.json"))
        {
            throw new ConfigurationErrorsException("Missing setting 'CosmosDb:EndpointUrl'. Please specify a valid endpoint in the appSettings.json file or your Azure Functions Settings.");
        }

        string authKey = Configuration["CosmosDb:AuthorizationKey"];
        if (string.IsNullOrEmpty(authKey) || authKey.StartsWith("Put actual key in secrets.json"))
        {
            throw new ConfigurationErrorsException("Missing setting 'CosmosDb:AuthorizationKey'. Please specify a valid AuthorizationKey in secrets.json, the appSettings.json file or your Azure Functions Settings.");
        }

        var configurationBuilder = new CosmosClientBuilder(endpoint, authKey);
        return configurationBuilder
            .WithSerializerOptions(new CosmosSerializationOptions()
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
            }).Build();
    }
}
