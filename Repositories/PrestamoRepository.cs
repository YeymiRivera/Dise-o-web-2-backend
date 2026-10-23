// Repositories/PrestamoRepository.cs
using Microsoft.EntityFrameworkCore;
using TarijaReadApp.Data;
using TarijaReadApp.Interfaces;
using TarijaReadApp.Models;

namespace TarijaReadApp.Repositories;

public class PrestamoRepository : Repository<Prestamo>, IPrestamoRepository
{
    public PrestamoRepository(ApplicationDbContext context) : base(context) { }

    public override async Task<IEnumerable<Prestamo>> GetAllAsync() =>
        await _context.Prestamos
            .Include(p => p.Ejemplar).ThenInclude(e => e.Libro)
            .Include(p => p.Socio)
            .Include(p => p.Usuario)
            .AsNoTracking()
            .ToListAsync();

    public override async Task<Prestamo?> GetByIdAsync(int id) =>
        await _context.Prestamos
            .Include(p => p.Ejemplar).ThenInclude(e => e.Libro)
            .Include(p => p.Socio)
            .Include(p => p.Usuario)
            .FirstOrDefaultAsync(p => p.Id == id);
}