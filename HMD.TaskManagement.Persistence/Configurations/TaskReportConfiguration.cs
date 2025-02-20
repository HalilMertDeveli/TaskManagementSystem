using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMD.TaskManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMD.TaskManagement.Persistence.Configurations
{
    public class TaskReportConfiguration:IEntityTypeConfiguration<TaskReport>
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
