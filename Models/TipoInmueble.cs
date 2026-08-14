using System;
using System.ComponentModel.DataAnnotations;

public class TipoInmueble
{
    [Key]
    public int Id { get; set;}

[Required(ErrorMessage = "El nombre es obligatorio")]

    public string Nombre { get; set; } = "";

}