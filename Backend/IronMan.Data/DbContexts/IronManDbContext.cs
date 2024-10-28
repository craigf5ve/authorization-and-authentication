using System.Text.Json;
using IronMan.Data.AggregateRoots;
using IronMan.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace IronMan.Data.DbContexts
{
    public class IronManDbContext : DbContext
    {
        // DbSets for each of your entity classes
        public DbSet<Account> Accounts { get; set; }
       
        public IronManDbContext(DbContextOptions<IronManDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureAuditedEntity<Account>(modelBuilder);

            AddSeedData<Account>(modelBuilder);
        }

        private static void AddSeedData<TEntity>(ModelBuilder modelBuilder) where TEntity : class
        {
            // Read the JSON file whose path is classname.json
            var jsonData = System.IO.File.ReadAllText($"{typeof(TEntity).Name}.json");

            // Deserialize it to your entity type
            var entities = JsonSerializer.Deserialize<List<TEntity>>(jsonData);

            // Seed the data
            modelBuilder.Entity<TEntity>().HasData(entities!);
        }

        private static void ConfigureAuditedEntity<TEntity>(ModelBuilder modelBuilder) where TEntity : AuditedAggregateRoot<int>
        {
            modelBuilder.Entity<TEntity>(entity =>
            {
                // Configure the relationship for Creator
                entity.HasOne<Account>("Creator")
                      .WithMany()
                      .HasForeignKey("CreatorId")
                      .OnDelete(DeleteBehavior.Restrict);

                // Configure the relationship for Deleter
                entity.HasOne<Account>("Deleter")
                      .WithMany()
                      .HasForeignKey("DeleterId")
                      .OnDelete(DeleteBehavior.Restrict);

                // If the entity is of type FullAuditedAggregateRoot, configure LastModifierUser
                if (typeof(FullAuditedAggregateRoot<int>).IsAssignableFrom(typeof(TEntity)))
                {
                    entity.HasOne<Account>("LastModifierUser")
                          .WithMany()
                          .HasForeignKey("LastModifierUserId")
                          .OnDelete(DeleteBehavior.Restrict);
                }
            });
        }
    }
}
