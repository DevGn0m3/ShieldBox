using ShieldBox.DAL;
using ShieldBox.Security;
using ShieldBox.Services;
using PasswordHashing = ShieldBox.Services.PasswordHasher;

namespace ShieldBox.BLL;

public sealed class AuthService
{
    private readonly DemoStore _store;

    public AuthService(DemoStore store) => _store = store;

    public bool Authenticate(string login, string password, out string error)
    {
        error = string.Empty;
        var user = _store.Users.FirstOrDefault(u => u.Login.Equals(login.Trim(), StringComparison.OrdinalIgnoreCase));
        if (user is null || !user.IsActive)
        {
            error = "Usuario inexistente o inactivo.";
            return false;
        }

        if (!PasswordHashing.Verify(password, user.PasswordHash))
        {
            error = "Las credenciales no son válidas.";
            return false;
        }

        ManejadorDeSesion.Login(user);
        _store.AddAudit(user.Name, "Login", "User", user.Login, "Success");
        return true;
    }

    public void Logout()
    {
        if (ManejadorDeSesion.Session is { } session) _store.AddAudit(session.Name, "Logout", "User", session.Login, "Info");
        ManejadorDeSesion.Logout();
    }
}
