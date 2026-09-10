using System;
using MySqlConnector;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace inmobiliaria.Models
{
    public class InquilinoRepository
    {
        private readonly Database _database;

        public InquilinoRepository(Database database)
        {
            _database = database;
        }

        public async Task<PaginadoResult<Inquilino>> ObtenerPaginadosAsync(string? busqueda, int pagina, int tamañoPagina)
        {
            var resultado = new PaginadoResult<Inquilino>
            {
                PaginaActual = pagina,
                Busqueda = busqueda
            };

            using var connection = new MySqlConnection(_database.ConnectionString);
            await connection.OpenAsync();

            var where = "";
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                where = @"WHERE DNI LIKE @Busqueda
                            OR NombreCompleto LIKE @Busqueda
                            OR Email LIKE @Busqueda";
            }

            var countQuery = $"SELECT COUNT(*) FROM inquilinos {where}";
            using (var countCmd = new MySqlCommand(countQuery, connection))
            {
                if (!string.IsNullOrWhiteSpace(busqueda))
                    countCmd.Parameters.AddWithValue("@Busqueda", $"%{busqueda}%");

                resultado.TotalRegistros = Convert.ToInt32(await countCmd.ExecuteScalarAsync());
            }

            resultado.TotalPaginas = (int)Math.Ceiling((double)resultado.TotalRegistros / tamañoPagina);

            if (pagina < 1) pagina = 1;
            if (resultado.TotalPaginas > 0 && pagina > resultado.TotalPaginas)
                pagina = resultado.TotalPaginas;

            resultado.PaginaActual = pagina;

            var query = $@"SELECT Id, DNI, NombreCompleto, Telefono, Email, Direccion, FechaAlta
                        FROM inquilinos
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
                resultado.Items.Add(new Inquilino
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
        public async Task<List<Inquilino>> ObtenerTodosAsync()
        {
            List<Inquilino> inquilinos = new List<Inquilino>();

            using var connection = new MySqlConnection(_database.ConnectionString);
            await connection.OpenAsync();
            var query = @"SELECT Id, DNI, NombreCompleto, Telefono, Email, Direccion, FechaAlta FROM inquilinos ORDER BY NombreCompleto";

            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                inquilinos.Add(new Inquilino
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

            return inquilinos;

        }

        public async Task<Inquilino?> ObtenerPorIdAsync(int id)
        {
            using var connection = new MySqlConnection(_database.ConnectionString);
            await connection.OpenAsync();

            var query = @"SELECT Id, DNI, NombreCompleto, Telefono, Email, Direccion, FechaAlta FROM inquilinos WHERE Id = @Id";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Inquilino
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

        public async Task CrearAsync(Inquilino inquilino)
        {
            using var connection = new MySqlConnection(_database.ConnectionString);
            await connection.OpenAsync();
            var query = @"INSERT INTO inquilinos (DNI, NombreCompleto,Telefono,Email,Direccion,FechaAlta) VALUES(@DNI, @NombreCompleto,@Telefono,@Email,@Direccion,@FechaAlta) ";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@DNI", inquilino.DNI);
            command.Parameters.AddWithValue("@NombreCompleto", inquilino.NombreCompleto);
            command.Parameters.AddWithValue("@Telefono", (object?)inquilino.Telefono ?? DBNull.Value);
            command.Parameters.AddWithValue("@Email", (object?)inquilino.Email ?? DBNull.Value);
            command.Parameters.AddWithValue("@Direccion", (object?)inquilino.Direccion ?? DBNull.Value);
            command.Parameters.AddWithValue("@FechaAlta", inquilino.FechaAlta);
            await command.ExecuteNonQueryAsync();
        }

        public async Task ActualizarAsync(Inquilino inquilino)
        {
            using var connection = new MySqlConnection(_database.ConnectionString);
            await connection.OpenAsync();
            var query = @"UPDATE inquilinos 
                            SET DNI = @DNI,
                                NombreCompleto = @NombreCompleto,
                                Telefono = @Telefono,
                                Email = @Email,
                                Direccion = @Direccion 
                            WHERE Id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@DNI", inquilino.DNI);
            command.Parameters.AddWithValue("@NombreCompleto", inquilino.NombreCompleto);
            command.Parameters.AddWithValue("@Telefono", (object?)inquilino.Telefono ?? DBNull.Value);
            command.Parameters.AddWithValue("@Email", (object?)inquilino.Email ?? DBNull.Value);
            command.Parameters.AddWithValue("@Direccion", (object?)inquilino.Direccion ?? DBNull.Value);
            command.Parameters.AddWithValue("@Id", inquilino.Id);

            await command.ExecuteNonQueryAsync();
        }

        public async Task EliminarAsync(int id)
        {
            using var connection = new MySqlConnection(_database.ConnectionString);
            await connection.OpenAsync();
            var query = @"DELETE FROM inquilinos WHERE Id = @Id";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            await command.ExecuteNonQueryAsync();
        }


    }

}