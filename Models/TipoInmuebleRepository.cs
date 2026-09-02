using MySqlConnector;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace inmobiliaria.Models
{
  public class TipoInmuebleRepository
  {
    private readonly Database _database;

    public TipoInmuebleRepository(Database database)
    {
      _database = database;
    }

    public async Task<List<TipoInmueble>> ObtenerTodosAsync()
    {
      var lista = new List<TipoInmueble>();

      using var connection = new MySqlConnection(_database.ConnectionString);

      await connection.OpenAsync();

      var query = "SELECT Id, Nombre FROM tiposinmueble ORDER BY Nombre";
      using var command = new MySqlCommand(query, connection);
      using var reader = await command.ExecuteReaderAsync();

      while (await reader.ReadAsync())
      {
        lista.Add(new TipoInmueble
        {
          Id = reader.GetInt32("Id"),
          Nombre = reader.GetString("Nombre")
        });
      }
      return lista;
    }
  }
}