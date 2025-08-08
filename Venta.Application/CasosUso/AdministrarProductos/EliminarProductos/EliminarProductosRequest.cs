using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venta.Application.Common;

namespace Venta.Application.CasosUso.AdministrarProductos.EliminarProductos
{
    public class EliminarProductosRequest : IRequest<IResult>
    {
        public int IdProducto { get; set; }
    }
}
