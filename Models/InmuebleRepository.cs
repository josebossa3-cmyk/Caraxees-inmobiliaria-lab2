using MySqlConnector;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace inmobiliaria.Models
{
    public class InmuebleRepository
    {
        private readonly Database _database;

        public InmuebleRepository(Database database)
        {
            _database = database;
        }

        public async Task<List<Inmueble>> ObtenerTodosAsync()
        {
            var lista = new List<Inmueble>();
            using var connection = new MySqlConnection(_database.ConnectionString);
            await connection.OpenAsync();

            var query = @"SELECT i.Id, i.PropietarioId, i.TipoInmuebleId, i.Direccion, i.Cupo, i.PrecioPorDia, i.PorcentajeReserva, i.Estado, i.Coordenadas, i.ImagenPortada, i.FechaAlta,
                                p.Id AS PropId, p.DNI, p.NombreCompleto, p.Telefono, p.Email, p.Direccion AS PropietarioDireccion, p.FechaAlta as PropFechaAlta
                        FROM inmuebles i
                        LEFT JOIN propietarios p ON i.PropietarioId = p.Id
                        ORDER BY i.Direccion";
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new Inmueble
                {
                    Id = reader.GetInt32("Id"),
                    PropietarioId = reader.GetInt32("PropietarioId"),
                    TipoInmuebleId = reader.GetInt32("TipoInmuebleId"),
                    Direccion = reader.GetString("Direccion"),
                    Cupo = reader.GetInt32("Cupo"),
                    PrecioPorDia = reader.GetDecimal("PrecioPorDia"),
                    PorcentajeReserva = reader.GetDecimal("PorcentajeReserva"),
                    Estado = reader.GetBoolean("Estado"),
                    Coordenadas = reader.IsDBNull(reader.GetOrdinal("Coordenadas")) ? null : reader.GetString("Coordenadas"),
                    ImagenPortada = reader.IsDBNull(reader.GetOrdinal("ImagenPortada")) ? null : reader.GetString("ImagenPortada"),
                    FechaAlta = reader.GetDateTime("FechaAlta"),
                    Propietario = new Propietario
                    {
                        Id = reader.GetInt32("PropId"),
                        DNI = reader.GetString("DNI"),
                        NombreCompleto = reader.GetString("NombreCompleto"),
                        Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString("Telefono"),
                        Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString("Email"),
                        Direccion = reader.IsDBNull(reader.GetOrdinal("PropietarioDireccion")) ? null : reader.GetString("Direccion"),
                        FechaAlta = reader.GetDateTime("PropFechaAlta")
                    }
                });
            }
            return lista;
        }

        public async Task<List<Inmueble>> ObtenerDisponiblesAsync()
        {
            var lista = new List<Inmueble>();
            using var connection = new MySqlConnection(_database.ConnectionString);
            await connection.OpenAsync();

            var query = @"SELECT i.Id, i.PropietarioId, i.TipoInmuebleId, i.Direccion, i.Cupo, i.PrecioPorDia, i.PorcentajeReserva, i.Estado, i.Coordenadas, i.ImagenPortada, i.FechaAlta
                        FROM inmuebles i
                        WHERE i.Estado = 1
                        ORDER BY i.Direccion";
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new Inmueble
                {
                    Id = reader.GetInt32("Id"),
                    PropietarioId = reader.GetInt32("PropietarioId"),
                    TipoInmuebleId = reader.GetInt32("TipoInmuebleId"),
                    Direccion = reader.GetString("Direccion"),
                    Cupo = reader.GetInt32("Cupo"),
                    PrecioPorDia = reader.GetDecimal("PrecioPorDia"),
                    PorcentajeReserva = reader.GetDecimal("PorcentajeReserva"),
                    Estado = reader.GetBoolean("Estado"),
                    Coordenadas = reader.IsDBNull(reader.GetOrdinal("Coordenadas")) ? null : reader.GetString("Coordenadas"),
                    ImagenPortada = reader.IsDBNull(reader.GetOrdinal("ImagenPortada")) ? null : reader.GetString("ImagenPortada"),
                    FechaAlta = reader.GetDateTime("FechaAlta")
                });
            }
            return lista;
        }

        public async Task<Inmueble?> ObtenerPorIdAsync(int id)
        {
            using var connection = new MySqlConnection(_database.ConnectionString);
            await connection.OpenAsync();

            var query = @"SELECT i.Id, i.PropietarioId, i.TipoInmuebleId, i.Direccion, i.Cupo, i.PrecioPorDia, i.PorcentajeReserva, i.Estado, i.Coordenadas, i.ImagenPortada, i.FechaAlta,
                                p.Id AS PropId, p.DNI, p.NombreCompleto, p.Telefono, p.Email, p.Direccion AS PropietarioDireccion, p.FechaAlta as PropFechaAlta
                        FROM inmuebles i
                        LEFT JOIN propietarios p ON i.PropietarioId = p.Id
                        WHERE i.Id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Inmueble
                {
                    Id = reader.GetInt32("Id"),
                    PropietarioId = reader.GetInt32("PropietarioId"),
                    TipoInmuebleId = reader.GetInt32("TipoInmuebleId"),
                    Direccion = reader.GetString("Direccion"),
                    Cupo = reader.GetInt32("Cupo"),
                    PrecioPorDia = reader.GetDecimal("PrecioPorDia"),
                    PorcentajeReserva = reader.GetDecimal("PorcentajeReserva"),
                    Estado = reader.GetBoolean("Estado"),
                    Coordenadas = reader.IsDBNull(reader.GetOrdinal("Coordenadas")) ? null : reader.GetString("Coordenadas"),
                    ImagenPortada = reader.IsDBNull(reader.GetOrdinal("ImagenPortada")) ? null : reader.GetString("ImagenPortada"),
                    FechaAlta = reader.GetDateTime("FechaAlta"),
                    Propietario = new Propietario
                    {
                        Id = reader.GetInt32("PropId"),
                        DNI = reader.GetString("DNI"),
                        NombreCompleto = reader.GetString("NombreCompleto"),
                        Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString("Telefono"),
                        Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString("Email"),
                        Direccion = reader.IsDBNull(reader.GetOrdinal("Direccion")) ? null : reader.GetString("PropietarioDireccion"),
                        FechaAlta = reader.GetDateTime("PropFechaAlta")
                    }
                };
            }
            return null;
        }
    }
}
