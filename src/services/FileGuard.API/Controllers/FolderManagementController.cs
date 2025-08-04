using FileGuard.API.Models;
using FileGuard.Identity.Application.Commands.FileManagement;
using FileGuard.Identity.Application.DTOs;
using FileGuard.Identity.Application.Queries.FileManagement;
using Microsoft.AspNetCore.Mvc;

namespace FileGuard.API.Controllers;

public partial class UserFileManagementController
{

    [HttpPost("folder")]
    public async Task<ActionResult<FolderDto>> CreateUserFolderAsync(CreateFolderRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.FolderName))
            return BadRequest("A name is required to create a folder");

        var command = new CreateFolderCommand(request.FolderName, userProvider.GetCurrentUserId(), request.ParentFolderId);

        var result = await mediator.Send(command, cancellationToken);

        return Ok(result);
    }

    [HttpDelete("folder/{folderId:guid}")]
    public async Task<ActionResult<FolderDto>> DeleteFolderAsync(Guid folderId, CancellationToken cancellationToken)
    {
        var command = new DeleteFolderCommand(folderId, userProvider.GetCurrentUserId());

        await mediator.Send(command, cancellationToken);

        return Ok();
    }

    [HttpGet("folder/{folderId}")]
    public async Task<ActionResult<FolderDto>> GetFolderItemsAsync(Guid folderId, CancellationToken cancellationToken)
    {
        var query = new GetFolderQuery(folderId, userProvider.GetCurrentUserId());

        var result = await mediator.Send(query, cancellationToken);

        return Ok(result);
    }


    [HttpPost("folder/{folderId}/share/{userId}")]
    public async Task<ActionResult> ShareFolderWithUserAsync(Guid folderId, string userId, CancellationToken cancellationToken)
    {
        var command = new ShareFolderCommand(folderId, userProvider.GetCurrentUserId(), userId);

        await mediator.Send(command, cancellationToken);

        return Ok();
    }
}
