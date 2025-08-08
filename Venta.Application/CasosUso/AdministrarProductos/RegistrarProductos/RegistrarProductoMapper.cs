using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venta.Application.CasosUso.AdministrarProductos.ConsultarProductos;
using Venta.Domain.Models;

namespace Venta.Application.CasosUso.AdministrarProductos.RegistrarProductos
{
    public class RegistrarProductoMapper : Profile
    {
        public RegistrarProductoMapper()
        {
            CreateMap<RegistrarProductosRequest, Producto> ();
        }
    }
}
