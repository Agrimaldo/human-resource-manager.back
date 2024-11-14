
using HumanResourceManager.Domain.Entities;
using HumanResourceManager.Infra.Mapping;
using Microsoft.EntityFrameworkCore;

namespace HumanResourceManager.Infra.Context
{
    public  class PostgresqlContext : DbContext
    {
        public PostgresqlContext(DbContextOptions<PostgresqlContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserMap());
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> Users {  get; set; }
    }
}
