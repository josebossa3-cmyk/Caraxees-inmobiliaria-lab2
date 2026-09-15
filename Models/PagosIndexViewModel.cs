using System.Collections.Generic;

namespace inmobiliaria.Models
{
    public class PagosIndexViewModel
    {
        public List<Reserva> Reservas { get; set; } = new();
        public int? ReservaId { get; set; }
        public Reserva? ReservaSeleccionada { get; set; }
        public List<Pago> Pagos { get; set; } = new();
        public decimal TotalReserva { get; set; }
    }
}