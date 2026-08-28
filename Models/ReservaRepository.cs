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

    }
}