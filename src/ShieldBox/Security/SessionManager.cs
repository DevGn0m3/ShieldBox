namespace ShieldBox.Security;

public sealed class SessionManager
{
    private static readonly Lazy<SessionManager> _instance = new(() => new SessionManager());

    private SessionManager() { }

    public static SessionManager GetInstance() => _instance.Value;

    public int UserId { get; private set; }
    public string UserName { get; private set; } = string.Empty;
    public string Role { get; private set; } = string.Empty;
    public bool IsAuthenticated => UserId > 0;

    public void Start(int userId, string userName, string role)
    {
        UserId = userId;
        UserName = userName;
        Role = role;
    }

    public void Clear()
    {
        UserId = 0;
        UserName = string.Empty;
        Role = string.Empty;
    }
}
