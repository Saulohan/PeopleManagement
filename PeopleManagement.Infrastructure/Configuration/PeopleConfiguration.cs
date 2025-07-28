using PeopleManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PeopleManagement.Infrastructure.Configuration
{
    public class PeopleConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.ToTable("People");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            
            // Basic properties
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(200);
            builder.Property(x => x.PasswordHash).IsRequired().HasMaxLength(512);
            builder.Property(x => x.Gender).IsRequired();
            builder.Property(x => x.DateOfBirth).IsRequired();
            
            // Optional properties
            builder.Property(x => x.Naturality).HasMaxLength(200);
            builder.Property(x => x.Nationality).HasMaxLength(200);
            builder.Property(x => x.CPF).HasMaxLength(11);
            
            // Base entity properties
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();
            builder.Property(x => x.DeletionAt);

            // Indexes
            builder.HasIndex(x => x.CPF).IsUnique();
        }
    }
}