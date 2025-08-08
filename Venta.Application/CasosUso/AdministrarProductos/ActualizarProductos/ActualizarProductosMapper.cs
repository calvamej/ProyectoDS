using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venta.Application.CasosUso.AdministrarProductos.RegistrarProductos;
using Venta.Domain.Models;

namespace Venta.Application.CasosUso.AdministrarProductos.ActualizarProductos
{
    public class ActualizarProductosMapper : Profile
    {
        public ActualizarProductosMapper()
        {
            CreateMap<ActualizarProductosRequest, Producto>();
        }
    }
}
