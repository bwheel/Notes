using Microsoft.EntityFrameworkCore;
using Notes.Data;
using Notes.Services;
using Notes.Services.NotesService;

namespace Notes;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("Default");
        ArgumentNullException.ThrowIfNullOrWhiteSpace(connectionString, nameof(connectionString));

        builder.Services.AddDbContext<NotesDbContext>(options => options.UseMySQL(connectionString));
        builder.Services.AddTransient<INotesService, NotesService>();
        //builder.Services.AddHostedService<CleanupHostedService>();

        // Add services to the container.

        //builder.Services.AddControllersWithViews();
        builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Notes}/{action=Create}/{id?}");

        app.Run();
    }
}
