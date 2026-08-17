using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RescueHub.Infrastructure.SqlServer.Models;

namespace RescueHub.Infrastructure.SqlServer.Configurations
{
    public class TaskAssignmentConfiguration
    : IEntityTypeConfiguration<TaskAssignmentDataModel>
    {
        public void Configure(EntityTypeBuilder<TaskAssignmentDataModel> builder)
        {
            builder.ToTable("TaskAssignments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.AssignmentSource)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(x => x.AssignedAt)
                .HasColumnType("datetime2(7)")
                .IsRequired();

            builder.Property(x => x.AcceptedAt)
                .HasColumnType("datetime2(7)");

            builder.Property(x => x.ResponseAt)
                .HasColumnType("datetime2(7)");

            builder.Property(x => x.CompletedAt)
                .HasColumnType("datetime2(7)");

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetime2(7)")
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .HasColumnType("datetime2(7)");

            builder.Property(x => x.DeletedAt)
                .HasColumnType("datetime2(7)");

            builder.HasIndex(x => x.TaskId);
            builder.HasIndex(x => x.VolunteerId);
            builder.HasIndex(x => x.Status);

            builder.HasOne(x => x.Task)
                .WithMany(x => x.Assignments)
                .HasForeignKey(x => x.TaskId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Volunteer)
                .WithMany(x => x.TaskAssignments)
                .HasForeignKey(x => x.VolunteerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Assigner)
                .WithMany(x => x.AssignedTasks)
                .HasForeignKey(x => x.AssignedBy)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
