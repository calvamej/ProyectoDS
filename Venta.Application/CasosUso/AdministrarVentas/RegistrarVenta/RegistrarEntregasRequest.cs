using Venta.Application.Common;
using Venta.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venta.Application.CasosUso.AdministrarEntregas.RegistrarEntregas
{
    public class RegistrarEntregasRequest : IRequest<IResult>
    {
        public int IdVenta { get; set; }
        public DateTime Fecha { get; set; }
        public string Nombre { get; set; }
        public IEnumerable<RegistrarVentaDetalleRequest> Productos { get; set; }
        public RegistrarDestinoRequest Destino { get; set; }
        public class RegistrarVentaDetalleRequest
        {
            public string Producto { get; set; }

            public int Cantidad { get; set; }
        }
        public class RegistrarDestinoRequest
        {
            public string DireccionEntrega { get; set; }
            public string Ciudad { get; set; }
        }
    }
}
