using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySqlConnector;

namespace inmobiliaria.Models
{
    public class UsuarioRepository
    {
        private readonly Database _database;

        public UsuarioRepository(Database database)
        {
            _database = database;
        }
        public async Task<PaginadoResult<Usuario>> ObtenerPaginadosAsync(string? busqueda, int pagina, int tamañoPagina)
        {
            var resultado = new PaginadoResult<Usuario>
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
                where = @"WHERE NombreCompleto LIKE @Busqueda
                  OR Email LIKE @Busqueda";
            }

            // el total de registros
            var countQuery = $"SELECT COUNT(*) FROM usuarios {where}";
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
            var query = $@"SELECT Id, Email, PasswordHash, NombreCompleto, Avatar, Rol, FechaCreacion
                        FROM usuarios
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
                resultado.Items.Add(new Usuario
                {
                    Id = reader.GetInt32("Id"),
                    Email = reader.GetString("Email"),
                    PasswordHash = reader.GetString("PasswordHash"),
                    NombreCompleto = reader.GetString("NombreCompleto"),
                    Avatar = reader.IsDBNull(reader.GetOrdinal("Avatar")) ? null : reader.GetString("Avatar"),
                    Rol = reader.GetString("Rol"),
                    FechaCreacion = reader.GetDateTime("FechaCreacion")
                });
            }

            return resultado;
        }
        public async Task<List<Usuario>> ObtenerTodosAsync()
        {
            var usuarios = new List<Usuario>();
            using var connection = new MySqlConnection(_database.ConnectionString);
            var query = @"SELECT Id,Email,PasswordHash,NombreCompleto,Avatar,Rol,FechaCreacion FROM usuarios";
            await connection.OpenAsync();

            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                usuarios.Add(new Usuario
                {
                    Id = reader.GetInt32("Id"),
                    Email = reader.GetString("Email"),
                    PasswordHash = reader.GetString("PasswordHash"),
                    NombreCompleto = reader.GetString("NombreCompleto"),
                    Avatar = reader.IsDBNull(reader.GetOrdinal("Avatar")) ? (string?)null : reader.GetString("Avatar"),
                    Rol = reader.GetString("Rol"),
                    FechaCreacion = reader.GetDateTime("FechaCreacion")
                });
            }
            return usuarios;
        }

        public async Task<Usuario?> ObtenerPorIdAsync(int id)
        {
            using var connection = new MySqlConnection(_database.ConnectionString);
            var query = @"SELECT Id,Email,PasswordHash,NombreCompleto,Avatar,Rol,FechaCreacion FROM usuarios WHERE Id = @Id ";
            await connection.OpenAsync();
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                return new Usuario
                {
                    Id = reader.GetInt32("Id"),
                    Email = reader.GetString("Email"),
                    PasswordHash = reader.GetString("PasswordHash"),
                    NombreCompleto = reader.GetString("NombreCompleto"),
                    Avatar = reader.IsDBNull(reader.GetOrdinal("Avatar")) ? (string?)null : reader.GetString("Avatar"),
                    Rol = reader.GetString("Rol"),
                    FechaCreacion = reader.GetDateTime("FechaCreacion")
                };
            }

            return null;
        }

        public async Task<Usuario?> ObtenerPorEmailAsync(string email)
        {
            using var connection = new MySqlConnection(_database.ConnectionString);
            await connection.OpenAsync();
            var query = @"SELECT Id,Email,PasswordHash,NombreCompleto,Avatar,Rol,FechaCreacion FROM usuarios WHERE Email = @Email";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Email", email);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                return new Usuario
                {
                    Id = reader.GetInt32("Id"),
                    Email = reader.GetString("Email"),
                    PasswordHash = reader.GetString("PasswordHash"),
                    NombreCompleto = reader.GetString("NombreCompleto"),
                    Avatar = reader.IsDBNull(reader.GetOrdinal("Avatar")) ? (string?)null : reader.GetString("Avatar"),
                    Rol = reader.GetString("Rol"),
                    FechaCreacion = reader.GetDateTime("FechaCreacion")
                };
            }
            return null;
        }

        public async Task CrearAsync(Usuario usuario)
        {
            using var connection = new MySqlConnection(_database.ConnectionString);
            await connection.OpenAsync();
            var query = @"INSERT INTO usuarios (
                        Email,
                        PasswordHash,
                        NombreCompleto,
                        Avatar,
                        Rol,
                        FechaCreacion)
                        VALUES(
                        @Email,
                        @PasswordHash,
                        @NombreCompleto,
                        @Avatar,
                        @Rol,
                        @FechaCreacion
                        )";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Email", usuario.Email);
            command.Parameters.AddWithValue("@PasswordHash", usuario.PasswordHash);
            command.Parameters.AddWithValue("@NombreCompleto", usuario.NombreCompleto);
            command.Parameters.AddWithValue("@Avatar", (object?)usuario.Avatar ?? DBNull.Value);
            command.Parameters.AddWithValue("@Rol", usuario.Rol);
            command.Parameters.AddWithValue("@FechaCreacion", usuario.FechaCreacion);
            await command.ExecuteNonQueryAsync();
        }

        public async Task ActualizarAsync(Usuario usuario)
        {
            using var connection = new MySqlConnection(_database.ConnectionString);
            await connection.OpenAsync();
            var query = @"UPDATE usuarios
                        SET Email = @Email,
                        PasswordHash = @PasswordHash,
                        NombreCompleto = @NombreCompleto,
                        Avatar = @Avatar,
                        Rol = @Rol
                        WHERE Id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Email", usuario.Email);
            command.Parameters.AddWithValue("@PasswordHash", usuario.PasswordHash);
            command.Parameters.AddWithValue("@NombreCompleto", usuario.NombreCompleto);
            command.Parameters.AddWithValue("@Avatar", (object?)usuario.Avatar ?? DBNull.Value);
            command.Parameters.AddWithValue("@Rol", usuario.Rol);
            command.Parameters.AddWithValue("@Id", usuario.Id);
            await command.ExecuteNonQueryAsync();

        }

        public async Task EliminarAsync(int id)
        {
            using var connection = new MySqlConnection(_database.ConnectionString);
            var query = @"DELETE FROM usuarios WHERE Id = @Id";
            await connection.OpenAsync();
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            await command.ExecuteNonQueryAsync();
        }

    }
}