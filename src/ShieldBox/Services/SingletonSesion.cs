using ShieldBox.BE;
namespace ShieldBox.Services;
public sealed class Sesion
{
    public User? Usuario { get; private set; }
    public bool IsLogged() => Usuario is not null;
    public void Login(User user) => Usuario = user;
    public void Logout() => Usuario = null;
}
public static class SingletonSesion
{
    private static Sesion? _instancia;
    private static readonly object _lock = new();
    public static Sesion Instancia
    {
        get { lock (_lock) { _instancia ??= new Sesion(); return _instancia; } }
    }
}
