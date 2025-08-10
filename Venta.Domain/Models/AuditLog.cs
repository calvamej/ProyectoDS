namespace Venta.API.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string UserEmail => "TO DO:  Obtener user desde los Claims del JWT";
        public required string EntityName { get; set; }
        public required string Action { get; set; }
        public DateTime Timestamp { get; set; }
        public required string Changes { get; set; }
    }
}
