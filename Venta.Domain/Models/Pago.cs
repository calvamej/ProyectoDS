namespace Venta.Domain.Models
{
    public class Pago
    {
        public int IdPago { get; set; }
        public int IdVenta { get; set; }
        public DateTime Fecha
        {
            get
            {
                return DateTime.Now;
            }
            private set { }
        }
        public decimal Monto { get; set; }
        public int FormaPago { get; set; }
        public string? NumeroTarjeta { get; set; }
        public DateTime? FechaVencimiento { get; set; }

        public string? CVV { get; set; }
        public string? NombreTitular { get; set; }
        public int? NumeroCuotas { get; set; }
        public int Procesado
        {
            get
            {
                return 0;
            }
            private set { }
        }
        public virtual Venta Venta { get; set; }
    }
}
