using ShieldBox.DAL;
using ShieldBox.BE;

namespace ShieldBox.BLL;

public sealed class ShieldBoxService
{
    private readonly DemoStore _store;
    private int _sequence = 1046;

    public ShieldBoxService(DemoStore store) => _store = store;
    public IReadOnlyList<TransferRequest> Requests => _store.Requests;
    public IReadOnlyList<Movement> Movements => _store.Movements;
    public IReadOnlyList<Policy> Policies => _store.Policies;
    public IReadOnlyList<AuditEvent> AuditEvents => _store.AuditEvents;
    public IReadOnlyList<AuditEvent> SearchAudit(AuditFilter filter) => _store.SearchAudit(filter);
    public int PendingCount => _store.Requests.Count(r => r.Status == RequestStatus.Pending);
    public int CriticalMovementCount => _store.Movements.Count(m => m.Result == ReconciliationResult.Critical);

    public (RiskLevel Risk, int RequiredApprovals, string Message) Evaluate(decimal amount)
    {
        if (amount > 350000)
            return (RiskLevel.High, 2, "Supera el límite diario del operador. Se requieren dos firmas.");
        if (amount > 120000)
            return (RiskLevel.Medium, 1, "Requiere validación del aprobador de turno.");
        return (RiskLevel.Low, 1, "Dentro del límite operativo. Requiere una aprobación de turno.");
    }

    public TransferRequest CreateRequest(string requestedBy, string wallet, string recipient, decimal amount, string concept, string evidence)
    {
        var evaluation = Evaluate(amount);
        var request = new TransferRequest
        {
            Code = $"SB-{_sequence++}",
            CreatedAt = DateTime.Now,
            RequestedBy = requestedBy,
            Wallet = wallet,
            Recipient = recipient,
            Amount = amount,
            Concept = concept,
            Evidence = evidence,
            Risk = evaluation.Risk,
            Status = RequestStatus.Pending
        };
        _store.Requests.Insert(0, request);
        AddAudit("Sistema", "Solicitud creada", request.Code, "Revisión");
        return request;
    }

    public bool Approve(string code, string approver)
    {
        var request = Find(code);
        if (request is null || request.Status != RequestStatus.Pending || request.Approvals.Any(a => a.Approver == approver))
            return false;
        var order = request.Approvals.Count + 1;
        request.Approvals.Add(new Approval { Approver = approver, Order = order });
        AddAudit(approver, order == 1 ? "Primera firma" : "Segunda firma", request.Code, "Éxito");
        if (request.Approvals.Count >= request.RequiredApprovals)
        {
            request.Status = RequestStatus.Approved;
            request.Risk = RiskLevel.Low;
        }
        return true;
    }

    public bool Reject(string code, string approver)
    {
        var request = Find(code);
        if (request is null || request.Status != RequestStatus.Pending) return false;
        request.Status = RequestStatus.Rejected;
        AddAudit(approver, "Solicitud rechazada", request.Code, "Crítico");
        return true;
    }

    public TransferRequest? Find(string code) => _store.Requests.FirstOrDefault(r => r.Code == code);

    public void MarkReconciled(string code)
    {
        var movement = _store.Movements.FirstOrDefault(m => m.Code == code);
        if (movement is null) return;
        movement.Result = ReconciliationResult.Reconciled;
        AddAudit("Administrador", "Conciliación revisada", code, "Éxito");
    }

    private void AddAudit(string actor, string eventType, string entityCode, string severity)
    {
        _store.AuditEvents.Insert(0, new AuditEvent { At = DateTime.Now, Actor = actor, EventType = eventType, EntityCode = entityCode, Severity = severity });
    }
}
