using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inmobiliaria.Models;
{
  [Table("inmuebles")]
public class Inmueble
{
  [Key]
  public int Id { get; set; }

  [Required(ErrorMessage = "El propietario es obligatorio")]
  [Display(Name = "Propietario")]
  public int PropietarioId { get; set; }

  [Required(ErrorMessage = "El tipo de inmueble es obligatorio")]
  [Display(Name = "Tipo de inmueble")]
  public int TipoInmuebleId { get; set; }

  [Required(ErrorMessage = "La dirección es obligatoria")]
  [StringLength(250)]
  public string Direccion { get; set; }

  [Required(ErrorMessage = "El cupo es obligatorio")]
  [Range(1, int.MaxValue, ErrorMessage = "El cupo debe ser mayor a 0")]
  public int Cupo { get; set; }

  [Required(ErrorMessage = "El precio por día es obligatorio")]
  [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0")]
  [Column(TypeName = "decimal(18,2)")]
  public decimal PrecioPorDia { get; set; }

  [Required(ErrorMessage = "El porcentaje de reserva es obligatorio")]
  [Range(0, 100, ErrorMessage = "El porcentaje debe estar en entre 0 y 100")]
  [Column(TypeName = "decimal(5,2)")]
  public decimal PorcentajeReserva { get; set; }

  [Display(Name = "Disponible")]
  public bool Estado { get; set; } = true;

  [StringLength(100)]
  public string? Coordenadas { get; set; }

  [StringLength(500)]
  public string? ImagenPortada { get; set; }

  [Display(Name = "Fecha de alta")]
  public DateTime FechaAlta { get; set; } = DateTime.Now;

  [ForeignKey("PropietarioId")]
  public virtual Propietario? Propietario { get; set; }

  [ForeignKey("TipoInmuebleId")]
  public virtual TipoInmueble? TipoInmueble { get; set; }

  public virtual ICollection<ImagenInmueble>? ImagenInmuebles { get; set;}

}
}