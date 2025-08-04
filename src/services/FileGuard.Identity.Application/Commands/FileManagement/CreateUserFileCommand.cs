using FileGuard.Crypto.Interfaces;
using FileGuard.Identity.Application.DTOs;
using FileGuard.Identity.Application.Exceptions;
using FileGuard.Identity.Application.Queries.FileManagement;
using FileGuard.Identity.DataAccess;
using FileGuard.Identity.DataAccess.Models;
using FileGuard.Shared.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using File = FileGuard.Identity.Models.File;

namespace FileGuard.Identity.Application.Commands.FileManagement;

public class CreateUserFileCommand(string userId, string fileName, byte[] fileContent, Guid? folderId) : IRequest<FileDto>
{
    public string UserId { get; set; } = userId;
    public string FileName { get; set; } = fileName;
    public byte[] FileContent { get; set; } = fileContent;
    public Guid? FolderId { get; set; } = folderId;
}

internal class CreateUserFileCommandHandler(FileGuardDbContext db, IMediator mediator, ICryptoClient crypto) : IRequestHandler<CreateUserFileCommand, FileDto>
{
    public async Task<FileDto> Handle(CreateUserFileCommand request, CancellationToken cancellationToken)
    {
        _ = await db.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
            ?? throw new UserNotFoundException($"User {request.UserId} not found.");

        string folderPath = null;

        if (request.FolderId.HasValue)
        {
            var folder = await db.Folders.FirstOrDefaultAsync(f => f.Id == request.FolderId.Value, cancellationToken)
                     ?? throw new FolderNotFoundException($"Folder with id = '{request.FolderId.Value}' was not found.");

            folderPath = await mediator.Send(new GetFolderPathQuery(folder.Id, request.UserId), cancellationToken);
        }

        var path = !string.IsNullOrEmpty(folderPath)
            ? Path.Combine(folderPath, request.FileName)
            : request.FileName;

        var file = new File
        {
            Id = Guid.NewGuid(),
            Name = request.FileName,
            Path = path,
            SizeInBytes = request.FileContent.Length,
            CreatedAt = DateTime.UtcNow,
            FolderId = request.FolderId
        };

        db.Files.Add(file);

        var userFile = new UserFile
        {
            UserId = request.UserId,
            FileId = file.Id,
            AccessLevel = AccessLevel.Owner
        };

        db.UserFiles.Add(userFile);

        await db.SaveChangesAsync(cancellationToken);

        await crypto.EncryptAsync(request.FileContent, Path.Combine(request.UserId.ToUserRootFolderName(), path), cancellationToken);

        var fileDto = new FileDto
        {
            Id = file.Id,
            FileName = file.Name,
            Path = path,
            SizeInBytes = file.SizeInBytes,
            CreatedAt = file.CreatedAt,
            FolderId = file.FolderId
        };

        return fileDto;
    }
}