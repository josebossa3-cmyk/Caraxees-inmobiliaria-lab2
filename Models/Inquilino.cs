using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inmobiliaria.Models
{
  [Table("inquilinos")]
  public class Inquilino
  {
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El DNI es obligatorio")]
    [StringLength(20)]
    public string DNI { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre completo es obligatorio")]
    [StringLength(150)]
    public string NombreCompleto { get; set; } = string.Empty;

    [StringLength(30)]
    public string? Telefono { get; set; }

    [EmailAddress(ErrorMessage = "Formato de email inválido")]
    [StringLength(150)]
    public string? Email { get; set; }

    [StringLength(250)]
    public string? Direccion { get; set; }

    [Display(Name = "Fecha de alta")]
    public DateTime FechaAlta { get; set; } = DateTime.Now;

    public ICollection<Reserva>? Reservas { get; set; }
  }
}