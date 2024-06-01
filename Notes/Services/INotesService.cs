using Notes.Data.Dao;

namespace Notes.Services;

public interface INotesService
{
    Task<Note?> Create(string content, CancellationToken token);
    Task<Note?> Get(string id, CancellationToken token);
    Task<Note?> Update(string id, string content, CancellationToken token);
}
