using MySqlConnector;
using System.Threading.Tasks;

namespace inmobiliaria.Models
{
  public class ImagenInmuebleRepository
  {
    private readonly Database _database;

    public ImagenInmuebleRepository(Database database)
    {
      _database = database;
    }

    public async Task<List<ImagenInmueble>> ObtenerPorInmuebleAsync(int inmuebleId)
    {
      var lista = new List<ImagenInmueble>();

      using var connection = new MySqlConnection(_database.ConnectionString);
      await connection.OpenAsync();

      var query = @"SELECT Id, InmuebleId, Url, EsPortada, Orden
                    FROM imagenesinmueble
                    WHERE InmuebleId = @InmuebleId
                    ORDER BY Orden";
      using var command = new MySqlCommand(query, connection);
      command.Parameters.AddWithValue("@InmuebleId", inmuebleId);

      using var reader = await command.ExecuteReaderAsync();
      while (await reader.ReadAsync())
      {
        lista.Add(new ImagenInmueble
        {
          Id = reader.GetInt32("Id"),
          InmuebleId = reader.GetInt32("InmuebleId"),
          Url = reader.GetString("Url"),
          EsPortada = reader.GetBoolean("EsPortada"),
          Orden = reader.GetInt32("Orden")
        });
      }
      return lista;
    }

    public async Task CrearAsync(ImagenInmueble imagen)
    {
      using var connection = new MySqlConnection(_database.ConnectionString);
      await connection.OpenAsync();

      var query = @"INSERT INTO imagenesinmueble (InmuebleId, Url, EsPortada, Orden)
                    VALUES (@InmuebleId, @Url, @EsPortada, @Orden)";

      using var command = new MySqlCommand(query, connection);
      command.Parameters.AddWithValue("@InmuebleId", imagen.InmuebleId);
      command.Parameters.AddWithValue("@Url", imagen.Url);
      command.Parameters.AddWithValue("@EsPortada", imagen.EsPortada);
      command.Parameters.AddWithValue("@Orden", imagen.Orden);

      await command.ExecuteNonQueryAsync();
    }

    public async Task EliminarPorInmuebleAsync(int inmuebleId)
    {
      using var connection = new MySqlConnection(_database.ConnectionString);

      await connection.OpenAsync();

      var query = "DELETE FROM imagenesinmueble WHERE InmuebleId = @InmuebleId";

      using var command = new MySqlCommand(query, connection);
      command.Parameters.AddWithValue("@InmuebleId", inmuebleId);

      await command.ExecuteNonQueryAsync();
    }
  }
  
  
  
}