using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TarijaReadApp.Interfaces;

namespace TarijaReadApp.Controllers;

[ApiController]
[Route("api/Multas")]
[Authorize]
public class MultasApiController : ControllerBase
{
    private readonly IMultaRepository _repository;

    public MultasApiController(IMultaRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetMultas()
    {
        var multas = await _repository.GetAllAsync();

        var resultado = multas.Select(m => new
        {
            id = m.Id,
            monto = m.Monto,
            pagoRealizado = m.PagoRealizado,
            prestamoId = m.PrestamoId,
            prestamo = m.Prestamo == null ? null : new
            {
                id = m.Prestamo.Id,
                socio = m.Prestamo.Socio == null ? null : new
                {
                    nombre = m.Prestamo.Socio.Nombre
                }
            }
        });

        return Ok(resultado);
    }
}