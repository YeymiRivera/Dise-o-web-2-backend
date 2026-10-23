using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TarijaReadApp.Interfaces;
using TarijaReadApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace TarijaReadApp.Controllers;

[Authorize]

public class PrestamosController : Controller
{
    private readonly IPrestamoRepository _repository;
    private readonly IEjemplarRepository _ejemplarRepository;
    private readonly IRepository<Socio> _socioRepository;
    private readonly IRepository<Usuario> _usuarioRepository;

    public PrestamosController(
        IPrestamoRepository repository,
        IEjemplarRepository ejemplarRepository,
        IRepository<Socio> socioRepository,
        IRepository<Usuario> usuarioRepository)
    {
        _repository = repository;
        _ejemplarRepository = ejemplarRepository;
        _socioRepository = socioRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<IActionResult> Index() => View(await _repository.GetAllAsync());

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        await CargarListas();
        return View();
    }

    [HttpPost]
[ValidateAntiForgeryToken]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> Create(Prestamo prestamo)
{
    if (!ModelState.IsValid)
    {
        foreach (var error in ModelState)
        {
            foreach (var err in error.Value.Errors)
            {
                Console.WriteLine($"Campo: {error.Key} - Error: {err.ErrorMessage}");
            }
        }

        await CargarListas(prestamo.EjemplarId, prestamo.SocioId, prestamo.UsuarioId);
        return View(prestamo);
    }

    await _repository.AddAsync(prestamo);
    await _repository.SaveChangesAsync();
    return RedirectToAction(nameof(Index));
}

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var prestamo = await _repository.GetByIdAsync(id);
        if (prestamo == null) return NotFound();
        await CargarListas(prestamo.EjemplarId, prestamo.SocioId, prestamo.UsuarioId);
        return View(prestamo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, Prestamo prestamo)
    {
        if (id != prestamo.Id) return NotFound();
        if (!ModelState.IsValid)
        {
            await CargarListas(prestamo.EjemplarId, prestamo.SocioId, prestamo.UsuarioId);
            return View(prestamo);
        }

        _repository.Update(prestamo);
        await _repository.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var prestamo = await _repository.GetByIdAsync(id);
        if (prestamo == null) return NotFound();
        return View(prestamo);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var prestamo = await _repository.GetByIdAsync(id);
        if (prestamo != null)
        {
            _repository.Remove(prestamo);
            await _repository.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListas(int? ejemplarSel = null, int? socioSel = null, int? usuarioSel = null)
    {
        var ejemplares = await _ejemplarRepository.GetAllAsync();
        var socios = await _socioRepository.GetAllAsync();
        var usuarios = await _usuarioRepository.GetAllAsync();

        ViewBag.EjemplarId = new SelectList(ejemplares, "Id", "Codigo", ejemplarSel);
        ViewBag.SocioId = new SelectList(socios, "Id", "Nombre", socioSel);
        ViewBag.UsuarioId = new SelectList(usuarios, "Id", "Nombre", usuarioSel);
    }
}