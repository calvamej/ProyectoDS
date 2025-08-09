using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Venta.Domain.Models;
using Venta.Domain.Repositories;
using Venta.Infrastructure.Repositories.Base;

namespace Venta.Infrastructure.Repositories
{
    public class VentaRepository : IVentaRepository
    {
        private readonly VentaDbContext _context;

        public VentaRepository(VentaDbContext context)
        {
            _context = context;
        }
        public async Task<bool> Registrar(Domain.Models.Venta venta)
        {
            try
            {
                _context.Add(venta);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public async Task<IEnumerable<Pago>> Get()
        {
            return await _context.Database.GetDbConnection().QueryAsync<Pago>(
                "GetPagos",
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
