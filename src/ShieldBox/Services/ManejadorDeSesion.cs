using ShieldBox.BE;
using ShieldBox.Interfaces;
namespace ShieldBox.Services;
public static class ManejadorDeSesion
{
    private static readonly IList<IIdiomaObserver> _observers = new List<IIdiomaObserver>();
    public static User? Session => SingletonSesion.Instancia.Usuario;
    public static bool IsLogged() => SingletonSesion.Instancia.IsLogged();
    public static void Login(User user) => SingletonSesion.Instancia.Login(user);
    public static void Logout() => SingletonSesion.Instancia.Logout();
    public static void SuscribirObservador(IIdiomaObserver observer) { if (!_observers.Contains(observer)) _observers.Add(observer); }
    public static void DesuscribirObservador(IIdiomaObserver observer) => _observers.Remove(observer);
    public static void CambiarIdioma(IIdioma idioma) { foreach (var observer in _observers.ToArray()) observer.UpdateLanguage(idioma); }
}
