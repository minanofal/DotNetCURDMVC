using DotNetCore6CRUD.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotNetCore6CRUD.Configurations
{
    public class GenreEntityTypeConfiguration : IEntityTypeConfiguration<Genre>
    {
        public void Configure(EntityTypeBuilder<Genre> builder)
        {
            builder.HasKey(G => G.Id);

            builder.Property(G => G.Name).HasMaxLength(100).IsRequired();
        }
    }
}
