using Newtonsoft.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading;
using Venta.Domain.Models;
using Venta.Domain.Service.Events;
using Venta.Domain.Services.WebServices;

namespace Venta.Infrastructure.Services.WebServices
{
    public class PagoService : IPagoService
    {
        private readonly HttpClient _httpClientPago;
        private readonly IEventSender _eventSender;
        public PagoService(HttpClient httpClientPago, IEventSender eventSender)
        {
            _httpClientPago = httpClientPago;
            _eventSender = eventSender;
        }

        public async Task<bool> RealizarPago(Pago pago, int idVenta, decimal monto, CancellationToken cancellationToken)
        {
            if (pago == null) { return false; }

            using var request = new HttpRequestMessage(HttpMethod.Post, "api/pago/realizarPago");

            var entidadSerializada = System.Text.Json.JsonSerializer.Serialize(new { IdVenta = idVenta, Monto = monto, FormaPago = pago.FormaPago, 
                NumeroTarjeta = pago.NumeroTarjeta, FechaVencimiento = pago.FechaVencimiento, CVV = pago.CVV,
                NombreTitular = pago.NombreTitular, NumeroCuotas = pago.NumeroCuotas
            });
            request.Content = new StringContent(entidadSerializada, Encoding.UTF8, MediaTypeNames.Application.Json);

            var response = await _httpClientPago.SendAsync(request);

            if(response.IsSuccessStatusCode)
            {
                var JsonString = await response.Content.ReadAsStringAsync();
                if(JsonString.ToUpper().Contains("TRUE"))
                {
                    return true;
                }
                else { return false; }
            }
            else {
                var ventaSerialize = JsonConvert.SerializeObject(new
                {
                    IdVenta = idVenta,
                    Monto = monto,
                    FormaPago = pago.FormaPago,
                    NumeroTarjeta = pago.NumeroTarjeta,
                    FechaVencimiento = pago.FechaVencimiento,
                    CVV = pago.CVV,
                    NombreTitular = pago.NombreTitular,
                    NumeroCuotas = pago.NumeroCuotas
                }, Formatting.Indented,
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    }
                    );
                await _eventSender.PublishAsync("pago", ventaSerialize, cancellationToken);
                return false; }
        }
    }
}
