namespace GameServer.Infrastructure.Persistence;

public sealed class UserEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }
}
