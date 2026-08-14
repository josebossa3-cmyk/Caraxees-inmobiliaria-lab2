using System;
using System.ComponentModel.DataAnnotations;

public class Pagos
{
    public int Id { get; set;}
    public int ReservaId { get; set;}

    public string Concepto { get; set;} = "";

    public DateTime FechaPago { get; set; }

    public decimal Importe { get; set; }

    public Boolean Estado { get; set; }

    public int UsuarioCreadorId { get; set;}

    public int UsuarioAnuladorId { get; set; }

    public DateTime FechaAnulacion { get; set;}


}