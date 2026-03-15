using GameServer.Application.Abstractions;

namespace GameServer.Infrastructure;

public sealed class UuidGenerator : IUuidGenerator
{
    public Guid NewUuid() => Guid.NewGuid();
}
