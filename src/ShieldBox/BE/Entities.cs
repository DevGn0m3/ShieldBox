namespace ShieldBox.BE;

public sealed class Role
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
}

public sealed class User
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Login { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public string PasswordHash { get; init; } = string.Empty;
    public bool IsActive { get; init; } = true;
}

public sealed class Wallet
{
    public int Id { get; init; }
    public string Provider { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string PermissionMode { get; init; } = "ReadOnly";
}

public sealed class Policy
{
    public int Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public decimal DailyLimit { get; set; }
    public decimal WeeklyLimit { get; set; }
    public decimal MonthlyLimit { get; set; }
    public decimal DoubleApprovalFrom { get; set; }
    public bool IsActive { get; set; } = true;
}

public sealed class Approval
{
    public string Approver { get; init; } = string.Empty;
    public int Order { get; init; }
    public DateTime At { get; init; } = DateTime.Now;
    public string Decision { get; init; } = "Approved";
}

public sealed class TransferRequest
{
    public string Code { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; } = DateTime.Now;
    public string RequestedBy { get; init; } = string.Empty;
    public string Wallet { get; init; } = string.Empty;
    public string Recipient { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Concept { get; init; } = string.Empty;
    public string Evidence { get; init; } = string.Empty;
    public RequestStatus Status { get; set; } = RequestStatus.Pending;
    public RiskLevel Risk { get; set; } = RiskLevel.Low;
    public List<Approval> Approvals { get; } = new();
    public int RequiredApprovals => Amount > 350000 ? 2 : 1;
    public string Signatures => $"{Approvals.Count}/{RequiredApprovals}";
}

public sealed class Movement
{
    public string Code { get; init; } = string.Empty;
    public string Psp { get; init; } = string.Empty;
    public DateTime At { get; init; } = DateTime.Now;
    public decimal Amount { get; init; }
    public string LinkedRequest { get; init; } = string.Empty;
    public ReconciliationResult Result { get; set; }
    public string Note { get; init; } = string.Empty;
}

public sealed class AuditEvent
{
    public DateTime At { get; init; } = DateTime.Now;
    public string Actor { get; init; } = string.Empty;
    public string EventType { get; init; } = string.Empty;
    public string EntityType { get; init; } = string.Empty;
    public string EntityCode { get; init; } = string.Empty;
    public string Severity { get; init; } = string.Empty;
}
