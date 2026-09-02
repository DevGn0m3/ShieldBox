namespace ShieldBox.BE;

/// <summary>
/// Criterios opcionales combinables para consultar la bitácora.
/// Un valor nulo o vacío significa que ese filtro no se aplica.
/// </summary>
public sealed class AuditFilter
{
    public DateTime? From { get; init; }
    public DateTime? To { get; init; }
    public string Actor { get; init; } = string.Empty;
    public string Activity { get; init; } = string.Empty;
    public string Information { get; init; } = string.Empty;
}
