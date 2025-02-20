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
    public class PriorityConfiguration:IEntityTypeConfiguration<Priority>
    {
        public void Configure(EntityTypeBuilder<Priority> builder)
        {
            builder.Property(x => x.Defination).IsRequired(true);
            builder.Property(x => x.Defination).HasMaxLength(250);

            builder.HasMany(x => x.Tasks).WithOne(x => x.Priority).HasForeignKey(x => x.PriorityId);
        }
    }
}
