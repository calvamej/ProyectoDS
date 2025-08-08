using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venta.Application.Common;

namespace Venta.Application.CasosUso.AdministrarVentas.RegistrarVenta
{
    public class RegistrarVentaRequest : IRequest<IResult>
    {
        public int IdCliente { get; set; }
        public IEnumerable<RegistrarVentaDetalleRequest> Productos { get; set; }
        public RegistrarPagoRequest Pago { get; set; }
    }

    public class RegistrarVentaDetalleRequest
    {
        public int IdProducto { get; set; }

        public int Cantidad { get; set; }
    }
    public class RegistrarPagoRequest
    {
        public int FormaPago { get; set; }
        public string? NumeroTarjeta { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string? CVV { get; set; }
        public string? NombreTitular { get; set; }
        public int? NumeroCuotas { get; set; }
    }
}
