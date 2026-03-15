using GameServer.Application.Abstractions;
using GameServer.Domain.Users;
using GameServer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Infrastructure.Users;

public sealed class UserRepository(GameServerDbContext dbContext) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        return User.CreateNew(entity.Id, entity.Name, entity.CreatedAtUtc);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        var entity = new UserEntity
        {
            Id = user.Id,
            Name = user.Name,
            CreatedAtUtc = user.CreatedAtUtc
        };

        await dbContext.Users.AddAsync(entity, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
