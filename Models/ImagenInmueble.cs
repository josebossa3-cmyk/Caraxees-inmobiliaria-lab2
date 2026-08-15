using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inmobiliaria.Models
{
  [Table("imagenesinmueble")]
  public class ImagenInmueble
  {
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El inmueble es obligatorio")]
    [Display(Name = "Inmueble")]
    public int InmuebleId { get; set; }

    [Required(ErrorMessage = "La URL es obligatoria")]
    [StringLength(500)]
    public string Url { get; set; } = string.Empty;

    [Display(Name = "Es portada")]
    public bool EsPortada { get; set; } = false;

    [Display(Name = "Orden")]
    public int Orden { get; set; } = 0;

    [ForeignKey("InmuebleId")]
    public virtual Inmueble? Inmueble { get; set; }
  }
}