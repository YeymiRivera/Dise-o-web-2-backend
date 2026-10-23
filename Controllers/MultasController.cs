using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TarijaReadApp.Interfaces;
using TarijaReadApp.Models;
using TarijaReadApp.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace TarijaReadApp.Controllers;

[Authorize]

public class MultasController : Controller
{
    private readonly IMultaRepository _repository;
    private readonly IPrestamoRepository _prestamoRepository;

    public MultasController(IMultaRepository repository, IPrestamoRepository prestamoRepository)
    {
        _repository = repository;
        _prestamoRepository = prestamoRepository;
    }

    public async Task<IActionResult> Index() => View(await _repository.GetAllAsync());

    [Authorize(Roles = "Admin")]
   public async Task<IActionResult> Create()
{
    await CargarPrestamos();
    return View();
}

// POST: Multas/Create
[HttpPost]
[ValidateAntiForgeryToken]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> Create(MultaCreateViewModel vm)
{
    if (!ModelState.IsValid)
    {
        await CargarPrestamos(vm.PrestamoId);
        return View(vm);
    }

    // Mapeo manual: del ViewModel (lo que el usuario llenó) a la Entidad real
    var nuevaMulta = new Multa
    {
        PrestamoId = vm.PrestamoId,
        Monto = vm.Monto,
        PagoRealizado = vm.PagoRealizado
    };

    await _repository.AddAsync(nuevaMulta);
    await _repository.SaveChangesAsync();
    return RedirectToAction(nameof(Index));
}

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var multa = await _repository.GetByIdAsync(id);
        if (multa == null) return NotFound();
        await CargarPrestamos(multa.PrestamoId);
        return View(multa);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, Multa multa)
    {
        if (id != multa.Id) return NotFound();
        if (!ModelState.IsValid)
        {
            await CargarPrestamos(multa.PrestamoId);
            return View(multa);
        }

        _repository.Update(multa);
        await _repository.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var multa = await _repository.GetByIdAsync(id);
        if (multa == null) return NotFound();
        return View(multa);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var multa = await _repository.GetByIdAsync(id);
        if (multa != null)
        {
            _repository.Remove(multa);
            await _repository.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarPrestamos(int? prestamoSel = null)
    {
        var prestamos = await _prestamoRepository.GetAllAsync();
        ViewBag.PrestamoId = new SelectList(prestamos, "Id", "Descripcion", prestamoSel);
    }
}