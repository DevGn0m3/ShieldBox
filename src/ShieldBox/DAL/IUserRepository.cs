using ShieldBox.BE;
namespace ShieldBox.DAL;
public interface IUserRepository { User? FindByLogin(string login); }
