namespace Infrastructure;

public class LoyaltyContext : DbContext
{
    public LoyaltyContext(DbContextOptions<LoyaltyContext> options) : base(options)
    {
    }

    public LoyaltyContext()
    {
        
    }
    public DbSet<User> Users { get; set; }
    public DbSet<PointTransaction> PointTransactions { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Points).HasDefaultValue(0);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.HasMany(e => e.Transactions)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PointTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Points).IsRequired();
            entity.Property(e => e.Type).HasConversion<string>();
            entity.Property(e => e.Timestamp).IsRequired();
        });
    }
}