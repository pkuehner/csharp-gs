using GameServer.Application.Abstractions;

namespace GameServer.Infrastructure;

public sealed class RandomUserNameGenerator : IUserNameGenerator
{
    private static readonly string[] Prefixes = ["Swift", "Bold", "Wise", "Mighty", "Clever", "Lucky"];
    private static readonly string[] Suffixes = ["Fox", "Rook", "Knight", "Bishop", "Panda", "Otter"];

    public string Generate()
    {
        var random = Random.Shared;
        var prefix = Prefixes[random.Next(Prefixes.Length)];
        var suffix = Suffixes[random.Next(Suffixes.Length)];
        var number = random.Next(1000, 9999);
        return $"{prefix}{suffix}{number}";
    }
}
