using MySqlConnector;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace inmobiliaria.Models
{
  public class PropietarioRepository
  {
    private readonly Database _database;

    public PropietarioRepository(Database database)
    {
      _database = database;
    }

    public async Task<List<Propietario>> ObtenerTodosAsync()
    {
      var propietarios = new List<Propietario>();
      using var connection = new MySqlConnection(_database.ConnectionString);
      await connection.OpenAsync();

      var query = "SELECT Id, DNI, NombreCompleto, Telefono, Email, Direccion, FechaAlta FROM propietarios ORDER BY NombreCompleto";
      using var command = new MySqlCommand(query, connection);
      using var reader = await command.ExecuteReaderAsync();

      while (await reader.ReadAsync())
      {
        propietarios.Add(new Propietario
        {
          Id = reader.GetInt32("Id"),
          DNI = reader.GetString("DNI"),
          NombreCompleto = reader.GetString("NombreCompleto"),
          Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString("Telefono"),
          Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString("Email"),
          Direccion = reader.IsDBNull(reader.GetOrdinal("Direccion")) ? null : reader.GetString("Direccion"),
          FechaAlta = reader.GetDateTime("FechaAlta")
        });
      }
      return propietarios;
    }

    public async Task<Propietario?> ObtenerPorIdAsync(int id)
    {
      using var connection = new MySqlConnection(_database.ConnectionString);
      await connection.OpenAsync();

      var query = "SELECT Id, DNI, NombreCompleto, Telefono, Email, Direccion, FechaAlta FROM propietarios WHERE Id = @Id";
      using var command = new MySqlCommand(query, connection);
      command.Parameters.AddWithValue("@Id", id);
      using var reader = await command.ExecuteReaderAsync();

      if (await reader.ReadAsync())
      {
        return new Propietario
        {
          Id = reader.GetInt32("Id"),
          DNI = reader.GetString("DNI"),
          NombreCompleto = reader.GetString("NombreCompleto"),
          Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString("Telefono"),
          Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString("Email"),
          Direccion = reader.IsDBNull(reader.GetOrdinal("Direccion")) ? null : reader.GetString("Direccion"),
          FechaAlta = reader.GetDateTime("FechaAlta")
        };
      }
      return null;
    }

    public async Task CrearAsync(Propietario propietario)
    {
      using var connection = new MySqlConnection(_database.ConnectionString);
      await connection.OpenAsync();

      var query = @"INSERT INTO propietarios (DNI, NombreCompleto, Telefono, Email, Direccion, FechaAlta) VALUES (@DNI, @NombreCompleto, @Telefono, @Email, @Direccion, @FechaAlta)";
      using var command = new MySqlCommand(query, connection);
      command.Parameters.AddWithValue("@DNI", propietario.DNI);
      command.Parameters.AddWithValue("@NombreCompleto", propietario.NombreCompleto);
      command.Parameters.AddWithValue("@Telefono", (object?)propietario.Telefono ?? DBNull.Value);
      command.Parameters.AddWithValue("@Email", (object?)propietario.Email ?? DBNull.Value);
      command.Parameters.AddWithValue("@Direccion", (object?)propietario.Direccion ?? DBNull.Value);
      command.Parameters.AddWithValue("@FechaAlta", propietario.FechaAlta);

      await command.ExecuteNonQueryAsync();
    }

    public async Task ActualizarAsync(Propietario propietario)
    {
      using var connection = new MySqlConnection(_database.ConnectionString);
      await connection.OpenAsync();

      var query = @"UPDATE propietarios
                      SET DNI = @DNI,
                          NombreCompleto = @NombreCompleto,
                          Telefono = @Telefono,
                          Email = @Email,
                          Direccion = @Direccion
                      WHERE Id = @Id";
      using var command = new MySqlCommand(query, connection);
      command.Parameters.AddWithValue("@DNI", propietario.DNI);
      command.Parameters.AddWithValue("@NombreCompleto", propietario.NombreCompleto);
      command.Parameters.AddWithValue("@Telefono", (object?)propietario.Telefono ?? DBNull.Value);
      command.Parameters.AddWithValue("@Email", (object?)propietario.Email ?? DBNull.Value);
      command.Parameters.AddWithValue("@Direccion", (object?)propietario.Direccion ?? DBNull.Value);

      command.Parameters.AddWithValue("@Id", propietario.Id);

      await command.ExecuteNonQueryAsync();
    }

    public async Task EliminarAsync(int id)
    {
      using var connection = new MySqlConnection(_database.ConnectionString);
      await connection.OpenAsync();

      var query = "DELETE FROM propietarios WHERE Id = @Id";
      using var command = new MySqlCommand(query, connection);
      command.Parameters.AddWithValue("@Id", id);

      await command.ExecuteNonQueryAsync();
    }
  }
}