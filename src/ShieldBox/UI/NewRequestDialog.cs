using ShieldBox.BE;
using ShieldBox.BLL;

namespace ShieldBox.UI;

public sealed partial class NewRequestDialog : Form
{
    private readonly ShieldBoxService _service;
    private readonly ComboBox _wallet = new();
    private readonly NumericUpDown _amount = new();
    private readonly ComboBox _recipient = new();
    private readonly ComboBox _concept = new();
    private readonly TextBox _evidence = new();
    private readonly Label _evaluation = new();

    public string RequestedBy { get; set; } = "Martín García";
    public string WalletName => _wallet.Text;
    public decimal Amount => _amount.Value;
    public string RecipientName => _recipient.Text;
    public string ConceptName => _concept.Text;
    public string EvidenceText => _evidence.Text;

    public NewRequestDialog() : this(new ShieldBox.BLL.ShieldBoxService(new ShieldBox.DAL.DemoStore())) { }

    public NewRequestDialog(ShieldBoxService service)
    {
        _service = service;
        InitializeComponent();
        Text = "Nueva solicitud de transferencia · ShieldBox";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.Sizable;
        AutoScaleMode = AutoScaleMode.Dpi;
        AutoScaleDimensions = new SizeF(96F, 96F);
        MinimumSize = new Size(680, 520);
        ClientSize = new Size(760, 590);
        BackColor = UiKit.Background;
        Font = new Font("Segoe UI", 9F);
    }

    private void Build()
    {
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(24), ColumnCount = 2, RowCount = 7, BackColor = UiKit.Surface, GrowStyle = TableLayoutPanelGrowStyle.FixedSize };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 66));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 66));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 64));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 18));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        Controls.Add(root);

        _wallet.Items.AddRange(new object[] { "Mercado Pago", "Ualá Bis", "Banco" });
        _wallet.SelectedIndex = 0; ConfigureCombo(_wallet);
        _amount.DecimalPlaces = 0; _amount.Maximum = 100000000; _amount.Minimum = 1; _amount.Value = 480000; _amount.ThousandsSeparator = true; _amount.Dock = DockStyle.Fill; _amount.ValueChanged += (_, _) => Evaluate();
        _recipient.Items.AddRange(new object[] { "Distribuidora Norte", "Caja chica", "Banco corporativo", "Proveedor nuevo" });
        _recipient.SelectedIndex = 0; ConfigureCombo(_recipient);
        _concept.Items.AddRange(new object[] { "Pago a proveedor", "Retiro de excedente", "Reposición de caja chica", "Devolución a cliente" });
        _concept.SelectedIndex = 0; ConfigureCombo(_concept);
        _evidence.Multiline = true; _evidence.ScrollBars = ScrollBars.Vertical; _evidence.Text = "Factura F-0004-1832 · OC-2098"; _evidence.Dock = DockStyle.Fill; _evidence.BorderStyle = BorderStyle.FixedSingle;
        AddField(root, "Billetera / origen", _wallet, 0, 0);
        AddField(root, "Monto (ARS)", _amount, 1, 0);
        AddField(root, "Destinatario", _recipient, 0, 1);
        AddField(root, "Concepto", _concept, 1, 1);
        var evidenceCaption = UiKit.Label("Motivo y evidencia", 9, FontStyle.Bold, UiKit.Muted); evidenceCaption.Dock = DockStyle.Fill; evidenceCaption.TextAlign = ContentAlignment.BottomLeft; root.Controls.Add(evidenceCaption, 0, 2); root.SetColumnSpan(evidenceCaption, 2);
        root.Controls.Add(_evidence, 0, 3); root.SetColumnSpan(_evidence, 2);
        _evaluation.AutoSize = false; _evaluation.Dock = DockStyle.Fill; _evaluation.Padding = new Padding(12); _evaluation.Margin = new Padding(0, 8, 0, 0); _evaluation.TextAlign = ContentAlignment.MiddleLeft; _evaluation.Font = new Font("Segoe UI", 9, FontStyle.Bold); root.Controls.Add(_evaluation, 0, 4); root.SetColumnSpan(_evaluation, 2);
        var buttons = new FlowLayoutPanel { FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Fill, WrapContents = false, AutoSize = false, Padding = new Padding(0, 8, 0, 0) };
        buttons.Controls.Add(UiKit.Button("Enviar a aprobación", (_, _) => Submit(), true, "✓"));
        buttons.Controls.Add(UiKit.Button("Cancelar", (_, _) => DialogResult = DialogResult.Cancel));
        root.Controls.Add(buttons, 0, 6); root.SetColumnSpan(buttons, 2);
    }

    private static void ConfigureCombo(ComboBox combo)
    {
        combo.Dock = DockStyle.Fill;
        combo.DropDownStyle = ComboBoxStyle.DropDownList;
        combo.FlatStyle = FlatStyle.Standard;
        combo.Margin = Padding.Empty;
    }

    private static void AddField(TableLayoutPanel root, string caption, Control control, int col, int row)
    {
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Margin = new Padding(0, 0, 10, 0), Padding = Padding.Empty };
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        var label = UiKit.Label(caption, 8, FontStyle.Bold, UiKit.Muted); label.Dock = DockStyle.Fill; label.TextAlign = ContentAlignment.MiddleLeft;
        panel.Controls.Add(label, 0, 0); panel.Controls.Add(control, 0, 1);
        root.Controls.Add(panel, col, row);
    }

    private void Amount_ValueChanged(object? sender, EventArgs e) => Evaluate();

    private void SubmitButton_Click(object? sender, EventArgs e) => Submit();

    private void Evaluate()
    {
        var evaluation = _service.Evaluate(_amount.Value);
        var isHigh = evaluation.Risk == RiskLevel.High;
        var isMedium = evaluation.Risk == RiskLevel.Medium;
        _evaluation.Text = $"{(isHigh ? "⚠" : "✓")}  {evaluation.Message}   ·   Riesgo: {evaluation.Risk}   ·   Firmas necesarias: {evaluation.RequiredApprovals}";
        _evaluation.ForeColor = isHigh ? UiKit.Red : isMedium ? UiKit.Amber : UiKit.Green;
        _evaluation.BackColor = isHigh ? UiKit.RedBackground : isMedium ? UiKit.AmberBackground : UiKit.GreenBackground;
    }

    private void Submit()
    {
        if (string.IsNullOrWhiteSpace(_evidence.Text)) { MessageBox.Show("Ingrese motivo y evidencia.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        DialogResult = DialogResult.OK;
    }
}
