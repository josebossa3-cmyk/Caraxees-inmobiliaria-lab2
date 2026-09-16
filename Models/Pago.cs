using System;

namespace inmobiliaria.Models
{
    public class Pago
    {
        public int Id { get; set; }
        public int ReservaId { get; set; }

        public string Concepto { get; set; } = "";

        public DateTime FechaPago { get; set; }

        public decimal Importe { get; set; }

        public string Estado { get; set; } = "Activo";

        // para la auditoria
        public int? UsuarioCreadorId { get; set; }
        public int? UsuarioAnuladorId { get; set; }
        public DateTime? FechaAnulacion { get; set; }
    }
}