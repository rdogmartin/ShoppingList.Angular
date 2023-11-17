using Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Api.Functions
{
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

    //[FunctionName("Hello")]
    //public static async Task<IActionResult> Run(
    //    [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
    //    ILogger log)
    //{
    //  log.LogInformation("Hello endpoint triggered.");

    //  string name = req.Query["name"];

    //  string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    //  dynamic data = JsonConvert.DeserializeObject(requestBody);
    //  name = name ?? data?.name;

    //  string responseMessage = string.IsNullOrEmpty(name)
    //      ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
    //      : $"Hello, {name}. This HTTP triggered function executed successfully.";

    //  return new OkObjectResult(responseMessage);
    //}
  }
}
