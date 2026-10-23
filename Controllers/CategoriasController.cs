using Microsoft.AspNetCore.Mvc;
using TarijaReadApp.Interfaces;
using TarijaReadApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace TarijaReadApp.Controllers;

[Authorize]
public class CategoriasController : Controller
{
    private readonly IRepository<Categoria> _repository;

    public CategoriasController(IRepository<Categoria> repository)
    {
        _repository = repository;
    }

    // GET: Categorias
    public async Task<IActionResult> Index()
    {
        var categorias = await _repository.GetAllAsync();
        return View(categorias);
    }

    // GET: Categorias/Create
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Categorias/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Categoria categoria)
    {
        if (!ModelState.IsValid)
            return View(categoria);

        await _repository.AddAsync(categoria);
        await _repository.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Categorias/Edit/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var categoria = await _repository.GetByIdAsync(id);
        if (categoria == null) return NotFound();
        return View(categoria);
    }

    // POST: Categorias/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, Categoria categoria)
    {
        if (id != categoria.Id) return NotFound();
        if (!ModelState.IsValid) return View(categoria);

        _repository.Update(categoria);
        await _repository.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Categorias/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var categoria = await _repository.GetByIdAsync(id);
        if (categoria == null) return NotFound();
        return View(categoria);
    }

    // POST: Categorias/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var categoria = await _repository.GetByIdAsync(id);
        if (categoria != null)
        {
            _repository.Remove(categoria);
            await _repository.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}