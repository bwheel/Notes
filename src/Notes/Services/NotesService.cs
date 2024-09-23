
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Notes.Data;
using Notes.Data.Dao;
using Notes.Utils;

namespace Notes.Services.NotesService;

public class NotesService : INotesService
{
    private readonly NotesDbContext _notesDbContext;
    private readonly IHostEnvironment _env;
    private readonly ILogger<NotesService> _logger;

    public NotesService(ILogger<NotesService> logger, NotesDbContext notesDbContext, IHostEnvironment env)
    {
        _logger = logger;
        _notesDbContext = notesDbContext;
        _env = env;
    }

    public async Task<Note?> Create(string content, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(content);
        // TODO: validate the upper limit of the content size.

        var expireAt = _env.IsDevelopment()
            ? DateTime.UtcNow.AddMinutes(5)
            : DateTime.UtcNow.AddMinutes(30).AddSeconds(59);
        // TODO: figure out expireAt based on account.

        var note = new Note
        {
            Id = content.HashWithDate(expireAt),
            Content = content,
            ExpireAt = expireAt,
        };
        await _notesDbContext.Notes.AddAsync(note, token);
        var x = await _notesDbContext.SaveChangesAsync(token);
        if (x < 1)
            return null;

        return note;
    }

    public async Task<Note?> Get(string id, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id, nameof(id));
        var note = await _notesDbContext.Notes.FirstOrDefaultAsync(n => n.Id == id, token);

        return note;
    }

    public async Task<Note?> Update(string id, string content, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(content, nameof(content));
        ArgumentException.ThrowIfNullOrWhiteSpace(id, nameof(id));
        var note = await _notesDbContext.Notes.FirstOrDefaultAsync((n) => n.Id == id, token);
        if (note == null)
            return null;
        note.Content = content;
        int count = await _notesDbContext.SaveChangesAsync(token);
        if (count >= 1)
            return note;
        return null;
    }

}