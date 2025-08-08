using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venta.Application.CasosUso.AdministrarProductos.ActualizarProductos;
using Venta.Application.Common;
using Venta.Domain.Models;
using Venta.Domain.Repositories;

namespace Venta.Application.CasosUso.AdministrarProductos.EliminarProductos
{
    public class EliminarProductosHandler :
            IRequestHandler<EliminarProductosRequest, IResult>
    {
        private readonly IProductoRepository _productoRepository;
        private readonly IMapper _mapper;

        public EliminarProductosHandler(IProductoRepository productoRepository, IMapper mapper)
        {
            _productoRepository = productoRepository;
            _mapper = mapper;
        }


        public async Task<IResult> Handle(EliminarProductosRequest request, CancellationToken cancellationToken)
        {
            IResult response = null;

            try
            {

                response = (await _productoRepository.Eliminar(_mapper.Map<Producto>(request)) == true) ? new SuccessResult() : new FailureResult();
                return response;
            }
            catch (Exception ex)
            {
                response = new FailureResult();
                return response;
            }
        }
    }
}
