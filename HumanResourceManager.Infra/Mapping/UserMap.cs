
using HumanResourceManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HumanResourceManager.Infra.Mapping
{
    public class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("tb_user");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("id").ValueGeneratedOnAdd();
            builder.Property(p => p.Name).HasColumnName("name").HasMaxLength(100);
            builder.Property(p => p.Birthdate).HasColumnName("birthdate");
            builder.Property(p => p.Email).HasColumnName("email");
            builder.Property(p => p.Password).HasColumnName("password");
        }
    }
}
