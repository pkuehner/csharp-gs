namespace GameServer.Application.Users.Connect;

public sealed record ConnectUserResult(
    bool IsSuccess,
    string? ErrorCode,
    string? ErrorMessage,
    Guid UserId,
    string? Name,
    bool IsNewUser)
{
    public static ConnectUserResult Success(Guid userId, string name, bool isNewUser) =>
        new(true, null, null, userId, name, isNewUser);

    public static ConnectUserResult Failure(string errorCode, string errorMessage) =>
        new(false, errorCode, errorMessage, Guid.Empty, null, false);
}
