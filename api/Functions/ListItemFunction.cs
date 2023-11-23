using System.Threading.Tasks;
using api.Services;
using Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Api.Functions;

public class ListItemFunction
{
    private readonly IAuthService _authService;
    private readonly IListItemService _listItemService;

    public ListItemFunction(IAuthService authService, IListItemService listItemService)
    {
        _authService = authService;
        _listItemService = listItemService;
    }

    [FunctionName("GetListItems")]
    public Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.User, "get", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("GetListItems endpoint triggered.");

        var authResult = this._authService.Authorize(req);
        if (!authResult.IsAuthenticated)
        {
            log.LogWarning("User not authentication. Aborting invocation of Api.Functions.GetListItems");
            return Task.FromResult<IActionResult>(new UnauthorizedResult());
        }

        var listItems = _listItemService.GetListItems();

        return Task.FromResult<IActionResult>(new OkObjectResult(listItems));
    }
}
