using GameServer.Application.Abstractions;

namespace GameServer.Infrastructure;

public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
