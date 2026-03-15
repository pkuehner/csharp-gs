namespace GameServer.Domain.Users;

public sealed class User
{
    private User(Guid id, string name, DateTime createdAtUtc)
    {
        Id = id;
        Name = name;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; }

    public string Name { get; private set; }

    public DateTime CreatedAtUtc { get; }

    public static User CreateNew(Guid id, string name, DateTime createdAtUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("User id must not be empty.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("User name must not be empty.", nameof(name));
        }

        return new User(id, name.Trim(), createdAtUtc);
    }
}
