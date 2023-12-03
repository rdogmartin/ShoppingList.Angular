using System.IO;
using System.Threading.Tasks;
using api.Services;
using Api.Dto;
using Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Api.Functions;

public class ListItemFunctions
{
    private readonly IAuthService _authService;
    private readonly IListItemService _listItemService;

    public ListItemFunctions(IAuthService authService, IListItemService listItemService)
    {
        _authService = authService;
        _listItemService = listItemService;
    }

    [FunctionName("GetListItems")]
    public async Task<ActionResult<UserListItems>> GetAll(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("GetListItems endpoint triggered.");

        //dynamic authResult = null;
        var authResult = this._authService.Authorize(req);
        if (!authResult.IsAuthenticated)
        {
            log.LogWarning("User not authenticated. Aborting invocation of Api.Functions.ListItemFunctions.GetListItems");
            return await Task.FromResult<ActionResult>(new UnauthorizedResult());
        }

        var listItems = await _listItemService.GetListItems(authResult?.User?.Identity?.Name ?? "Unknown");
        //var listItems = _listItemService.GetListItems("Unknown");

        return await Task.FromResult<ActionResult>(new OkObjectResult(listItems));
    }

    [FunctionName("AddListItem")]
    public async Task<IActionResult> Add(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("AddListItem endpoint triggered.");

        var authResult = this._authService.Authorize(req);
        if (!authResult.IsAuthenticated)
        {
            log.LogWarning("User not authenticated. Aborting invocation of Api.Functions.ListItemFunctions.AddListItem");
            return await Task.FromResult<IActionResult>(new UnauthorizedResult());
        }

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var itemToAdd = JsonConvert.DeserializeObject<ListItem>(requestBody);

        if (itemToAdd == null)
        {
            return await Task.FromResult<IActionResult>(new BadRequestObjectResult("Invalid request body. Must be a valid ListItem."));
        }

        var userListItems = await _listItemService.AddListItem(authResult.User.Identity?.Name ?? "Unknown", itemToAdd);

        return new CreatedResult($"{req.Scheme}://{req.Host}/api/GetListItems", userListItems);
    }

    [FunctionName("UpdateListItem")]
    public async Task<IActionResult> Update(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("UpdateListItem endpoint triggered.");

        var authResult = this._authService.Authorize(req);
        if (!authResult.IsAuthenticated)
        {
            log.LogWarning("User not authenticated. Aborting invocation of Api.Functions.ListItemFunctions.UpdateListItem");
            return await Task.FromResult<IActionResult>(new UnauthorizedResult());
        }

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

        // TODO: Validate that the request body is a valid ListItem.
        var itemToUpdate = JsonConvert.DeserializeObject<ListItemUpdate>(requestBody);

        if (itemToUpdate == null)
        {
            return await Task.FromResult<IActionResult>(new BadRequestObjectResult("Invalid request body. Must be a valid ListItem."));
        }

        var userListItems = await _listItemService.UpdateListItem(authResult.User.Identity?.Name ?? "Unknown", itemToUpdate);

        return await Task.FromResult<ActionResult>(new OkObjectResult(userListItems));
    }

    [FunctionName("DeleteListItem")]
    public async Task<IActionResult> Delete(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("DeleteListItem endpoint triggered.");

        var authResult = this._authService.Authorize(req);
        if (!authResult.IsAuthenticated)
        {
            log.LogWarning("User not authenticated. Aborting invocation of Api.Functions.ListItemFunctions.DeleteListItem");
            return await Task.FromResult<IActionResult>(new UnauthorizedResult());
        }

        var itemDescription = req.Query["item"];

        if (string.IsNullOrWhiteSpace(itemDescription))
        {
            return await Task.FromResult<ActionResult>(new OkResult());
        }

        var userListItems = await _listItemService.DeleteListItem(authResult.User.Identity?.Name ?? "Unknown", itemDescription);

        return await Task.FromResult<ActionResult>(new OkObjectResult(userListItems));
    }
}
