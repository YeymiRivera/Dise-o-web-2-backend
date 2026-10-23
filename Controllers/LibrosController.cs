using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TarijaReadApp.Interfaces;
using TarijaReadApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace TarijaReadApp.Controllers;

[Authorize]
public class LibrosController : Controller
{
    private readonly ILibroRepository _repository;
    private readonly IRepository<Categoria> _categoriaRepository;

    public LibrosController(ILibroRepository repository, IRepository<Categoria> categoriaRepository)
    {
        _repository = repository;
        _categoriaRepository = categoriaRepository;
    }

    public async Task<IActionResult> Index()
    {
        var libros = await _repository.GetAllAsync();
        return View(libros);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        await CargarCategorias();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Libro libro)
    {
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                Console.WriteLine(error.ErrorMessage);

            await CargarCategorias(libro.CategoriaId);
            return View(libro);
        }

        await _repository.AddAsync(libro);
        await _repository.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var libro = await _repository.GetByIdAsync(id);
        if (libro == null) return NotFound();
        await CargarCategorias(libro.CategoriaId);
        return View(libro);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, Libro libro)
    {
        if (id != libro.Id) return NotFound();
        if (!ModelState.IsValid)
        {
            await CargarCategorias(libro.CategoriaId);
            return View(libro);
        }

        _repository.Update(libro);
        await _repository.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

     [Authorize(Roles = "Admin")] 
    public async Task<IActionResult> Delete(int id)
    {
        var libro = await _repository.GetByIdAsync(id);
        if (libro == null) return NotFound();
        return View(libro);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var libro = await _repository.GetByIdAsync(id);
        if (libro != null)
        {
            _repository.Remove(libro);
            await _repository.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarCategorias(int? categoriaSeleccionada = null)
    {
        var categorias = await _categoriaRepository.GetAllAsync();
        ViewBag.CategoriaId = new SelectList(categorias, "Id", "Nombre", categoriaSeleccionada);
    }
}