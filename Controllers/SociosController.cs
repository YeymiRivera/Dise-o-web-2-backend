using Microsoft.AspNetCore.Mvc;
using TarijaReadApp.Interfaces;
using TarijaReadApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace TarijaReadApp.Controllers;

[Authorize]
public class SociosController : Controller
{
    private readonly IRepository<Socio> _repository;

    public SociosController(IRepository<Socio> repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> Index() => View(await _repository.GetAllAsync());

    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Socio socio)
    {
        if (!ModelState.IsValid) return View(socio);
        await _repository.AddAsync(socio);
        await _repository.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var socio = await _repository.GetByIdAsync(id);
        if (socio == null) return NotFound();
        return View(socio);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, Socio socio)
    {
        if (id != socio.Id) return NotFound();
        if (!ModelState.IsValid) return View(socio);
        _repository.Update(socio);
        await _repository.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var socio = await _repository.GetByIdAsync(id);
        if (socio == null) return NotFound();
        return View(socio);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var socio = await _repository.GetByIdAsync(id);
        if (socio != null)
        {
            _repository.Remove(socio);
            await _repository.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}