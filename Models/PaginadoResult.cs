using System.Collections.Generic;

namespace inmobiliaria.Models
{
  public class PaginadoResult<T>
  {
    public List<T> Items { get; set; } = new();
    public int PaginaActual { get; set; }
    public int TotalPaginas { get; set; }
    public int TotalRegistros { get; set; }
    public string? Busqueda { get; set; }
  }
}