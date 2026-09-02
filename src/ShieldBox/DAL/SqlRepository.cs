using System.Data;
using Microsoft.Data.SqlClient;
using ShieldBox.BE;

namespace ShieldBox.DAL;

/// <summary>
/// Acceso ADO.NET parametrizado a SQL Server. El mockup utiliza DemoStore por defecto,
/// pero esta clase permite conectar la misma aplicación a ShieldBoxDemo.
/// </summary>
public sealed class SqlRepository
{
    public string ConnectionString { get; }

    public SqlRepository(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public SqlConnection CreateConnection() => new(ConnectionString);

    public async Task<User?> FindUserAsync(string login, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT u.UserId, u.UserName, u.LoginName, r.RoleName, u.PasswordHash, u.IsActive
            FROM dbo.Users AS u
            INNER JOIN dbo.Roles AS r ON r.RoleId = u.RoleId
            WHERE u.LoginName = @login;
            """;
        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@login", SqlDbType.NVarChar, 80).Value = login.Trim();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken)) return null;
        return new User
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Login = reader.GetString(2),
            Role = reader.GetString(3),
            PasswordHash = reader.GetString(4),
            IsActive = reader.GetBoolean(5)
        };
    }

    public async Task<IReadOnlyList<AuditEvent>> SearchAuditAsync(AuditFilter filter, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT EventAt, Actor, EventType, EntityType, EntityCode, EventData, Severity
            FROM dbo.AuditEvents
            WHERE (@from IS NULL OR EventAt >= @from)
              AND (@to IS NULL OR EventAt <= @to)
              AND (@actor = N'' OR Actor LIKE N'%' + @actor + N'%')
              AND (@activity = N'' OR EventType LIKE N'%' + @activity + N'%')
              AND (@information = N'' OR EntityType LIKE N'%' + @information + N'%' OR EntityCode LIKE N'%' + @information + N'%' OR EventData LIKE N'%' + @information + N'%' OR Severity LIKE N'%' + @information + N'%')
            ORDER BY EventAt DESC;
            """;
        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@from", SqlDbType.DateTime2).Value = filter.From.HasValue ? filter.From.Value : DBNull.Value;
        command.Parameters.Add("@to", SqlDbType.DateTime2).Value = filter.To.HasValue ? filter.To.Value : DBNull.Value;
        command.Parameters.Add("@actor", SqlDbType.NVarChar, 120).Value = filter.Actor.Trim();
        command.Parameters.Add("@activity", SqlDbType.NVarChar, 120).Value = filter.Activity.Trim();
        command.Parameters.Add("@information", SqlDbType.NVarChar, 160).Value = filter.Information.Trim();
        var result = new List<AuditEvent>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            result.Add(new AuditEvent { At = reader.GetDateTime(0), Actor = reader.GetString(1), EventType = reader.GetString(2), EntityType = reader.GetString(3), EntityCode = reader.IsDBNull(4) ? string.Empty : reader.GetString(4), Severity = reader.GetString(6) });
        }
        return result;
    }

    public async Task AddAuditAsync(string actor, string eventType, string entityType, string entityCode, string severity, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT dbo.AuditEvents(Actor, EventType, EntityType, EntityCode, Severity)
            VALUES (@actor, @eventType, @entityType, @entityCode, @severity);
            """;
        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@actor", SqlDbType.NVarChar, 120).Value = actor;
        command.Parameters.Add("@eventType", SqlDbType.NVarChar, 80).Value = eventType;
        command.Parameters.Add("@entityType", SqlDbType.NVarChar, 80).Value = entityType;
        command.Parameters.Add("@entityCode", SqlDbType.NVarChar, 80).Value = entityCode;
        command.Parameters.Add("@severity", SqlDbType.NVarChar, 20).Value = severity;
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
