using System.Threading.Tasks;
using api.Services;
using Api.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Api.Functions;

public class AuthFunctions
{
    private readonly IAuthService _authService;

    public AuthFunctions(IAuthService authService)
    {
        _authService = authService;
    }

    [FunctionName("Authenticate")]
    public async Task<ActionResult<AuthResult>> Authenticate(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("Authenticate endpoint triggered.");

        var authResult = await this._authService.Authenticate(req);
        if (!authResult.IsAuthenticated)
        {
            log.LogWarning("User not authenticated.");
            return await Task.FromResult<ActionResult>(new UnauthorizedResult());
        }

        return await Task.FromResult<ActionResult>(new OkObjectResult(authResult));
    }
}
