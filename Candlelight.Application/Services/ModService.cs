using Candlelight.Core.Entities;
using Candlelight.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Candlelight.Application.Services;

public class ModService(DataContext context)
{
    private readonly DataContext _context = context;

    public async Task<Mod?> GetModByIdAsync(Guid modId)
    {
        return await _context.Mods
            .Include(m => m.Game)
            .FirstOrDefaultAsync(m => m.Id == modId);
    }

    public async Task<List<Mod>> GetModsByUserIdAsync(Guid userId)
    {
        return await _context.Mods
            .Include(m => m.Game)
            .Where(m => m.CreatedBy == userId)
            .ToListAsync();
    }

    public async Task<Mod> AddModAsync(Mod mod)
    {
        await _context.Mods.AddAsync(mod);
        if (mod.Versions.Count > 0)
        {
            foreach (var version in mod.Versions)
            {
                await AddModVersionAsync(version);
            }
            
        }
        await _context.SaveChangesAsync();
        return mod;
    }

    public async Task<ModVersion> AddModVersionAsync(ModVersion modVersion)
    {
        await _context.ModVersions.AddAsync(modVersion);
        await _context.SaveChangesAsync();
        return modVersion;
    }

    public async Task<(List<Mod> Mods, int TotalCount)> GetModsByGameIdAsync(Guid gameId, int page, int pageSize)
    {
        var query = _context.Mods
            .Where(m => m.GameId == gameId)
            .OrderByDescending(m => m.CreatedAt);

        var totalCount = await query.CountAsync();

        var mods = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (mods, totalCount);
    }

}
