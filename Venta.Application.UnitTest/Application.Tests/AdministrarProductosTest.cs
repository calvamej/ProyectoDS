using AutoMapper;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Venta.Application.CasosUso.AdministrarProductos.ConsultarProductos;
using Venta.Domain.Repositories;

namespace Venta.UnitTest.Application.Tests
{
    public class AdministrarProductosTest
    {
        private readonly IProductoRepository _productoRepository;
        private readonly IMapper _mapper;
        private readonly ConsultarProductosHandler _consultarProductosHandler;

        public AdministrarProductosTest()
        {
            _productoRepository = Substitute.For<IProductoRepository>();
            _mapper = new MapperConfiguration(c=>c.AddProfile<ConsultarProductosMapper>()).CreateMapper();
            _consultarProductosHandler = Substitute.For<ConsultarProductosHandler>(_productoRepository, _mapper);
        }

        [Fact]
        public async Task ConsultarProductos()
        {

            /// Assert.True(lista.count>0);
        }
    }
}
