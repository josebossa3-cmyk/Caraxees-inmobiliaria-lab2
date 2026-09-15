using System.ComponentModel.DataAnnotations;

namespace inmobiliaria.Models
{
    public class CambiarPasswordDTO
    {
        [Required(ErrorMessage = "Ingresá tu contraseña actual")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña actual")]
        public string PasswordActual { get; set; } = "";

        [Required(ErrorMessage = "Ingresá la nueva contraseña")]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string PasswordNueva { get; set; } = "";

        [Required(ErrorMessage = "Confirmá la nueva contraseña")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar nueva contraseña")]
        [Compare(nameof(PasswordNueva), ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarPasswordNueva { get; set; } = "";
    }
}