using ShieldBox.Interfaces;
namespace ShieldBox.BE;
public abstract class PermisoCompuesto : IPermiso
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public abstract void AgregarPermiso(IPermiso p);
    public abstract void QuitarPermiso(IPermiso p);
    public abstract IList<IPermiso> ObtenerHijos();
    public override string ToString() => Nombre;
}
public sealed class Familia : PermisoCompuesto
{
    private readonly IList<IPermiso> _hijos = new List<IPermiso>();
    public override void AgregarPermiso(IPermiso p) { if (!_hijos.Contains(p)) _hijos.Add(p); }
    public override void QuitarPermiso(IPermiso p) => _hijos.Remove(p);
    public override IList<IPermiso> ObtenerHijos() => _hijos.ToArray();
}
public sealed class Patente : PermisoCompuesto
{
    public override void AgregarPermiso(IPermiso p) { }
    public override void QuitarPermiso(IPermiso p) { }
    public override IList<IPermiso> ObtenerHijos() => new List<IPermiso>();
}
public static class PermisoAuthorization
{
    public static bool HasPermission(IEnumerable<IPermiso> permisos, string name) => permisos.Any(p => p.Nombre == name || HasPermission(p.ObtenerHijos(), name));
}
