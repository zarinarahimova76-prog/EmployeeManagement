using Npgsql;

namespace Infrastructure.Data;

public class Database
{
    private readonly string _connectionString;

    public Database(string connectionString)
    {
        _connectionString = connectionString;
    }

    public NpgsqlConnection Connection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}