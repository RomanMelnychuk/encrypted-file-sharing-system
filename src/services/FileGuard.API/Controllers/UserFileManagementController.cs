using System.Security.Cryptography.Xml;
using FileGuard.API.Models;
using FileGuard.API.Providers;
using FileGuard.Identity.Application.Commands.FileManagement;
using FileGuard.Identity.Application.DTOs;
using FileGuard.Identity.Application.Queries.FileManagement;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FileGuard.API.Controllers;

[ApiController]
[Route("api/FileManagement")]
public partial class UserFileManagementController(
    IMediator mediator, IUserProvider userProvider) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<UserFileSystemDto>> GetUserFilesAsync(CancellationToken cancellationToken)
    {
        var currentUserId = userProvider.GetCurrentUserId();

        var query = new GetUserTopLevelFileSystemQuery(currentUserId);

        var result = await mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return BadRequest("Query and UserId are required.");
        }

        var searchQuery = new FileManagementSearchQuery(searchTerm, userProvider.GetCurrentUserId());
        var result = await mediator.Send(searchQuery);

        return Ok(result);
    }
}
