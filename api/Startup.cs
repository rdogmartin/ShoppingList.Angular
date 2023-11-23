using api.Services;
using Api.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Api.Startup))]

namespace Api;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddScoped<IAppInfoService, AppInfoService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IListItemService, ListItemService>();
    }
}
