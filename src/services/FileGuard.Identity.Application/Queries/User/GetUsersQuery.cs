using FileGuard.Identity.Application.DTOs;
using FileGuard.Identity.Application.Exceptions;
using FileGuard.Identity.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FileGuard.Identity.Application.Queries.User;

public class GetUsersQuery : IRequest<IList<UserDto>>
{
}

internal class GetUsersQueryHandler(FileGuardDbContext db) : IRequestHandler<GetUsersQuery, IList<UserDto>>
{
    public async Task<IList<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await db.Users
            .Select(user => new UserDto
            {
                Email = user.Email,
                UserName = user.UserName,
                Id = user.Id,
            })
            .ToListAsync(cancellationToken: cancellationToken);

        return users;
    }
}