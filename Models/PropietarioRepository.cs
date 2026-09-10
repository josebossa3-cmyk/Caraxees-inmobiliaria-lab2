using MySqlConnector;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace inmobiliaria.Models
{
  public class PropietarioRepository
  {
    private readonly Database _database;

    public PropietarioRepository(Database database)
    {
      _database = database;
    }

    public async Task<PaginadoResult<Propietario>> ObtenerPaginadosAsync(string? busqueda, int pagina, int tamañoPagina)
    {
      var resultado = new PaginadoResult<Propietario>
      {
        PaginaActual = pagina,
        Busqueda = busqueda
      };

      using var connection = new MySqlConnection(_database.ConnectionString);
      await connection.OpenAsync();

      //filtro
      var where = "";
      if (!string.IsNullOrWhiteSpace(busqueda))
      {
        where = @"WHERE DNI LIKE @Busqueda
                  OR NombreCompleto LIKE @Busqueda
                  OR Email LIKE @Busqueda";
      }

      // el total de registros
      var countQuery = $"SELECT COUNT(*) FROM propietarios {where}";
      using (var countCmd = new MySqlCommand(countQuery, connection))
      {
        if (!string.IsNullOrWhiteSpace(busqueda))
          countCmd.Parameters.AddWithValue("@Busqueda", $"%{busqueda}%");

        resultado.TotalRegistros = Convert.ToInt32(await countCmd.ExecuteScalarAsync());
      }
      resultado.TotalPaginas = (int)Math.Ceiling((double)resultado.TotalRegistros / tamañoPagina);

      //ajustar por tamaño
      if (pagina < 1) pagina = 1;
      if (resultado.TotalPaginas > 0 && pagina > resultado.TotalPaginas)
        pagina = resultado.TotalPaginas;

      resultado.PaginaActual = pagina;

      //datos
      var query = $@"SELECT Id, DNI, NombreCompleto, Telefono, Email, Direccion, FechaAlta
                      FROM propietarios
                      {where}
                      ORDER BY NombreCompleto
                      LIMIT @Offset, @Tamaño";

      using var command = new MySqlCommand(query, connection);
      if (!string.IsNullOrWhiteSpace(busqueda))
        command.Parameters.AddWithValue("@Busqueda", $"%{busqueda}%");
      command.Parameters.AddWithValue("@Offset", (pagina - 1) * tamañoPagina);
      command.Parameters.AddWithValue("@Tamaño", tamañoPagina);

      using var reader = await command.ExecuteReaderAsync();
      while (await reader.ReadAsync())
      {
        resultado.Items.Add(new Propietario
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

      return resultado;
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