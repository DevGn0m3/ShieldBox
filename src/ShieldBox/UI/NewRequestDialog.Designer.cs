namespace ShieldBox.UI;

public sealed partial class NewRequestDialog
{
    private TableLayoutPanel requestLayout;
    private Label requestTitleLabel;
    private Label walletLabel;
    private Label amountLabel;
    private Label recipientLabel;
    private Label conceptLabel;
    private Label evidenceLabel;
    private Button cancelButton;
    private Button submitButton;
    private FlowLayoutPanel requestButtons;
    private Panel walletFieldPanel;
    private Panel amountFieldPanel;
    private Panel recipientFieldPanel;
    private Panel conceptFieldPanel;
    private Panel evidenceFieldPanel;

    private void InitializeComponent()
    {
        this.requestLayout = new TableLayoutPanel();
        this.requestTitleLabel = new Label();
        this.walletLabel = new Label();
        this.amountLabel = new Label();
        this.recipientLabel = new Label();
        this.conceptLabel = new Label();
        this.evidenceLabel = new Label();
        this.cancelButton = new Button();
        this.submitButton = new Button();
        this.requestButtons = new FlowLayoutPanel();
        this.walletFieldPanel = new Panel();
        this.amountFieldPanel = new Panel();
        this.recipientFieldPanel = new Panel();
        this.conceptFieldPanel = new Panel();
        this.evidenceFieldPanel = new Panel();
        this.SuspendLayout();
        this.requestLayout.Dock = DockStyle.Fill;
        this.requestLayout.Padding = new Padding(24);
        this.requestLayout.ColumnCount = 2;
        this.requestLayout.RowCount = 6;
        this.requestLayout.BackColor = UiKit.Surface;
        this.requestLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        this.requestLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        this.requestLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
        this.requestLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 66F));
        this.requestLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 66F));
        this.requestLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        this.requestLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 62F));
        this.requestLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
        this.requestTitleLabel.Dock = DockStyle.Fill;
        this.requestTitleLabel.Text = "Nueva solicitud de transferencia";
        this.requestTitleLabel.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        this.requestTitleLabel.ForeColor = UiKit.Ink;
        this.requestTitleLabel.BackColor = UiKit.Surface;
        this.requestTitleLabel.TextAlign = ContentAlignment.MiddleLeft;
        this.requestLayout.Controls.Add(this.requestTitleLabel, 0, 0);
        this.requestLayout.SetColumnSpan(this.requestTitleLabel, 2);
        this.walletLabel.Dock = DockStyle.Top; this.walletLabel.Height = 24; this.walletLabel.Text = "Billetera / origen"; this.walletLabel.ForeColor = UiKit.Muted; this.walletLabel.BackColor = UiKit.Surface;
        this.amountLabel.Dock = DockStyle.Top; this.amountLabel.Height = 24; this.amountLabel.Text = "Monto (ARS)"; this.amountLabel.ForeColor = UiKit.Muted; this.amountLabel.BackColor = UiKit.Surface;
        this.recipientLabel.Dock = DockStyle.Top; this.recipientLabel.Height = 24; this.recipientLabel.Text = "Destinatario"; this.recipientLabel.ForeColor = UiKit.Muted; this.recipientLabel.BackColor = UiKit.Surface;
        this.conceptLabel.Dock = DockStyle.Top; this.conceptLabel.Height = 24; this.conceptLabel.Text = "Concepto"; this.conceptLabel.ForeColor = UiKit.Muted; this.conceptLabel.BackColor = UiKit.Surface;
        this._wallet.Name = "walletComboBox"; this._wallet.Dock = DockStyle.Bottom; this._wallet.Height = 30; this._wallet.DropDownStyle = ComboBoxStyle.DropDownList; this._wallet.Items.AddRange(new object[] { "Mercado Pago", "Ualá Bis", "Banco" }); this._wallet.SelectedIndex = 0;
        this._amount.Name = "amountNumericUpDown"; this._amount.Dock = DockStyle.Bottom; this._amount.Height = 30; this._amount.DecimalPlaces = 0; this._amount.Minimum = 1; this._amount.Maximum = 100000000; this._amount.Value = 480000; this._amount.ThousandsSeparator = true; this._amount.ValueChanged += new EventHandler(this.Amount_ValueChanged);
        this._recipient.Name = "recipientComboBox"; this._recipient.Dock = DockStyle.Bottom; this._recipient.Height = 30; this._recipient.DropDownStyle = ComboBoxStyle.DropDownList; this._recipient.Items.AddRange(new object[] { "Distribuidora Norte", "Caja chica", "Banco corporativo", "Proveedor nuevo" }); this._recipient.SelectedIndex = 0;
        this._concept.Name = "conceptComboBox"; this._concept.Dock = DockStyle.Bottom; this._concept.Height = 30; this._concept.DropDownStyle = ComboBoxStyle.DropDownList; this._concept.Items.AddRange(new object[] { "Pago a proveedor", "Retiro de excedente", "Reposición de caja chica", "Devolución a cliente" }); this._concept.SelectedIndex = 0;
        this.walletFieldPanel.Dock = DockStyle.Fill; this.walletFieldPanel.BackColor = UiKit.Surface; this.walletFieldPanel.Controls.Add(this._wallet); this.walletFieldPanel.Controls.Add(this.walletLabel);
        this.amountFieldPanel.Dock = DockStyle.Fill; this.amountFieldPanel.BackColor = UiKit.Surface; this.amountFieldPanel.Controls.Add(this._amount); this.amountFieldPanel.Controls.Add(this.amountLabel);
        this.recipientFieldPanel.Dock = DockStyle.Fill; this.recipientFieldPanel.BackColor = UiKit.Surface; this.recipientFieldPanel.Controls.Add(this._recipient); this.recipientFieldPanel.Controls.Add(this.recipientLabel);
        this.conceptFieldPanel.Dock = DockStyle.Fill; this.conceptFieldPanel.BackColor = UiKit.Surface; this.conceptFieldPanel.Controls.Add(this._concept); this.conceptFieldPanel.Controls.Add(this.conceptLabel);
        this.requestLayout.Controls.Add(this.walletFieldPanel, 0, 1); this.requestLayout.Controls.Add(this.amountFieldPanel, 1, 1); this.requestLayout.Controls.Add(this.recipientFieldPanel, 0, 2); this.requestLayout.Controls.Add(this.conceptFieldPanel, 1, 2);
        this.evidenceLabel.Dock = DockStyle.Top; this.evidenceLabel.Height = 24; this.evidenceLabel.Text = "Motivo y evidencia"; this.evidenceLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold); this.evidenceLabel.ForeColor = UiKit.Muted; this.evidenceLabel.BackColor = UiKit.Surface;
        this._evidence.Name = "evidenceTextBox"; this._evidence.Dock = DockStyle.Fill; this._evidence.Multiline = true; this._evidence.ScrollBars = ScrollBars.Vertical; this._evidence.Text = "Factura F-0004-1832 · OC-2098";
        this.evidenceFieldPanel.Dock = DockStyle.Fill; this.evidenceFieldPanel.BackColor = UiKit.Surface; this.evidenceFieldPanel.Controls.Add(this._evidence); this.evidenceFieldPanel.Controls.Add(this.evidenceLabel); this.requestLayout.Controls.Add(this.evidenceFieldPanel, 0, 3); this.requestLayout.SetColumnSpan(this.evidenceFieldPanel, 2);
        this._evaluation.Name = "evaluationLabel"; this._evaluation.Dock = DockStyle.Fill; this._evaluation.Text = "✓  Requiere validación del aprobador de turno. · Riesgo: Medio · Firmas necesarias: 1"; this._evaluation.Padding = new Padding(12); this._evaluation.Font = new Font("Segoe UI", 9F, FontStyle.Bold); this._evaluation.ForeColor = UiKit.Amber; this._evaluation.BackColor = UiKit.AmberBackground;
        this.requestLayout.Controls.Add(this._evaluation, 0, 4); this.requestLayout.SetColumnSpan(this._evaluation, 2);
        this.requestButtons.Dock = DockStyle.Fill; this.requestButtons.FlowDirection = FlowDirection.RightToLeft; this.requestButtons.WrapContents = false; this.requestButtons.Padding = new Padding(0, 8, 0, 0);
        this.cancelButton.Name = "cancelButton"; this.cancelButton.Text = "Cancelar"; this.cancelButton.Width = 120; this.cancelButton.Height = 38; this.cancelButton.DialogResult = DialogResult.Cancel; this.cancelButton.FlatStyle = FlatStyle.Flat; this.cancelButton.BackColor = Color.White; this.cancelButton.ForeColor = UiKit.Ink;
        this.submitButton.Name = "submitButton"; this.submitButton.Text = "✓  Enviar a aprobación"; this.submitButton.Width = 190; this.submitButton.Height = 38; this.submitButton.FlatStyle = FlatStyle.Flat; this.submitButton.BackColor = UiKit.Blue; this.submitButton.ForeColor = Color.White; this.submitButton.Click += new EventHandler(this.SubmitButton_Click);
        this.requestButtons.Controls.Add(this.cancelButton); this.requestButtons.Controls.Add(this.submitButton); this.requestLayout.Controls.Add(this.requestButtons, 0, 5); this.requestLayout.SetColumnSpan(this.requestButtons, 2);
        this.Controls.Add(this.requestLayout); this.AcceptButton = this.submitButton; this.CancelButton = this.cancelButton;
        this.AutoScaleMode = AutoScaleMode.Dpi; this.AutoScaleDimensions = new SizeF(96F, 96F); this.Text = "Nueva solicitud de transferencia · ShieldBox"; this.StartPosition = FormStartPosition.CenterParent; this.MinimumSize = new Size(680, 520); this.ClientSize = new Size(760, 590); this.BackColor = UiKit.Background; this.Font = new Font("Segoe UI", 9F);
        this.ResumeLayout(false);
    }
}
