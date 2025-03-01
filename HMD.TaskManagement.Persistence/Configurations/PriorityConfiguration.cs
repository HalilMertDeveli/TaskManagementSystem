using HMD.TaskManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMD.TaskManagement.Persistence.Configurations
{
    public class PriorityConfiguration : IEntityTypeConfiguration<Priority>
    {
        public void Configure(EntityTypeBuilder<Priority> builder)
        {
            builder.Property(x => x.Defination).IsRequired(true);
            builder.Property(x => x.Defination).HasMaxLength(250);

            builder.HasMany(x => x.Tasks).WithOne(x => x.Priority).HasForeignKey(x => x.PriorityId);
        }
    }
}
