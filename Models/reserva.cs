using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inmobiliaria.Models
{
  [Table("reservas")]
  public class Reserva
  {
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El inquilino es obligatorio")]
    public int InquilinoId { get; set; }
    public Inquilino? Inquilino { get; set; }

    [Required(ErrorMessage = "El inmueble es obligatorio")]
    public int InmuebleId { get; set; }
    public Inmueble? Inmueble { get; set; }

    [Required(ErrorMessage = "El monto por día es obligatorio")]
    [Column(TypeName = "decimal(10,2)")]
    public decimal MontoPorDia { get; set; }

    [Required(ErrorMessage = "La fecha desde es obligatoria")]
    [Display(Name = "Fecha desde")]
    public DateTime FechaDesde { get; set; }

    [Required(ErrorMessage = "La fecha hasta es obligatoria")]
    [Display(Name = "Fecha hasta")]
    public DateTime FechaHasta { get; set; }

    [Display(Name = "Fecha fin")]
    public DateTime? FechaFin { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    [Display(Name = "Porcentaje de reserva")]
    public decimal PorcentajeReserva { get; set; }

    [Required]
    [StringLength(30)]
    public string Estado { get; set; } = "Vigente";

    [Display(Name = "Fecha de reserva")]
    public DateTime FechaReserva { get; set; } = DateTime.Now;

    [Column(TypeName = "decimal(10,2)")]
    public decimal? Multa { get; set; }

    [Display(Name = "Reserva renovada")]
    public int? ReservaRenovadaId { get; set; }
    public Reserva? ReservaRenovada { get; set; }

    [Required(ErrorMessage = "El usuario creador es obligatorio")]
    [Display(Name = "Usuario creador")]
    public int UsuarioCreadorId { get; set; }
    public Usuario? UsuarioCreador { get; set; }

    [Display(Name = "Usuario terminador")]
    public int? UsuarioTerminadorId { get; set; }
    public Usuario? UsuarioTerminador { get; set; }

    [Display(Name = "Fecha de terminación")]
    public DateTime? FechaTerminacion { get; set; }
  }
}