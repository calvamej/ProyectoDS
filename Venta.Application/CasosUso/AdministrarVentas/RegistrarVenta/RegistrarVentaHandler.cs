using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Venta.API.Models;
using Venta.Application.CasosUso.AdministrarEntregas.RegistrarEntregas;
using Venta.Application.Common;
using Venta.Domain.Models;
using Venta.Domain.Repositories;
using Venta.Domain.Service.Events;
using Venta.Domain.Services.WebServices;
using static Venta.Application.CasosUso.AdministrarEntregas.RegistrarEntregas.RegistrarEntregasRequest;
using Models = Venta.Domain.Models;

namespace Venta.Application.CasosUso.AdministrarVentas.RegistrarVenta
{
    public class RegistrarVentaHandler :
        IRequestHandler<RegistrarVentaRequest, IResult>
    {
        private readonly IVentaRepository _ventaRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IMapper _mapper;
        private readonly IStocksService _stocksService;
        private readonly IPagoService _pagoService;
        private readonly ILogger _logger;
        private readonly IEventSender _eventSender;

        public RegistrarVentaHandler(IVentaRepository ventaRepository, IProductoRepository productoRepository, IMapper mapper,
            IStocksService stocksService, ILogger<RegistrarVentaHandler> logger, IPagoService pagoService, IEventSender eventSender, IClienteRepository clienteRepository)
        {
            _ventaRepository = ventaRepository;
            _productoRepository = productoRepository;
            _mapper = mapper;
            _stocksService = stocksService;
            _pagoService = pagoService;
            _logger = logger;
            _eventSender = eventSender;
            _clienteRepository = clienteRepository;
        }

        public async Task<IResult> Handle(RegistrarVentaRequest request, CancellationToken cancellationToken)
        {
            IResult response = null;
            try
            {
                var venta = _mapper.Map<Models.Venta>(request);
                _logger.LogInformation($"Cantidad de productos: {venta.Detalle.Count()}");

                var productsWithoutEncryption = await _ventaRepository.Get();

                //Valida existencia de producto y stock
                foreach (var detalle in venta.Detalle)
                {
                    var productoEncontrado = await _productoRepository.Consultar(detalle.IdProducto);
                    if (productoEncontrado == null || productoEncontrado?.IdProducto <= 0)
                    {
                        throw new Exception($"Producto no encontrado, código {detalle.IdProducto}");
                    }
                    if (productoEncontrado.Stock < detalle.Cantidad)
                    {
                        throw new Exception($"Producto sin stock, código {detalle.IdProducto}");
                    }
                    detalle.Precio = productoEncontrado.PrecioUnitario;
                }
                //Actualiza stocks en locales

                foreach (var detalle in venta.Detalle)
                {
                    bool ok = await _stocksService.ActualizarStock(detalle.IdProducto, detalle.Cantidad) == true ?
                        (await _productoRepository.ModificarStock(detalle.IdProducto, detalle.Cantidad) == true) ?
                        true : throw new Exception($"Error actualizando stock (SQL), código {detalle.IdProducto}") :
                        throw new Exception($"Error actualizando stock (Mongo DB), código {detalle.IdProducto}");
                }
                //Registra venta y 
                response = await _ventaRepository.Registrar(venta) == true ?
                    (await _pagoService.RealizarPago(_mapper.Map<Pago>(request.Pago), venta.IdVenta, venta.Monto, cancellationToken) == true ?
                    new SuccessResult() : new FailureResult()) : new FailureResult();


                if (response.HasSucceeded)
                {
                    RegistrarEntregasRequest entregasRequest = new();
                    entregasRequest.IdVenta = venta.IdVenta;
                    entregasRequest.Fecha = venta.Fecha;
                    var cliente = await _clienteRepository.Consultar(venta.IdCliente);
                    if (cliente != null)
                    {
                        entregasRequest.Nombre = cliente.Nombre + " " + cliente.Apellidos;
                        RegistrarDestinoRequest destinoRequest = new();
                        destinoRequest.Ciudad = cliente.Ciudad;
                        destinoRequest.DireccionEntrega = cliente.DireccionEntrega;
                        entregasRequest.Destino = destinoRequest;
                    }
                    List<RegistrarEntregasRequest.RegistrarVentaDetalleRequest> listDetalleRequest = new();

                    foreach (VentaDetalle detalle in venta.Detalle)
                    {
                        RegistrarEntregasRequest.RegistrarVentaDetalleRequest detalleRequest = new();
                        var producto = await _productoRepository.Consultar(detalle.IdProducto);
                        if (producto != null)
                        {
                            detalleRequest.Producto = producto.Nombre;
                        }
                        detalleRequest.Cantidad = detalle.Cantidad;
                        listDetalleRequest.Add(detalleRequest);
                    }
                    entregasRequest.Productos = listDetalleRequest.AsEnumerable();

                    //Publicar la información en la cola de Kafka
                    //var ventaSerialize = JsonConvert.SerializeObject(entregasRequest, Formatting.Indented,
                    //new JsonSerializerSettings()
                    //{
                    //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    //}
                    //);
                    //await _eventSender.PublishAsync("venta", ventaSerialize, cancellationToken);
                    return new SuccessResult();
                }
                else
                    return new FailureResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response = new FailureResult();
                return response;
            }
        }
    }
}
