using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Venta.Application.CasosUso.AdministrarProductos.ActualizarProductos;
using Venta.Application.CasosUso.AdministrarProductos.ActualizarStockProductos;
using Venta.Application.CasosUso.AdministrarProductos.ConsultarProductos;
using Venta.Application.CasosUso.AdministrarProductos.EliminarProductos;
using Venta.Application.CasosUso.AdministrarProductos.RegistrarProductos;

namespace Venta.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductosController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configInfo;
        public ProductosController(IMediator mediator, IConfiguration configInfo)
        {
            _mediator = mediator;
            _configInfo = configInfo;
        }

        [HttpGet("consultar")]
        public async Task<IActionResult> Consultar([FromQuery] ConsultarProductosRequest request)
        {
            var response = await _mediator.Send(request);

            return Ok(response);
        }
        [HttpPost("agregar")]
        public async Task<IActionResult> Adicionar([FromBody] RegistrarProductosRequest request)
        {
            var response = await _mediator.Send(request);

            return Ok(response);
        }

        [HttpPut("actualizar")]
        public async Task<IActionResult> Actualizar([FromBody] ActualizarProductosRequest request)
        {
            var response = await _mediator.Send(request);

            return Ok(response);
        }
        [HttpPut("actualizarStock")]
        public async Task<IActionResult> ActualizarStock([FromBody] ActualizarStockProductosRequest request)
        {
            var response = await _mediator.Send(request);

            return Ok(response);
        }
        [HttpDelete("eliminar")]
        public async Task<IActionResult> Eliminar([FromBody] EliminarProductosRequest request)
        {
            var response = await _mediator.Send(request);

            return Ok(response);
        }
    }
}
