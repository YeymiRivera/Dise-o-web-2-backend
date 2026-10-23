using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TarijaReadApp.Interfaces;
using TarijaReadApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace TarijaReadApp.Controllers;

[Authorize]

public class EjemplaresController : Controller
{
    private readonly IEjemplarRepository _repository;
    private readonly ILibroRepository _libroRepository;

    public EjemplaresController(IEjemplarRepository repository, ILibroRepository libroRepository)
    {
        _repository = repository;
        _libroRepository = libroRepository;
    }

    public async Task<IActionResult> Index() => View(await _repository.GetAllAsync());


    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        await CargarLibros();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Ejemplar ejemplar)
    {
        if (!ModelState.IsValid)
        {
            await CargarLibros(ejemplar.LibroId);
            return View(ejemplar);
        }

        await _repository.AddAsync(ejemplar);
        await _repository.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var ejemplar = await _repository.GetByIdAsync(id);
        if (ejemplar == null) return NotFound();
        await CargarLibros(ejemplar.LibroId);
        return View(ejemplar);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, Ejemplar ejemplar)
    {
        if (id != ejemplar.Id) return NotFound();
        if (!ModelState.IsValid)
        {
            await CargarLibros(ejemplar.LibroId);
            return View(ejemplar);
        }

        _repository.Update(ejemplar);
        await _repository.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var ejemplar = await _repository.GetByIdAsync(id);
        if (ejemplar == null) return NotFound();
        return View(ejemplar);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var ejemplar = await _repository.GetByIdAsync(id);
        if (ejemplar != null)
        {
            _repository.Remove(ejemplar);
            await _repository.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarLibros(int? libroSeleccionado = null)
    {
        var libros = await _libroRepository.GetAllAsync();
        ViewBag.LibroId = new SelectList(libros, "Id", "Titulo", libroSeleccionado);
    }
}