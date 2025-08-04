using FileGuard.Identity.Application.DTOs;
using FileGuard.Identity.Application.Exceptions;
using FileGuard.Identity.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FileGuard.Identity.Application.Queries.User;

public class GetUserQuery(string id) : IRequest<UserDto>
{
    public string Id { get; set; } = id;
}

internal class GetUserQueryHandler(FileGuardDbContext db) : IRequestHandler<GetUserQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken: cancellationToken)
                   ?? throw new UserNotFoundException($"User not found by id = '{request.Id}'");

        return new UserDto
        {
            Email = user.Email,
            UserName = user.UserName,
            Id = user.Id,
        };
    }
}
