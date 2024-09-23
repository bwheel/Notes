using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Notes.Data.Dao;
using Notes.Models;
using Notes.Services;
using Notes.Services.NotesService;
using Org.BouncyCastle.Utilities.Encoders;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;

namespace Notes.Controllers;

public class NotesController : Controller
{
    private readonly ILogger<NotesController> _logger;
    private readonly INotesService _notesService;

    public NotesController(ILogger<NotesController> logger, INotesService notesService)
    {
        _logger = logger;
        _notesService = notesService;
    }

    [HttpGet]
    public IActionResult Create(CancellationToken token)
    {
        return View("Create", new CreateNoteViewModel { Content = string.Empty });
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateNoteViewModel model, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            model.ErrorMessage = "Missing Content";
            return View(model);
        }

        var note = await _notesService.Create(model.Content, token);
        if (note == null)
        {
            model.ErrorMessage = "Unable to save to DB";
            return View(model);
        }

        var url = Base64UrlTextEncoder.Encode(Encoding.UTF8.GetBytes( note.Id));
         return Redirect(url);
    }


    [HttpGet("/{idEncoded}")]
    public async Task<IActionResult> Edit([FromRoute] string idEncoded, CancellationToken token, [FromQuery] string? returnUrl = null)
    {
        var id = Encoding.UTF8.GetString(Base64UrlTextEncoder.Decode(idEncoded));
        if(id == null)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl)) 
                return Redirect(returnUrl);
            return RedirectToAction("Create");
        }
        var note = await _notesService.Get(id, token);
        if (note == null)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Create");
        }
        var expirationIn = note.ExpireAt.Subtract(DateTime.UtcNow);
        return View("Edit", new EditNoteViewModel { Id = note.Id, Content = note.Content, ExpirationIn = expirationIn });
    }

    [HttpPost("/{id}")]
    public async Task<IActionResult> Edit([FromRoute] string id, EditNoteViewModel model, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            model.ErrorMessage = "Invalid Submission";
            return View(model);
        }
        if (model.Id != id)
        {
            model.ErrorMessage = "Note Id must match model";
            return View(model);
        }

        var note = await _notesService.Update(model.Id, model.Content, token);
        if (note == null)
        {
            model.ErrorMessage = "Unable to update in db";
            return View(model);
        }

        return RedirectToAction("Edit", "Notes", new { model.Id });
    }
}
