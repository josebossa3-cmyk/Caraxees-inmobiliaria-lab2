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

    [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
    [Display(Name = "Fecha de inicio")]
    [Column(TypeName = "date")]
    public DateTime FechaInicio { get; set; }

    [Required(ErrorMessage = "La fecha de fin es obligatoria")]
    [Display(Name = "Fecha de fin")]
    [Column(TypeName = "date")]
    public DateTime FechaFin { get; set; }

    [Required(ErrorMessage = "La fecha de fin original es obligatoria")]
    [Display(Name = "Fecha de fin original")]
    [Column(TypeName = "date")]
    public DateTime FechaFinOriginal { get; set; }

    [Required(ErrorMessage = "El monto por día es obligatorio")]
    [Column(TypeName = "decimal(10,2)")]
    public decimal MontoPorDia { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    [Display(Name = "Porcentaje de reserva")]
    public decimal PorcentajeReserva { get; set; }

    [Required]
    [StringLength(30)]
    public string Estado { get; set; } = "Vigente";

    [Display(Name = "Fecha de terminación")]
    public DateTime? FechaTerminacion { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal? Multa { get; set; }

    [Display(Name = "Reserva renovada de")]
    public int? ReservaRenovadaDeId { get; set; }
    public Reserva? ReservaRenovada { get; set; }

    [Required(ErrorMessage = "El usuario creador es obligatorio")]
    [Display(Name = "Usuario creador")]
    public int UsuarioCreadorId { get; set; }
    public Usuario? UsuarioCreador { get; set; }

    [Display(Name = "Usuario terminador")]
    public int? UsuarioTerminadorId { get; set; }
    public Usuario? UsuarioTerminador { get; set; }

    [Display(Name = "Fecha de creacion")]
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
  }
}