using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Notes.Data;

namespace Notes.Services;

public class CleanupHostedService : IHostedService, IDisposable
{
    private readonly ILogger<CleanupHostedService> _logger;
    private readonly IServiceProvider _services;
    private Timer? _timer = null;

    public CleanupHostedService(ILogger<CleanupHostedService> logger, IServiceProvider services)
    {
        _logger = logger;
        _services = services;
    }

    public Task StartAsync(CancellationToken _)
    {
        _timer = new Timer(doWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
        return Task.CompletedTask;
    }

    private async void doWork(object? state)
    {
        try
        {
            var now = DateTime.UtcNow; ;
            using var scope = _services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<NotesDbContext>();
            var notes = dbContext.Notes.Where(n => n.ExpireAt >= now);
            foreach (var note in notes)
            {
                dbContext.Notes.Remove(note);
                await dbContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in cleaning up");
        }
    }

    public Task StopAsync(CancellationToken _)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
