using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RescueHub.Infrastructure.SqlServer.Models;

namespace RescueHub.Infrastructure.SqlServer.Configurations
{
    public class VolunteerSkillConfiguration : IEntityTypeConfiguration<VolunteerSkillDataModel>
    {
        public void Configure(EntityTypeBuilder<VolunteerSkillDataModel> builder)
        {
            builder.ToTable("VolunteerSkills");

            builder.HasKey(x => new
            {
                x.VolunteerId,
                x.SkillId
            });

            builder.Property(x => x.Level)
                .IsRequired();

            builder.HasOne(x => x.Volunteer)
                .WithMany(x => x.VolunteerSkills)
                .HasForeignKey(x => x.VolunteerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Skill)
                .WithMany(x => x.VolunteerSkills)
                .HasForeignKey(x => x.SkillId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
