using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlConnector;

namespace inmobiliaria.Models
{
    public class PagoRepository
    {
        private readonly Database _database;

        public PagoRepository(Database database)
        {
            _database = database;
        }

        public async Task<List<Pago>> ObtenerPorReservaAsync(int? reservaId)
        {
            var pagos = new List<Pago>();
            using var connection = new MySqlConnection(_database.ConnectionString);
            await connection.OpenAsync();

            const string query = @"SELECT Id, ReservaId, Concepto, FechaPago, Importe, Estado, UsuarioCreadorId, UsuarioAnuladorId, FechaAnulacion
                                FROM pagos
                                WHERE (@ReservaId IS NULL OR ReservaId = @ReservaId)
                                ORDER BY FechaPago DESC, Id DESC";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@ReservaId", (object?)reservaId ?? DBNull.Value);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                pagos.Add(new Pago
                {
                    Id = reader.GetInt32("Id"),
                    ReservaId = reader.GetInt32("ReservaId"),
                    Concepto = reader.GetString("Concepto"),
                    FechaPago = reader.GetDateTime("FechaPago"),
                    Importe = reader.GetDecimal("Importe"),
                    Estado = reader.GetString("Estado"),
                    UsuarioCreadorId = reader.IsDBNull(reader.GetOrdinal("UsuarioCreadorId")) ? null : reader.GetInt32("UsuarioCreadorId"),
                    UsuarioAnuladorId = reader.IsDBNull(reader.GetOrdinal("UsuarioAnuladorId")) ? null : reader.GetInt32("UsuarioAnuladorId"),
                    FechaAnulacion = reader.IsDBNull(reader.GetOrdinal("FechaAnulacion")) ? null : reader.GetDateTime("FechaAnulacion")
                });
            }

            return pagos;
        }

        public async Task<Pago?> ObtenerPorIdAsync(int id)
        {
            using var connection = new MySqlConnection(_database.ConnectionString);
            await connection.OpenAsync();

            const string query = @"SELECT Id, ReservaId, Concepto, FechaPago, Importe, Estado
            FROM pagos WHERE Id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Pago
                {
                    Id = reader.GetInt32("Id"),
                    ReservaId = reader.GetInt32("ReservaId"),
                    Concepto = reader.GetString("Concepto"),
                    FechaPago = reader.GetDateTime("FechaPago"),
                    Importe = reader.GetDecimal("Importe"),
                    Estado = reader.GetString("Estado")
                };
            }
            return null;
        }
        public async Task CrearAsync(Pago pago)
        {
            using var connection = new MySqlConnection(_database.ConnectionString);
            await connection.OpenAsync();

            const string query = @"INSERT INTO pagos
                                (ReservaId, Concepto, FechaPago, Importe, Estado, UsuarioCreadorId)
                                VALUES (@ReservaId, @Concepto, @FechaPago, @Importe, @Estado, @UsuarioCreadorId)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@ReservaId", pago.ReservaId);
            command.Parameters.AddWithValue("@Concepto", pago.Concepto);
            command.Parameters.AddWithValue("@FechaPago", pago.FechaPago);
            command.Parameters.AddWithValue("@Importe", pago.Importe);
            command.Parameters.AddWithValue("@Estado", pago.Estado);
            command.Parameters.AddWithValue("@UsuarioCreadorId", (object?)pago.UsuarioCreadorId ?? DBNull.Value);
            await command.ExecuteNonQueryAsync();
        }

        public async Task ActualizarConceptoAsync(int id, string concepto)
        {
            using var connection = new MySqlConnection(_database.ConnectionString);
            await connection.OpenAsync();

            const string query = "UPDATE pagos SET Concepto = @Concepto WHERE Id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Concepto", concepto);
            command.Parameters.AddWithValue("@Id", id);
            await command.ExecuteNonQueryAsync();
        }

        public async Task AnularAsync(int id, int UsuarioAnuladorId)
        {
            using var connection = new MySqlConnection(_database.ConnectionString);
            await connection.OpenAsync();

            const string query = @"UPDATE pagos
            SET Estado = 'Anulado',
            UsuarioAnuladorId = @Usuario,
            FechaAnulacion = @Fecha
            WHERE Id = @Id";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Usuario", UsuarioAnuladorId);
            command.Parameters.AddWithValue("@Fecha", DateTime.Now);
            command.Parameters.AddWithValue("@Id", id);
            await command.ExecuteNonQueryAsync();
        }
    }
}