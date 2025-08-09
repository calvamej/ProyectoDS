using Venta.Domain.Models;

namespace Venta.Domain.Repositories
{
    public interface IVentaRepository : IRepository
    {
        Task<bool> Registrar(Models.Venta venta);

        Task<IEnumerable<Pago>> Get();
    }
}
