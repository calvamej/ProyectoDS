using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venta.Application.CasosUso.AdministrarProductos.ConsultarProductos;
using Venta.Application.Common;
using Venta.Domain.Models;
using Venta.Domain.Repositories;

namespace Venta.Application.CasosUso.AdministrarProductos.RegistrarProductos
{
    public class RegistrarProductoHandler :
            IRequestHandler<RegistrarProductosRequest, IResult>
    {
        private readonly IProductoRepository _productoRepository;
        private readonly IMapper _mapper;

        public RegistrarProductoHandler(IProductoRepository productoRepository, IMapper mapper)
        {
            _productoRepository = productoRepository;
            _mapper = mapper;
        }


        public async Task<IResult> Handle(RegistrarProductosRequest request, CancellationToken cancellationToken)
        {
            IResult response = null;

            try
            {

                response = (await _productoRepository.Adicionar(_mapper.Map<Producto>(request)) == true) ? new SuccessResult() : new FailureResult();

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
