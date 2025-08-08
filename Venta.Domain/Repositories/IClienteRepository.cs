using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venta.Domain.Models;

namespace Venta.Domain.Repositories
{
    public interface IClienteRepository : IRepository
    {
        Task<Cliente> Consultar(int id);
    }
}
