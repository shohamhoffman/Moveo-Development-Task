using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Config
{
    // Configurations of the Doctor table in moveo.db.
    public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.Property(d => d.Id).IsRequired();
            builder.Property(d => d.Name).IsRequired();
            builder.Property(d => d.Email).IsRequired();
            builder.Property(d => d.DoctorField).IsRequired();
        }
    }
}