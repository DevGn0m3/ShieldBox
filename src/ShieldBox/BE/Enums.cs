namespace ShieldBox.BE;

public enum RequestStatus
{
    Pending,
    Approved,
    Rejected
}

public enum RiskLevel
{
    Low,
    Medium,
    High
}

public enum ReconciliationResult
{
    Reconciled,
    Review,
    Critical
}
