using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace inmobiliaria.Models
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email {get;set;} = "";

        [Required(ErrorMessage = "El password es obligatorio")]
        [DataType(DataType.Password)]
        public string Password {get; set;} = "";
    }
}