using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PeopleManagement.Domain.Entities;

namespace PeopleManagement.Infrastructure.Configuration
{
    public class TokenConfiguration : IEntityTypeConfiguration<Token>
    {
        public void Configure(EntityTypeBuilder<Token> builder)
        {
            object value = builder.ToTable("Tokens");

            // Primary key
            builder.HasKey(x => x.AccessToken);

            // Properties
            builder.Property(x => x.AccessToken).IsRequired();
            builder.Property(x => x.Expiration).IsRequired();
            builder.Property(x => x.UserId).IsRequired();
        }
    }
}