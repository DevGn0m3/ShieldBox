using ShieldBox.BE;
using ShieldBox.Security;
using ShieldBox.Interfaces;

namespace ShieldBox.DAL;

public sealed class DemoStore : IUserRepository
{
    public List<User> Users { get; } = new();
    public List<Wallet> Wallets { get; } = new();
    public List<Policy> Policies { get; } = new();
    public List<TransferRequest> Requests { get; } = new();
    public List<Movement> Movements { get; } = new();
    public List<AuditEvent> AuditEvents { get; } = new();

    public DemoStore()
    {
        Users.AddRange(new[]
        {
            new User { Id = 1, Name = "Martín García", Login = "mgarcia", Role = "Administrador", PasswordHash = PasswordHasher.Hash("demo123") },
            new User { Id = 2, Name = "Laura Fernández", Login = "lfernandez", Role = "Aprobador", PasswordHash = PasswordHasher.Hash("demo123") },
            new User { Id = 3, Name = "Juan Pérez", Login = "jperez", Role = "Operador", PasswordHash = PasswordHasher.Hash("demo123") },
            new User { Id = 4, Name = "Lucía Gómez", Login = "lgomez", Role = "Operador", PasswordHash = PasswordHasher.Hash("demo123") }
        });

        Wallets.AddRange(new[]
        {
            new Wallet { Id = 1, Provider = "Mercado Pago", DisplayName = "Mercado Pago corporativo" },
            new Wallet { Id = 2, Provider = "Ualá Bis", DisplayName = "Ualá Bis corporativa" },
            new Wallet { Id = 3, Provider = "Banco", DisplayName = "Cuenta bancaria corporativa" }
        });

        Policies.AddRange(new[]
        {
            new Policy { Id = 1, Name = "Operador de caja", DailyLimit = 350000, WeeklyLimit = 900000, MonthlyLimit = 2500000, DoubleApprovalFrom = 350000 },
            new Policy { Id = 2, Name = "Retiro de excedente", DailyLimit = 120000, WeeklyLimit = 500000, MonthlyLimit = 1200000, DoubleApprovalFrom = 120000 },
            new Policy { Id = 3, Name = "Nuevo destinatario", DailyLimit = 0, WeeklyLimit = 0, MonthlyLimit = 0, DoubleApprovalFrom = 1 },
            new Policy { Id = 4, Name = "Excepción por emergencia", DailyLimit = 80000, WeeklyLimit = 0, MonthlyLimit = 0, DoubleApprovalFrom = 80000 }
        });

        var r1045 = NewRequest("SB-1045", "Juan Pérez", "Mercado Pago", "Distribuidora Norte", 480000, "Pago a proveedor", "Factura F-0004-1832 · OC-2098", RiskLevel.High);
        var r1044 = NewRequest("SB-1044", "Lucía Gómez", "Ualá Bis", "Caja chica", 125000, "Reposición de caja chica", "Comprobante adjunto", RiskLevel.Medium);
        var r1042 = NewRequest("SB-1042", "Juan Pérez", "Mercado Pago", "Banco corporativo", 125000, "Retiro de excedente", "Control de cierre", RiskLevel.Low);
        r1042.Approvals.Add(new Approval { Approver = "Laura Fernández", Order = 1 });
        r1042.Approvals.Add(new Approval { Approver = "Martín García", Order = 2 });
        r1042.Status = RequestStatus.Approved;
        var r1041 = NewRequest("SB-1041", "Martín García", "Mercado Pago", "Distribuidora Norte", 86000, "Pago a proveedor", "Orden OC-2097", RiskLevel.Low);
        r1041.Approvals.Add(new Approval { Approver = "Martín García", Order = 1 });
        r1041.Status = RequestStatus.Approved;

        Movements.AddRange(new[]
        {
            new Movement { Code = "MP-88421", Psp = "Mercado Pago", At = DateTime.Now.AddMinutes(-45), Amount = 220000, LinkedRequest = "Sin solicitud", Result = ReconciliationResult.Critical, Note = "Movimiento sin solicitud vinculada" },
            new Movement { Code = "MP-88420", Psp = "Mercado Pago", At = DateTime.Now.AddMinutes(-80), Amount = 125000, LinkedRequest = "SB-1042", Result = ReconciliationResult.Reconciled },
            new Movement { Code = "UA-77102", Psp = "Ualá Bis", At = DateTime.Now.AddMinutes(-100), Amount = 125000, LinkedRequest = "Sin evidencia", Result = ReconciliationResult.Review, Note = "Requiere evidencia" },
            new Movement { Code = "MP-88412", Psp = "Mercado Pago", At = DateTime.Now.AddHours(-4), Amount = 86000, LinkedRequest = "SB-1041", Result = ReconciliationResult.Reconciled }
        });

        AuditEvents.AddRange(new[]
        {
            new AuditEvent { At = DateTime.Now.AddMinutes(-18), Actor = "Sistema", EventType = "Sincronización completada", EntityCode = "Mercado Pago", Severity = "Informativo" },
            new AuditEvent { At = DateTime.Now.AddMinutes(-32), Actor = "Laura Fernández", EventType = "Segunda firma", EntityCode = "SB-1042", Severity = "Éxito" },
            new AuditEvent { At = DateTime.Now.AddMinutes(-55), Actor = "Sistema", EventType = "Payout sin respaldo", EntityCode = "MP-88421", Severity = "Crítico" },
            new AuditEvent { At = DateTime.Now.AddHours(-3), Actor = "Martín García", EventType = "Política modificada", EntityCode = "P-002", Severity = "Cambio" }
        });
    }

    public User? FindByLogin(string login) => Users.FirstOrDefault(u => u.Login.Equals(login.Trim(), StringComparison.OrdinalIgnoreCase));

    public void AddAudit(string actor, string eventType, string entityType, string entityCode, string severity)
    {
        AuditEvents.Insert(0, new AuditEvent { At = DateTime.Now, Actor = actor, EventType = eventType, EntityType = entityType, EntityCode = entityCode, Severity = severity });
    }

    public IReadOnlyList<AuditEvent> SearchAudit(AuditFilter filter)
    {
        IEnumerable<AuditEvent> query = AuditEvents;
        if (filter.From.HasValue) query = query.Where(e => e.At >= filter.From.Value);
        if (filter.To.HasValue) query = query.Where(e => e.At <= filter.To.Value);
        if (!string.IsNullOrWhiteSpace(filter.Actor)) query = query.Where(e => e.Actor.Contains(filter.Actor.Trim(), StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(filter.Activity)) query = query.Where(e => e.EventType.Contains(filter.Activity.Trim(), StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(filter.Information))
        {
            string term = filter.Information.Trim();
            query = query.Where(e => e.EntityType.Contains(term, StringComparison.OrdinalIgnoreCase) || e.EntityCode.Contains(term, StringComparison.OrdinalIgnoreCase) || e.Severity.Contains(term, StringComparison.OrdinalIgnoreCase));
        }
        return query.OrderByDescending(e => e.At).ToArray();
    }

    private TransferRequest NewRequest(string code, string user, string wallet, string recipient, decimal amount, string concept, string evidence, RiskLevel risk)
    {
        var request = new TransferRequest { Code = code, RequestedBy = user, Wallet = wallet, Recipient = recipient, Amount = amount, Concept = concept, Evidence = evidence, Risk = risk, CreatedAt = DateTime.Now.AddMinutes(-Requests.Count * 18) };
        Requests.Add(request);
        return request;
    }
}
