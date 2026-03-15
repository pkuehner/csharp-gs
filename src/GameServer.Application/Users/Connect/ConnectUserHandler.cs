using GameServer.Application.Abstractions;
using GameServer.Domain.Users;

namespace GameServer.Application.Users.Connect;

public sealed class ConnectUserHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IUserNameGenerator _userNameGenerator;
    private readonly IUuidGenerator _uuidGenerator;
    private readonly IClock _clock;

    public ConnectUserHandler(
        IUserRepository userRepository,
        IUserNameGenerator userNameGenerator,
        IUuidGenerator uuidGenerator,
        IClock clock)
    {
        _userRepository = userRepository;
        _userNameGenerator = userNameGenerator;
        _uuidGenerator = uuidGenerator;
        _clock = clock;
    }

    public async Task<ConnectUserResult> HandleAsync(ConnectUserRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.UserId))
        {
            var userId = _uuidGenerator.NewUuid();
            var randomName = _userNameGenerator.Generate();
            var user = User.CreateNew(userId, randomName, _clock.UtcNow);

            await _userRepository.AddAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            return ConnectUserResult.Success(user.Id, user.Name, isNewUser: true);
        }

        if (!Guid.TryParse(request.UserId, out var parsedUserId) || parsedUserId == Guid.Empty)
        {
            return ConnectUserResult.Failure("INVALID_USER_ID", "User ID must be a valid UUID.");
        }

        var existingUser = await _userRepository.GetByIdAsync(parsedUserId, cancellationToken);
        if (existingUser is null)
        {
            return ConnectUserResult.Failure("USER_NOT_FOUND", "User ID does not exist.");
        }

        return ConnectUserResult.Success(existingUser.Id, existingUser.Name, isNewUser: false);
    }
}
