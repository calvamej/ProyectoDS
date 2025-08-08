using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venta.Domain.Models;

namespace Venta.Domain.Services.WebServices
{
    public interface IPagoService
    {
        Task<bool> RealizarPago(Pago pago, int idVenta, decimal monto, CancellationToken cancellationToken);
    }
}
