using DotNetCore6CRUD.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotNetCore6CRUD.Configurations
{
    public class MovieEntityTypeConfiguration : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.HasKey(M => M.Id);

            builder.Property(M => M.Title).HasMaxLength(250).IsRequired();

            builder.Property(M => M.Storeline).HasMaxLength(2500).IsRequired();

            builder.Property(M=>M.Poster).IsRequired();

            builder.HasOne(M => M.Genre)
                .WithMany(G => G.movies)
                .HasForeignKey(M => M.GenreId);

        }
    }
}
