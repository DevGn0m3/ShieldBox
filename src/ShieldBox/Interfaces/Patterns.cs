namespace ShieldBox.Interfaces;
public interface IEntity { int Id { get; set; } }
public interface IPermiso : IEntity
{
    string Nombre { get; set; }
    void AgregarPermiso(IPermiso p);
    void QuitarPermiso(IPermiso p);
    IList<IPermiso> ObtenerHijos();
}
public interface IIdiomaObserver { void UpdateLanguage(IIdioma idioma); }
public interface IIdioma : IEntity { string Nombre { get; set; } bool Default { get; set; } }
public interface IRepository<T> where T : class { T? GetById(int id); IList<T> GetAll(); void Save(T entity); void Delete(T entity); }
