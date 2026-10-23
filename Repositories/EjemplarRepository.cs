// Repositories/EjemplarRepository.cs
using Microsoft.EntityFrameworkCore;
using TarijaReadApp.Data;
using TarijaReadApp.Interfaces;
using TarijaReadApp.Models;

namespace TarijaReadApp.Repositories;

public class EjemplarRepository : Repository<Ejemplar>, IEjemplarRepository
{
    public EjemplarRepository(ApplicationDbContext context) : base(context) { }

    public override async Task<IEnumerable<Ejemplar>> GetAllAsync() =>
        await _context.Ejemplares
            .Include(e => e.Libro)
            .AsNoTracking()
            .ToListAsync();

    public override async Task<Ejemplar?> GetByIdAsync(int id) =>
        await _context.Ejemplares
            .Include(e => e.Libro)
            .FirstOrDefaultAsync(e => e.Id == id);
}