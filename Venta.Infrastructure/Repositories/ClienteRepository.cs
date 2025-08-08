using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venta.Domain.Models;
using Venta.Domain.Repositories;
using Venta.Infrastructure.Repositories.Base;

namespace Venta.Infrastructure.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly VentaDbContext _context;
        public ClienteRepository(VentaDbContext context)
        {
            _context = context;
        }
        public async Task<Cliente> Consultar(int id)
        {
            return await _context.Clientes.FindAsync(id);
        }
    }
}
