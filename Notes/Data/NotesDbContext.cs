using Microsoft.EntityFrameworkCore;
using Notes.Data.Dao;

namespace Notes.Data;

public class NotesDbContext : DbContext
{
    public DbSet<Note> Notes { get; set; }
    public NotesDbContext(DbContextOptions<NotesDbContext> options)
        : base(options)
    { }
}
