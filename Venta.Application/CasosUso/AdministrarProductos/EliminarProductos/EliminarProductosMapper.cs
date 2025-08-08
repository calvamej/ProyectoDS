using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venta.Application.CasosUso.AdministrarProductos.ActualizarProductos;
using Venta.Domain.Models;

namespace Venta.Application.CasosUso.AdministrarProductos.EliminarProductos
{
    public class EliminarProductosMapper : Profile
    {
        public EliminarProductosMapper()
        {
            CreateMap<EliminarProductosRequest, Producto>();
        }
    }
}
