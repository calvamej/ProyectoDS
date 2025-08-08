using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venta.Application.CasosUso.AdministrarProductos.RegistrarProductos;
using Venta.Domain.Models;

namespace Venta.Application.CasosUso.AdministrarProductos.ActualizarStockProductos
{
    public class ActualizarStockProductosMapper : Profile
    {
        public ActualizarStockProductosMapper()
        {
            CreateMap<ActualizarStockProductosRequest, Producto>();
        }
    }
}
