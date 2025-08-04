using FileGuard.Identity.Application.Commands.FileManagement;
using FileGuard.Identity.Application.DTOs;
using FileGuard.Identity.Application.Queries.FileManagement;
using Microsoft.AspNetCore.Mvc;

namespace FileGuard.API.Controllers;

public partial class UserFileManagementController : ControllerBase
{
    [HttpGet("file/{fileId}")]
    public async Task<ActionResult> DownloadFileAsync(Guid fileId, CancellationToken cancellationToken)
    {
        var query = new GetUserFileQuery(userProvider.GetCurrentUserId(), fileId);

        var (fileName, content) = await mediator.Send(query, cancellationToken);

        return File(content, "application/octet-stream", fileName);
    }

    [HttpPost("file")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<FileDto>> CreateFileAndEncryptAsync([FromQuery] Guid? folderId, [FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        if (file.Length == 0)
            return BadRequest(new { message = "File is not loaded." });

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms, cancellationToken);
        var fileBytes = ms.ToArray();

        var command = new CreateUserFileCommand(userProvider.GetCurrentUserId(), fileName: file.FileName, fileBytes, folderId);

        var result = await mediator.Send(command, cancellationToken);

        return Ok(result);
    }

    [HttpPost("files")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<List<FolderDto>>> CreateFilesAndEncryptAsync([FromQuery] Guid? folderId, [FromForm] List<IFormFile> files, CancellationToken cancellationToken)
    {
        if (!files.Any())
            return BadRequest(new { message = "No files were uploaded." });

        var results = new List<FileDto>();

        foreach (var file in files)
        {
            if (file.Length == 0)
            {
                continue;
            }

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms, cancellationToken);
            var fileBytes = ms.ToArray();

            var command = new CreateUserFileCommand(userProvider.GetCurrentUserId(), fileName: file.FileName, fileBytes, folderId);

            var result = await mediator.Send(command, cancellationToken);
            results.Add(result);
        }

        if (!results.Any())
        {
            return BadRequest(new { message = "No valid files were uploaded." });
        }

        return Ok(results);
    }

    [HttpPost("file/{fileId}/share/{userId}")]
    public async Task<ActionResult> ShareFileWithUserAsync(Guid fileId, string userId, CancellationToken cancellationToken)
    {
        var command = new ShareFileCommand(fileId, userProvider.GetCurrentUserId(), userId);

        await mediator.Send(command, cancellationToken);

        return Ok();
    }

    [HttpDelete("file/{fileId}")]
    public async Task<ActionResult> DeleteAsync(Guid fileId, CancellationToken cancellationToken)
    {
        var command = new DeleteFileCommand(fileId, userProvider.GetCurrentUserId());

        await mediator.Send(command, cancellationToken);

        return NoContent();
    }
 }
