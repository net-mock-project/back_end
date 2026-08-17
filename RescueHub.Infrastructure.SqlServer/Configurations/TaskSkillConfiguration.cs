using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RescueHub.Infrastructure.SqlServer.Models;

namespace RescueHub.Infrastructure.SqlServer.Configurations
{
    public class TaskSkillConfiguration : IEntityTypeConfiguration<TaskSkillDataModel>
    {
        public void Configure(EntityTypeBuilder<TaskSkillDataModel> builder)
        {
            builder.ToTable("TaskSkills");

            builder.HasKey(x => new
            {
                x.TaskId,
                x.SkillId
            });

            builder.HasOne(x => x.Task)
                .WithMany(x => x.TaskSkills)
                .HasForeignKey(x => x.TaskId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Skill)
                .WithMany(x => x.TaskSkills)
                .HasForeignKey(x => x.SkillId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
