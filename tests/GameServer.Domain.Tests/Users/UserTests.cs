using FluentAssertions;
using GameServer.Domain.Users;
using Xunit;

namespace GameServer.Domain.Tests.Users;

public sealed class UserTests
{
    [Fact]
    public void CreateNew_ShouldCreateUser_WhenInputIsValid()
    {
        var id = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var user = User.CreateNew(id, "  PlayerOne  ", now);

        user.Id.Should().Be(id);
        user.Name.Should().Be("PlayerOne");
        user.CreatedAtUtc.Should().Be(now);
    }

    [Fact]
    public void CreateNew_ShouldThrow_WhenNameIsEmpty()
    {
        var act = () => User.CreateNew(Guid.NewGuid(), "", DateTime.UtcNow);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CreateNew_ShouldThrow_WhenIdIsEmptyGuid()
    {
        var act = () => User.CreateNew(Guid.Empty, "ValidName", DateTime.UtcNow);

        act.Should().Throw<ArgumentException>();
    }
}
