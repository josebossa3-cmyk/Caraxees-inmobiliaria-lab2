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

            const string query = @"SELECT Id, ReservaId, Concepto, FechaPago, Importe, Estado
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
                    Estado = reader.GetString("Estado")
                });
            }

            return pagos;
        }

        public async Task CrearAsync(Pago pago)
        {
            using var connection = new MySqlConnection(_database.ConnectionString);
            await connection.OpenAsync();

            const string query = @"INSERT INTO pagos
                                (ReservaId, Concepto, FechaPago, Importe, Estado)
                                VALUES (@ReservaId, @Concepto, @FechaPago, @Importe, @Estado)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@ReservaId", pago.ReservaId);
            command.Parameters.AddWithValue("@Concepto", pago.Concepto);
            command.Parameters.AddWithValue("@FechaPago", pago.FechaPago);
            command.Parameters.AddWithValue("@Importe", pago.Importe);
            command.Parameters.AddWithValue("@Estado", pago.Estado);
            await command.ExecuteNonQueryAsync();
        }
    }
}