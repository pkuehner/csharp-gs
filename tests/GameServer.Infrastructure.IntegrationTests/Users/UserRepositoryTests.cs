using FluentAssertions;
using GameServer.Domain.Users;
using GameServer.Infrastructure.Persistence;
using GameServer.Infrastructure.Users;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GameServer.Infrastructure.IntegrationTests.Users;

public sealed class UserRepositoryTests
{
    [Fact]
    public async Task AddAndGetByIdAsync_ShouldRoundtripUser_WhenUserExists()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<GameServerDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var dbContext = new GameServerDbContext(options);
        await dbContext.Database.EnsureCreatedAsync();

        var repository = new UserRepository(dbContext);
        var user = User.CreateNew(Guid.NewGuid(), "SQLiteUser", DateTime.UtcNow);

        await repository.AddAsync(user, CancellationToken.None);
        await repository.SaveChangesAsync(CancellationToken.None);

        var loaded = await repository.GetByIdAsync(user.Id, CancellationToken.None);

        loaded.Should().NotBeNull();
        loaded!.Id.Should().Be(user.Id);
        loaded.Name.Should().Be(user.Name);
    }
}
