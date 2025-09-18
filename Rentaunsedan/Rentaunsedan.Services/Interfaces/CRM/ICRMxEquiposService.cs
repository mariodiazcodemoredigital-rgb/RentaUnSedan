using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rentaunsedan.Entities.Entities.CRM;

namespace Rentaunsedan.Services.Interfaces.CRM
{
    public interface ICRMxEquiposService
    {       
        Task<List<CRMEquipo>> GetAllAsync(string? search = null, CancellationToken ct = default);
        Task<CRMEquipo?> GetAsync(int id, CancellationToken ct = default);
        Task<int> CreateAsync(CRMEquipo e, CancellationToken ct = default);
        Task UpdateAsync(CRMEquipo e, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task ToggleActivoAsync(int id, bool activo, CancellationToken ct = default);
    }
}
