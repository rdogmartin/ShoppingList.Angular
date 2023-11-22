using System.Threading.Tasks;
using Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Api.Functions;

public class ListItemFunction
{
    private readonly IListItemService _listItemService;

    public ListItemFunction(IListItemService listItemService)
    {
        _listItemService = listItemService;
    }

    [FunctionName("GetListItems")]
    public Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("GetListItems endpoint triggered.");

        var listItems = _listItemService.GetListItems();

        return Task.FromResult<IActionResult>(new OkObjectResult(listItems));
    }
}
