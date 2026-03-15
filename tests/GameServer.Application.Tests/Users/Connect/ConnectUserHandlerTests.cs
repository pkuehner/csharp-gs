using FluentAssertions;
using GameServer.Application.Abstractions;
using GameServer.Application.Users.Connect;
using GameServer.Domain.Users;
using Moq;
using Xunit;

namespace GameServer.Application.Tests.Users.Connect;

public sealed class ConnectUserHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldCreateUserWithRandomName_WhenUserIdIsMissing()
    {
        var repository = new Mock<IUserRepository>();
        var nameGenerator = new Mock<IUserNameGenerator>();
        var uuidGenerator = new Mock<IUuidGenerator>();
        var clock = new Mock<IClock>();

        var generatedId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        nameGenerator.Setup(x => x.Generate()).Returns("SwiftFox1234");
        uuidGenerator.Setup(x => x.NewUuid()).Returns(generatedId);
        clock.Setup(x => x.UtcNow).Returns(now);

        var handler = new ConnectUserHandler(repository.Object, nameGenerator.Object, uuidGenerator.Object, clock.Object);

        var result = await handler.HandleAsync(new ConnectUserRequest(null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.IsNewUser.Should().BeTrue();
        result.UserId.Should().Be(generatedId);
        result.Name.Should().Be("SwiftFox1234");
        result.ErrorCode.Should().BeNull();
        repository.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnUserNotFound_WhenProvidedUserIdDoesNotExist()
    {
        var repository = new Mock<IUserRepository>();
        var nameGenerator = new Mock<IUserNameGenerator>();
        var uuidGenerator = new Mock<IUuidGenerator>();
        var clock = new Mock<IClock>();

        var missingUserId = Guid.NewGuid();
        repository
            .Setup(x => x.GetByIdAsync(missingUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = new ConnectUserHandler(repository.Object, nameGenerator.Object, uuidGenerator.Object, clock.Object);

        var result = await handler.HandleAsync(new ConnectUserRequest(missingUserId.ToString()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("USER_NOT_FOUND");
        result.ErrorMessage.Should().Be("User ID does not exist.");
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnExistingUser_WhenProvidedUserIdExists()
    {
        var repository = new Mock<IUserRepository>();
        var nameGenerator = new Mock<IUserNameGenerator>();
        var uuidGenerator = new Mock<IUuidGenerator>();
        var clock = new Mock<IClock>();

        var existingId = Guid.NewGuid();
        var existingUser = User.CreateNew(existingId, "PersistedName", DateTime.UtcNow);

        repository
            .Setup(x => x.GetByIdAsync(existingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        var handler = new ConnectUserHandler(repository.Object, nameGenerator.Object, uuidGenerator.Object, clock.Object);

        var result = await handler.HandleAsync(new ConnectUserRequest(existingId.ToString()), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.IsNewUser.Should().BeFalse();
        result.UserId.Should().Be(existingId);
        result.Name.Should().Be("PersistedName");
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnInvalidUserId_WhenProvidedUserIdIsNotGuid()
    {
        var repository = new Mock<IUserRepository>();
        var nameGenerator = new Mock<IUserNameGenerator>();
        var uuidGenerator = new Mock<IUuidGenerator>();
        var clock = new Mock<IClock>();

        var handler = new ConnectUserHandler(repository.Object, nameGenerator.Object, uuidGenerator.Object, clock.Object);

        var result = await handler.HandleAsync(new ConnectUserRequest("not-a-guid"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("INVALID_USER_ID");
    }
}
