using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySqlConnector;

namespace inmobiliaria.Models
{
    public class ReservaRepository
    {
                private readonly Database _database;

        public ReservaRepository(Database database)
        {
            _database = database;
        }

    public async Task<List<Reserva>> ObtenerTodosAsync()
        {
            var reservas = new List<Reserva>();
            using var connection = new MySqlConnection (_database.ConnectionString);
            await connection.OpenAsync();

            var query = @"SELECT Id, InquilinoId, InmuebleId, FechaInicio, FechaFin, FechaFinOriginal, MontoPorDia, PorcentajeReserva, Estado, FechaTerminacion, Multa, ReservaRenovadaDeId, UsuarioCreadorId, UsuarioTerminadorId,FechaCreacion FROM reservas";
            using var command = new MySqlCommand(query,connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                reservas.Add(new Reserva
                {
                    Id = reader.GetInt32("Id"),
                    InquilinoId = reader.GetInt32("InquilinoId"), 
                    InmuebleId = reader.GetInt32("InmuebleId"), 
                    FechaInicio = reader.GetDateTime("FechaInicio"),
                    FechaFin = reader.GetDateTime("FechaFin"),
                    FechaFinOriginal = reader.GetDateTime("FechaFinOriginal"),
                    MontoPorDia = reader.GetDecimal("MontoPorDia"),
                    PorcentajeReserva = reader.GetDecimal("PorcentajeReserva"),
                    Estado =  reader.GetString("Estado"),
                    FechaTerminacion = reader.IsDBNull(reader.GetOrdinal("FechaTerminacion")) ? null : reader.GetDateTime("FechaTerminacion"),
                    Multa = reader.IsDBNull(reader.GetOrdinal("Multa")) ? (int?)null : reader.GetDecimal("Multa"),
                    ReservaRenovadaDeId = reader.IsDBNull(reader.GetOrdinal("ReservaRenovadaDeId")) ? (int?)null : reader.GetInt32("ReservaRenovadaDeId"),
                    UsuarioCreadorId = reader.GetInt32("UsuarioCreadorId"),
                    UsuarioTerminadorId = reader.IsDBNull(reader.GetOrdinal("UsuarioTerminadorId")) ? (int?)null : reader.GetInt32("UsuarioTerminadorId"),
                    FechaCreacion = reader.GetDateTime("FechaCreacion")
                });
            }

            return reservas;
        }

        public async Task<Reserva?> ObtenerPorIdAsync(int id)
        {
            var reserva = new Reserva();
            using var connection = new MySqlConnection(_database.ConnectionString);
            await connection.OpenAsync();
            var query = @"SELECT * FROM  reservas WHERE Id = @Id";
            using var command = new MySqlCommand(query,connection);
            command.Parameters.AddWithValue("@Id",id);
            var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Reserva
                {
                    Id = reader.GetInt32("Id"),
                    InquilinoId = reader.GetInt32("InquilinoId"), 
                    InmuebleId = reader.GetInt32("InmuebleId"), 
                    FechaInicio = reader.GetDateTime("FechaInicio"),
                    FechaFin = reader.GetDateTime("FechaFin"),
                    FechaFinOriginal = reader.GetDateTime("FechaFinOriginal"),
                    MontoPorDia = reader.GetDecimal("MontoPorDia"),
                    PorcentajeReserva = reader.GetDecimal("PorcentajeReserva"),
                    Estado =  reader.GetString("Estado"),
                    FechaTerminacion = reader.IsDBNull(reader.GetOrdinal("FechaTerminacion")) ? null : reader.GetDateTime("FechaTerminacion"),
                    Multa = reader.IsDBNull(reader.GetOrdinal("Multa")) ? (int?)null : reader.GetDecimal("Multa"),
                    ReservaRenovadaDeId = reader.IsDBNull(reader.GetOrdinal("ReservaRenovadaDeId")) ? (int?)null : reader.GetInt32("ReservaRenovadaDeId"),
                    UsuarioCreadorId = reader.GetInt32("UsuarioCreadorId"),
                    UsuarioTerminadorId = reader.IsDBNull(reader.GetOrdinal("UsuarioTerminadorId")) ? (int?)null : reader.GetInt32("UsuarioTerminadorId"),
                    FechaCreacion = reader.GetDateTime("FechaCreacion")
                };
            }

            return null;
        }


        public async Task CrearAsync(Reserva reserva)
        {
            using var connection = new MySqlConnection(_database.ConnectionString);
            await connection.OpenAsync();
            var query = @"INSERT INTO reservas(
            InquilinoId,
            InmuebleId,
            FechaInicio,
            FechaFin,
            FechaFinOriginal,
            MontoPorDia,
            PorcentajeReserva,
            Estado,
            FechaTerminacion,
            Multa,
            ReservaRenovadaDeId,
            UsuarioCreadorId,
            UsuarioTerminadorId,
            FechaCreacion) 
            VALUES (
            @InquilinoId,
            @InmuebleId,
            @FechaInicio,
            @FechaFin,
            @FechaFinOriginal,
            @MontoPorDia,
            @PorcentajeReserva,
            @Estado,
            @FechaTerminacion,
            @Multa,
            @ReservaRenovadaDeId,
            @UsuarioCreadorId,
            @UsuarioTerminadorId,
            @FechaCreacion
            )";
            using var command = new MySqlCommand(query,connection);
            command.Parameters.AddWithValue("@InquilinoId",reserva.InquilinoId);
            command.Parameters.AddWithValue("@InmuebleId",reserva.InmuebleId);
            command.Parameters.AddWithValue("@FechaInicio",reserva.FechaInicio);
            command.Parameters.AddWithValue("@FechaFin",reserva.FechaFin);
            command.Parameters.AddWithValue("@FechaFinOriginal",reserva.FechaFinOriginal);
            command.Parameters.AddWithValue("@MontoPorDia",reserva.MontoPorDia);
            command.Parameters.AddWithValue("@PorcentajeReserva",reserva.PorcentajeReserva);
            command.Parameters.AddWithValue("@Estado",reserva.Estado);
            command.Parameters.AddWithValue("@FechaTerminacion",(object?) reserva.FechaTerminacion ?? DBNull.Value);
            command.Parameters.AddWithValue("@Multa",(object?) reserva.Multa ?? DBNull.Value);
            command.Parameters.AddWithValue("@ReservaRenovadaDeId",(object?) reserva.ReservaRenovadaDeId ?? DBNull.Value);
            command.Parameters.AddWithValue("@UsuarioCreadorId",reserva.UsuarioCreadorId);
            command.Parameters.AddWithValue("@UsuarioTerminadorId", (object?) reserva.UsuarioTerminadorId ?? DBNull.Value);
            command.Parameters.AddWithValue("@FechaCreacion",reserva.FechaCreacion);
            await command.ExecuteNonQueryAsync();
        }

        public async Task ActualizarAsync(Reserva reserva)
        {
            using var connection = new MySqlConnection(_database.ConnectionString);
            await connection.OpenAsync();
            var query = @"UPDATE reservas 
                        SET InquilinoId = @InquilinoId,
                            InmuebleId = @InmuebleId,
                            FechaInicio = @FechaInicio,
                            FechaFin = @FechaFin,
                            FechaFinOriginal =@FechaFinOriginal,
                            MontoPorDia = @MontoPorDia,
                            PorcentajeReserva = @PorcentajeReserva,
                            Estado = @Estado ,
                            FechaTerminacion = @FechaTerminacion, 
                            Multa = @Multa,
                            ReservaRenovadaDeId = @ReservaRenovadaDeId,
                            UsuarioCreadorId = @UsuarioCreadorId,
                            UsuarioTerminadorId = @UsuarioTerminadorId,
                            FechaCreacion = @FechaCreacion
                            WHERE Id = @Id";
            using var command = new MySqlCommand(query,connection);
            command.Parameters.AddWithValue("@InquilinoId",reserva.InquilinoId);
            command.Parameters.AddWithValue("@InmuebleId",reserva.InmuebleId);
            command.Parameters.AddWithValue("@FechaInicio",reserva.FechaInicio);
            command.Parameters.AddWithValue("@FechaFin",reserva.FechaFin);
            command.Parameters.AddWithValue("@FechaFinOriginal",reserva.FechaFinOriginal);
            command.Parameters.AddWithValue("@MontoPorDia",reserva.MontoPorDia);
            command.Parameters.AddWithValue("@PorcentajeReserva",reserva.PorcentajeReserva);
            command.Parameters.AddWithValue("@Estado",reserva.Estado);
            command.Parameters.AddWithValue("@FechaTerminacion",(object?) reserva.FechaTerminacion ?? DBNull.Value);
            command.Parameters.AddWithValue("@Multa",(object?) reserva.Multa ?? DBNull.Value);
            command.Parameters.AddWithValue("@ReservaRenovadaDeId",(object?) reserva.ReservaRenovadaDeId ?? DBNull.Value);
            command.Parameters.AddWithValue("@UsuarioCreadorId",reserva.UsuarioCreadorId);
            command.Parameters.AddWithValue("@UsuarioTerminadorId", (object?) reserva.UsuarioTerminadorId ?? DBNull.Value);
            command.Parameters.AddWithValue("@FechaCreacion",reserva.FechaCreacion);
            command.Parameters.AddWithValue("@Id",reserva.Id);
            await command.ExecuteNonQueryAsync();

        }

        public async Task EliminarAsync(int id)
        {
            using var connection = new MySqlConnection(_database.ConnectionString);
            await connection.OpenAsync();
            var query = @"DELETE reservas WHERE Id = @Id";
            using var command = new MySqlCommand(query,connection);
            command.Parameters.AddWithValue("@Id",id);
            await command.ExecuteNonQueryAsync();
            
                
            
        }

        public async Task<bool> FechaReservadaAsync(int inquilinoId, DateTime fechaInicio,DateTime fechaFin, int? excluirReservaId = null)
        {
            using var connection = new MySqlConnection(_database.ConnectionString);
            await connection.OpenAsync();
            var query = @"SELECT COUNT(*)
                        FROM reservas
                        WHERE InquilinoId = @InquilinoId
                        AND Estado = 'Vigente'
                        AND FechaInicio < @FechaFin
                        AND FechaFin > @FechaInicio
                        AND (@ExcluirReservaId IS NULL OR Id != @ExcluirReservaId)";
            using var command = new MySqlCommand(query,connection);
            command.Parameters.AddWithValue("@InquilinoId",inquilinoId);
            command.Parameters.AddWithValue("@FechaInicio",fechaInicio);
            command.Parameters.AddWithValue("@FechaFin",fechaFin);
            command.Parameters.AddWithValue("@ExcluirReservaId", (object?) excluirReservaId ?? DBNull.Value );            
            var resultado = await command.ExecuteScalarAsync();
            var cantidad  = Convert.ToInt32(resultado);
            return cantidad > 0;
        }

        public async Task CambiarEstadoAsync(int id, string estado, decimal? multa,DateTime? fechaFin, int usuarioTerminadorId,DateTime fechaTerminacion)
        {
            using var connection = new MySqlConnection(_database.ConnectionString);
            await connection.OpenAsync();
            var query = @"
                UPDATE reservas
                SET Estado = @Estado,
                    Multa = @Multa,
                    FechaFin = COALESCE(@FechaFin, FechaFin),
                    UsuarioTerminadorId = @UsuarioId,
                    FechaTerminacion = @FechaTerminacion
                WHERE Id = @Id";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Estado", estado);
            command.Parameters.AddWithValue("@Multa", (object?)multa ?? DBNull.Value);
            command.Parameters.AddWithValue("@FechaFin", (object?)fechaFin ?? DBNull.Value);
            command.Parameters.AddWithValue("@UsuarioTerminadorId", usuarioTerminadorId);
            command.Parameters.AddWithValue("@FechaTerminacion", fechaTerminacion);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }
    }
}