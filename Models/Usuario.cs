using System;
using System.ComponentModel.DataAnnotations;

namespace inmobiliaria.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [Display(Name = "Correo Electronico")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "El password es obligatorio")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string PasswordHash { get; set; } = "";

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50)]
        public string Nombre { get; set; } = "";

        [Required(ErrorMessage = "El apellidos es obligatorio")]
        public string Apellido { get; set; } = "";

        public string? Avatar { get; set; }

        public string Rol { get; set; } = "";

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

    }
}