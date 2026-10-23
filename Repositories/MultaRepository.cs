// Repositories/MultaRepository.cs
using Microsoft.EntityFrameworkCore;
using TarijaReadApp.Data;
using TarijaReadApp.Interfaces;
using TarijaReadApp.Models;

namespace TarijaReadApp.Repositories;

public class MultaRepository : Repository<Multa>, IMultaRepository
{
    public MultaRepository(ApplicationDbContext context) : base(context) { }

    public override async Task<IEnumerable<Multa>> GetAllAsync() =>
        await _context.Multas
            .Include(m => m.Prestamo).ThenInclude(p => p.Socio)
            .AsNoTracking()
            .ToListAsync();

    public override async Task<Multa?> GetByIdAsync(int id) =>
        await _context.Multas
            .Include(m => m.Prestamo).ThenInclude(p => p.Socio)
            .FirstOrDefaultAsync(m => m.Id == id);
}