using Domain.Entities.CoreEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Presistence.Data.Configuration
{
    public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
    {
        public void Configure(EntityTypeBuilder<TaskItem> builder)
        {
            builder.ToTable("Tasks");

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Description)
                .HasMaxLength(1000);

            builder.Property(t => t.Status)
                .HasConversion<int>();

            builder.Property(t => t.Priority)
                .HasConversion<int>();
        }
    }
}
