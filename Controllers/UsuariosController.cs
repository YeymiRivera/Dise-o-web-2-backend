using Microsoft.AspNetCore.Mvc;
using TarijaReadApp.Interfaces;
using TarijaReadApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace TarijaReadApp.Controllers;

[Authorize]

public class UsuariosController : Controller
{
    private readonly IRepository<Usuario> _repository;

    public UsuariosController(IRepository<Usuario> repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> Index() => View(await _repository.GetAllAsync());

    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Usuario usuario)
    {
        if (!ModelState.IsValid) return View(usuario);
        await _repository.AddAsync(usuario);
        await _repository.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var usuario = await _repository.GetByIdAsync(id);
        if (usuario == null) return NotFound();
        return View(usuario);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, Usuario usuario)
    {
        if (id != usuario.Id) return NotFound();
        if (!ModelState.IsValid) return View(usuario);
        _repository.Update(usuario);
        await _repository.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var usuario = await _repository.GetByIdAsync(id);
        if (usuario == null) return NotFound();
        return View(usuario);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var usuario = await _repository.GetByIdAsync(id);
        if (usuario != null)
        {
            _repository.Remove(usuario);
            await _repository.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}