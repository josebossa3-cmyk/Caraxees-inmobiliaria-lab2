using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace inmobiliaria.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "El password es obligatorio")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string PasswordHash { get; set; } = "";

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; } = "";

        [Display(Name = "Avatar")]
        public string? Avatar { get; set; }


        [NotMapped]
        public IFormFile? AvatarFile {get; set;}

        [Display(Name = "Rol")]
        public string Rol { get; set; } = "";

        [Display(Name = "Fecha de creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

    }
}