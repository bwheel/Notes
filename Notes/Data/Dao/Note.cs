using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Notes.Data.Dao;

[EntityTypeConfiguration(typeof(NoteConfiguration))]
public class Note
{
    public required string Id { get; set; }
    public required string Content { get; set; }
    public DateTime ExpireAt { get; set; }
}


public class NoteConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.ToTable("NOTES");

        // Primary Key
        builder.HasKey(n => n.Id);

        // Property Configurations
        builder.Property(n => n.Id)
               .IsRequired()
               .HasColumnName("ID");

        builder.Property(n => n.Content)
               .IsRequired()
               .HasColumnType("TEXT")
               .HasColumnName("CONTENT");

        builder.Property(n => n.ExpireAt)
               .IsRequired()
               .HasColumnName("EXPIRE_AT");
    }
}