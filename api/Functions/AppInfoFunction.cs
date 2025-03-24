using System.Threading.Tasks;
using Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Api.Functions;

public class AppInfoFunction
{
    private readonly IAppInfoService _appInfoService;

    public AppInfoFunction(IAppInfoService appInfoService)
    {
        _appInfoService = appInfoService;
    }

    [FunctionName("AppInfo")]
    public Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("AppInfo endpoint triggered.");

        var apiInfo = _appInfoService.GetApiInfo();

        return Task.FromResult<IActionResult>(new OkObjectResult(apiInfo));
    }
}
