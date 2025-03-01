using HMD.TaskManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMD.TaskManagement.Persistence.Configurations
{
    public class AppRoleConfiguration : IEntityTypeConfiguration<AppRole>
    {
        public void Configure(EntityTypeBuilder<AppRole> builder)
        {
            builder.Property(x => x.Defination).IsRequired(true);
            builder.Property(x => x.Defination).HasMaxLength(150);

            builder.HasMany(x => x.Users).WithOne(x => x.Role).HasForeignKey(x => x.AppRoleId);
        }
    }
}
