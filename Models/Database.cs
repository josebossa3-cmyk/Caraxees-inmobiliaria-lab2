using Microsoft.Extensions.Configuration;

namespace inmobiliaria.Models
{
  public class Database
  {
    private readonly string _connectionString;

    public Database(IConfiguration configuration)
    {
      _connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' no encontrada.");
    }

    public string ConnectionString => _connectionString;
  }
}