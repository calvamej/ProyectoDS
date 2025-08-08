using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venta.Application.CasosUso.AdministrarProductos.RegistrarProductos;
using Venta.Application.Common;
using Venta.Domain.Models;
using Venta.Domain.Repositories;

namespace Venta.Application.CasosUso.AdministrarProductos.ActualizarStockProductos
{
    public class ActualizarStockProductosHandler :
            IRequestHandler<ActualizarStockProductosRequest, IResult>
    {
        private readonly IProductoRepository _productoRepository;
        private readonly IMapper _mapper;

        public ActualizarStockProductosHandler(IProductoRepository productoRepository, IMapper mapper)
        {
            _productoRepository = productoRepository;
            _mapper = mapper;
        }


        public async Task<IResult> Handle(ActualizarStockProductosRequest request, CancellationToken cancellationToken)
        {
            IResult response = null;

            try
            {

                response = (await _productoRepository.ModificarStock(request.IdProducto, request.Stock) == true) ? new SuccessResult() : new FailureResult();
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
