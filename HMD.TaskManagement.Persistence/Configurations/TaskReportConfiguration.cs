using HMD.TaskManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMD.TaskManagement.Persistence.Configurations
{
    public class TaskReportConfiguration : IEntityTypeConfiguration<TaskReport>
    {
        public void Configure(EntityTypeBuilder<TaskReport> builder)
        {
            builder.Property(x => x.Detail).IsRequired(true);
            builder.Property(x => x.Defination).IsRequired(true);
            builder.Property(x => x.Defination).HasMaxLength(250);
            builder.Property(x => x.AppTaskId).IsRequired(true);
        }
    }
}
