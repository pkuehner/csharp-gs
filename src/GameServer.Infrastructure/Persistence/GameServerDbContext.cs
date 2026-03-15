using Microsoft.EntityFrameworkCore;

namespace GameServer.Infrastructure.Persistence;

public sealed class GameServerDbContext(DbContextOptions<GameServerDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users => Set<UserEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var user = modelBuilder.Entity<UserEntity>();
        user.ToTable("Users");
        user.HasKey(x => x.Id);
        user.Property(x => x.Id).IsRequired();
        user.Property(x => x.Name).HasMaxLength(64).IsRequired();
        user.Property(x => x.CreatedAtUtc).IsRequired();
    }
}
