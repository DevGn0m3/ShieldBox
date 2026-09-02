namespace ShieldBox.Security;
public static class PasswordHasher
{
    public static string Hash(string password) => Services.PasswordHasher.Hash(password);
    public static bool Verify(string password, string encoded) => Services.PasswordHasher.Verify(password, encoded);
}
