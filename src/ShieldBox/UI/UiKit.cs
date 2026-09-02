using System.Drawing.Drawing2D;
using System.Globalization;

namespace ShieldBox.UI;

public static class UiKit
{
    public static readonly Color Navy = Color.FromArgb(11, 31, 51);
    public static readonly Color Navy2 = Color.FromArgb(16, 45, 73);
    public static readonly Color Blue = Color.FromArgb(38, 116, 184);
    public static readonly Color Cyan = Color.FromArgb(49, 182, 199);
    public static readonly Color Ink = Color.FromArgb(24, 37, 56);
    public static readonly Color Background = Color.FromArgb(244, 247, 251);
    public static readonly Color Surface = Color.White;
    public static readonly Color Border = Color.FromArgb(226, 232, 240);
    public static readonly Color Muted = Color.FromArgb(109, 123, 140);
    public static readonly Color Green = Color.FromArgb(22, 130, 93);
    public static readonly Color GreenBackground = Color.FromArgb(231, 247, 239);
    public static readonly Color Amber = Color.FromArgb(166, 93, 0);
    public static readonly Color AmberBackground = Color.FromArgb(255, 243, 220);
    public static readonly Color Red = Color.FromArgb(189, 48, 40);
    public static readonly Color RedBackground = Color.FromArgb(255, 233, 231);

    public static int Scale(Control control, int value) => Math.Max(1, (int)Math.Round(value * control.DeviceDpi / 96f));
    public static Padding Pad(Control control, int all) => new(Scale(control, all));
    public static Padding Pad(Control control, int left, int top, int right, int bottom) => new(Scale(control, left), Scale(control, top), Scale(control, right), Scale(control, bottom));
    public static Font Font(Control control, float size, FontStyle style = FontStyle.Regular) => new("Segoe UI", size * control.DeviceDpi / 96f, style, GraphicsUnit.Point);

    public static Label Label(string text, float size = 10, FontStyle style = FontStyle.Regular, Color? color = null)
        => new()
        {
            Text = text,
            AutoSize = true,
            Font = new Font("Segoe UI", size, style),
            ForeColor = color ?? Ink,
            Margin = new Padding(0, 3, 0, 3),
            UseCompatibleTextRendering = false
        };

    public static Label Icon(string glyph, float size = 12, Color? color = null)
        => new()
        {
            Text = glyph,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI Symbol", size, FontStyle.Bold),
            ForeColor = color ?? Color.White,
            Margin = Padding.Empty,
            UseCompatibleTextRendering = false
        };

    public static Button Button(string text, EventHandler click, bool primary = false, string? icon = null)
    {
        var b = new Button
        {
            Text = string.IsNullOrWhiteSpace(icon) ? text : $"{icon}  {text}",
            AutoSize = true,
            MinimumSize = new Size(104, 38),
            Height = 38,
            Padding = new Padding(13, 0, 13, 0),
            FlatStyle = FlatStyle.Flat,
            BackColor = primary ? Blue : Surface,
            ForeColor = primary ? Color.White : Ink,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            Cursor = Cursors.Hand,
            UseCompatibleTextRendering = false,
            TabStop = true
        };
        b.FlatAppearance.BorderSize = 1;
        b.FlatAppearance.BorderColor = primary ? Blue : Border;
        b.FlatAppearance.MouseOverBackColor = primary ? Color.FromArgb(30, 99, 158) : Color.FromArgb(248, 251, 255);
        b.FlatAppearance.MouseDownBackColor = primary ? Color.FromArgb(25, 84, 136) : Color.FromArgb(238, 245, 251);
        b.Click += click;
        return b;
    }

    public static Panel CardPanel(string title, Control body, string? icon = null)
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Surface,
            Padding = new Padding(20),
            Margin = new Padding(6),
            AutoScroll = true
        };
        panel.Paint += (_, e) =>
        {
            using var pen = new Pen(Border);
            e.Graphics.DrawRoundedRectangle(pen, new Rectangle(0, 0, panel.ClientSize.Width - 1, panel.ClientSize.Height - 1), 14);
        };
        var header = new Panel { Dock = DockStyle.Top, Height = 34, BackColor = Surface };
        var titleLabel = Label((string.IsNullOrWhiteSpace(icon) ? "" : $"{icon}  ") + title, 10.5f, FontStyle.Bold, Ink);
        titleLabel.Dock = DockStyle.Fill;
        titleLabel.TextAlign = ContentAlignment.MiddleLeft;
        header.Controls.Add(titleLabel);
        body.Dock = DockStyle.Fill;
        body.Margin = Padding.Empty;
        panel.Controls.Add(body);
        panel.Controls.Add(header);
        return panel;
    }

    public static GroupBox Card(string title, Control body)
    {
        var box = new GroupBox
        {
            Text = title,
            Dock = DockStyle.Fill,
            Padding = new Padding(14, 24, 14, 14),
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = Ink,
            BackColor = Surface,
            Margin = new Padding(6)
        };
        body.Dock = DockStyle.Fill;
        body.Margin = Padding.Empty;
        box.Controls.Add(body);
        return box;
    }

    public static Panel Metric(string title, string value, string note, Color accent, string glyph = "◉")
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            MinimumSize = new Size(160, 96),
            BackColor = Surface,
            Margin = new Padding(6),
            Padding = new Padding(18)
        };
        panel.Paint += (_, e) =>
        {
            using var pen = new Pen(Border);
            e.Graphics.DrawRoundedRectangle(pen, new Rectangle(0, 0, panel.ClientSize.Width - 1, panel.ClientSize.Height - 1), 14);
        };
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 3, BackColor = Surface, Margin = Padding.Empty, Padding = Padding.Empty };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 82));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 18));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 28));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 48));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 24));
        var t = Label(title, 9, FontStyle.Regular, Muted); t.Dock = DockStyle.Fill; t.TextAlign = ContentAlignment.MiddleLeft;
        var v = Label(value, 20, FontStyle.Bold, accent); v.Dock = DockStyle.Fill; v.TextAlign = ContentAlignment.MiddleLeft;
        var n = Label(note, 8, FontStyle.Regular, Muted); n.Dock = DockStyle.Fill; n.TextAlign = ContentAlignment.MiddleLeft;
        var icon = Icon(glyph, 12, accent); icon.BackColor = Color.FromArgb(237, 246, 253); icon.Margin = new Padding(3); icon.Dock = DockStyle.Fill;
        layout.Controls.Add(t, 0, 0); layout.SetColumnSpan(t, 2);
        layout.Controls.Add(v, 0, 1); layout.Controls.Add(icon, 1, 1);
        layout.Controls.Add(n, 0, 2); layout.SetColumnSpan(n, 2);
        panel.Controls.Add(layout);
        return panel;
    }

    public static DataGridView Grid()
    {
        var grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = Surface,
            BorderStyle = BorderStyle.None,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            RowHeadersVisible = false,
            Font = new Font("Segoe UI", 9),
            GridColor = Border,
            EnableHeadersVisualStyles = false,
            ColumnHeadersHeight = 34,
            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(247, 249, 252), ForeColor = Muted, Font = new Font("Segoe UI", 8, FontStyle.Bold), WrapMode = DataGridViewTriState.True, Alignment = DataGridViewContentAlignment.MiddleLeft, Padding = new Padding(8, 0, 8, 0) },
            DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.False, Padding = new Padding(8, 7, 8, 7), SelectionBackColor = Color.FromArgb(225, 239, 249), SelectionForeColor = Ink }
        };
        return grid;
    }

    public static string Money(decimal amount) => amount.ToString("C0", CultureInfo.GetCultureInfo("es-AR"));

    private static void DrawRoundedRectangle(this Graphics graphics, Pen pen, Rectangle rectangle, int radius)
    {
        using var path = new GraphicsPath();
        var d = radius * 2;
        path.AddArc(rectangle.X, rectangle.Y, d, d, 180, 90);
        path.AddArc(rectangle.Right - d, rectangle.Y, d, d, 270, 90);
        path.AddArc(rectangle.Right - d, rectangle.Bottom - d, d, d, 0, 90);
        path.AddArc(rectangle.X, rectangle.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        graphics.DrawPath(pen, path);
    }
}
