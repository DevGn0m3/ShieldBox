using ShieldBox.DAL;
using ShieldBox.UI;
using ShieldBox.BLL;

namespace ShieldBox;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        ApplicationConfiguration.Initialize();

        var store = new DemoStore();
        var service = new ShieldBoxService(store);
        var auth = new AuthService(store);
        Application.Run(new LoginForm(auth, service));
    }
}
