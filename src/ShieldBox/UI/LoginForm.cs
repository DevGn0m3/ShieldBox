using ShieldBox.BLL;
using ShieldBox.DAL;
using ShieldBox.Security;

namespace ShieldBox.UI;

public sealed partial class LoginForm : Form
{
    private readonly AuthService _auth;
    private readonly ShieldBoxService _service;
    private readonly TextBox _login = new();
    private readonly TextBox _password = new();
    private readonly Label _error = new();

    public LoginForm() : this(new ShieldBox.BLL.AuthService(new ShieldBox.DAL.DemoStore()), new ShieldBox.BLL.ShieldBoxService(new ShieldBox.DAL.DemoStore())) { }

    public LoginForm(AuthService auth, ShieldBoxService service)
    {
        _auth = auth;
        _service = service;
        InitializeComponent();
        Text = "ShieldBox — Iniciar sesión";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(900, 560);
        Size = new Size(1060, 680);
        BackColor = UiKit.Background;
        AutoScaleMode = AutoScaleMode.Dpi;
        AutoScaleDimensions = new SizeF(96F, 96F);
    }

    private void Build()
    {
        var shell = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, BackColor = UiKit.Background };
        shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 46));
        shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 54));

        var brand = new Panel { Dock = DockStyle.Fill, BackColor = UiKit.Navy, Padding = new Padding(56) };
        var brandStack = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true, BackColor = UiKit.Navy };
        var mark = UiKit.Icon("S", 17, Color.White); mark.BackColor = UiKit.Cyan; mark.Margin = new Padding(0, 24, 0, 20); mark.Size = new Size(48, 48);
        brandStack.Controls.Add(mark);
        brandStack.Controls.Add(UiKit.Label("ShieldBox", 28, FontStyle.Bold, Color.White));
        brandStack.Controls.Add(UiKit.Label("Control analítico de caja", 11, FontStyle.Regular, Color.FromArgb(159, 185, 211)));
        brandStack.Controls.Add(new Panel { Height = 34, Width = 10 });
        brandStack.Controls.Add(UiKit.Label("Protegé el circuito de fondos con\nreglas claras, doble aprobación\ny evidencia auditable.", 17, FontStyle.Bold, Color.White));
        brandStack.Controls.Add(new Panel { Height = 24, Width = 10 });
        brandStack.Controls.Add(UiKit.Label("Mockup funcional · SQL Server · WinForms", 10, FontStyle.Regular, Color.FromArgb(159, 185, 211)));
        brand.Controls.Add(brandStack);

        var cardHost = new Panel { Dock = DockStyle.Fill, BackColor = UiKit.Background, Padding = new Padding(48) };
        var card = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(34), BorderStyle = BorderStyle.FixedSingle };
        var content = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 8, BackColor = Color.White };
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 78));
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        content.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        content.Controls.Add(UiKit.Label("Iniciar sesión", 22, FontStyle.Bold, UiKit.Ink), 0, 0);
        content.Controls.Add(UiKit.Label("Accedé con un usuario del circuito de control.", 10, FontStyle.Regular, UiKit.Muted), 0, 1);
        content.Controls.Add(Field("Usuario", _login, "mgarcia"), 0, 2);
        content.Controls.Add(Field("Contraseña", _password, "demo123"), 0, 3);
        _password.UseSystemPasswordChar = true;
        _error.ForeColor = UiKit.Red; _error.Font = new Font("Segoe UI", 9); _error.AutoSize = true; content.Controls.Add(_error, 0, 5);
        var loginButton = UiKit.Button("Ingresar al sistema", (_, _) => Login(), true); loginButton.Dock = DockStyle.Fill; content.Controls.Add(loginButton, 0, 6);
        content.Controls.Add(UiKit.Label("Demo: mgarcia / demo123 · lfernandez / demo123 · jperez / demo123", 8, FontStyle.Regular, UiKit.Muted), 0, 7);
        card.Controls.Add(content); cardHost.Controls.Add(card); shell.Controls.Add(brand, 0, 0); shell.Controls.Add(cardHost, 1, 0); Controls.Add(shell); AcceptButton = loginButton;
    }

    private static Control Field(string label, TextBox box, string placeholder)
    {
        var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
        var text = UiKit.Label(label, 9, FontStyle.Bold, UiKit.Muted); text.Dock = DockStyle.Top; text.Height = 22;
        box.Dock = DockStyle.Bottom; box.Height = 32; box.PlaceholderText = placeholder; box.BorderStyle = BorderStyle.FixedSingle;
        panel.Controls.Add(box); panel.Controls.Add(text); return panel;
    }

    private void LoginButton_Click(object? sender, EventArgs e) => Login();

    private void Login()
    {
        if (_auth.Authenticate(_login.Text, _password.Text, out var error))
        {
            Hide();
            using var main = new MainForm(_service);
            main.ShowDialog(this);
            Show();
            _password.Clear();
        }
        else _error.Text = error;
    }
}
