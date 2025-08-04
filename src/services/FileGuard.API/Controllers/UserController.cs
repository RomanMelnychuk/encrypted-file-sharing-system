using FileGuard.API.Providers;
using FileGuard.Identity.Application.DTOs;
using FileGuard.Identity.Application.Queries.User;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FileGuard.API.Controllers;

[ApiController]
[Route("api/[controller]")]

public class UserController(IMediator mediator, IUserProvider provider) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<UserDto>> GetUserAsync(CancellationToken cancellationToken)
    {
        var user = await mediator.Send(new GetUserQuery(provider.GetCurrentUserId()), cancellationToken);

        return Ok(user);
    }

    [HttpGet("all")]
    public async Task<ActionResult<IList<UserDto>>> GetUsersAsync(CancellationToken cancellationToken)
    {
        var user = await mediator.Send(new GetUsersQuery(), cancellationToken);

        return Ok(user);
    }
}
