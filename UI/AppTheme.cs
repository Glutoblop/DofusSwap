using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;

namespace DofusSwap.UI
{
    public static class AppTheme
    {
        public static readonly Color Background = Color.FromArgb(30, 30, 46);
        public static readonly Color Surface = Color.FromArgb(45, 45, 63);
        public static readonly Color SurfaceLight = Color.FromArgb(55, 55, 75);
        public static readonly Color Accent = Color.FromArgb(212, 168, 67);
        public static readonly Color AccentHover = Color.FromArgb(232, 189, 90);
        public static readonly Color Text = Color.FromArgb(205, 214, 244);
        public static readonly Color TextMuted = Color.FromArgb(127, 132, 156);
        public static readonly Color Border = Color.FromArgb(61, 61, 80);
        public static readonly Color ButtonBg = Color.FromArgb(53, 53, 72);
        public static readonly Color ButtonHover = Color.FromArgb(69, 69, 96);
        public static readonly Color Danger = Color.FromArgb(224, 85, 85);
        public static readonly Color DangerHover = Color.FromArgb(240, 110, 110);
        public static readonly Color InputBg = Color.FromArgb(36, 36, 54);

        public static void Apply(Form form)
        {
            form.BackColor = Background;
            form.ForeColor = Text;
            ApplyToControls(form.Controls);
        }

        public static void ApplyToControl(Control control)
        {
            if (control is Button btn)
                StyleButton(btn);
            else if (control is Panel panel)
                StylePanel(panel);
            else if (control is RichTextBox rtb)
                StyleRichTextBox(rtb);
            else if (control is TextBox tb)
                StyleTextBox(tb);
            else if (control is CheckBox chk)
                StyleCheckBox(chk);
            else if (control is MenuStrip ms)
                StyleMenuStrip(ms);
            else if (control is Label lbl)
            {
                lbl.ForeColor = Text;
                lbl.BackColor = Color.Transparent;
            }

            if (control.Controls.Count > 0)
                ApplyToControls(control.Controls);
        }

        private static void ApplyToControls(Control.ControlCollection controls)
        {
            foreach (Control c in controls)
                ApplyToControl(c);
        }

        public static void StyleButton(Button btn, bool danger = false)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = danger ? Danger : Border;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.MouseOverBackColor = danger ? DangerHover : ButtonHover;
            btn.FlatAppearance.MouseDownBackColor = danger ? Danger : Accent;
            btn.BackColor = danger ? Color.FromArgb(60, 40, 40) : ButtonBg;
            btn.ForeColor = danger ? Danger : Text;
            btn.Cursor = Cursors.Hand;
            btn.Font = new Font("Segoe UI", btn.Font.Size, btn.Font.Style);
        }

        public static void StylePanel(Panel panel)
        {
            panel.BackColor = Surface;
        }

        public static void StyleRichTextBox(RichTextBox rtb)
        {
            rtb.BackColor = InputBg;
            rtb.ForeColor = Text;
            rtb.BorderStyle = BorderStyle.None;
            rtb.Font = new Font("Segoe UI", 9.5f);
        }

        public static void StyleTextBox(TextBox tb)
        {
            tb.BackColor = InputBg;
            tb.ForeColor = Text;
            tb.BorderStyle = BorderStyle.FixedSingle;
        }

        public static void StyleCheckBox(CheckBox chk)
        {
            chk.ForeColor = TextMuted;
            chk.Font = new Font("Segoe UI", chk.Font.Size);
        }

        public static void StyleMenuStrip(MenuStrip ms)
        {
            ms.BackColor = Background;
            ms.ForeColor = Text;
            ms.Renderer = new DarkMenuRenderer();

            foreach (ToolStripItem item in ms.Items)
            {
                item.ForeColor = Text;
                if (item is ToolStripMenuItem tsi)
                {
                    foreach (ToolStripItem sub in tsi.DropDownItems)
                        sub.ForeColor = Text;
                }
            }
        }

        public static void EnableDoubleBuffering(Control control)
        {
            typeof(Control).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, control, new object[] { true });
        }
    }

    internal class DarkMenuRenderer : ToolStripProfessionalRenderer
    {
        public DarkMenuRenderer() : base(new DarkColorTable()) { }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = AppTheme.Text;
            base.OnRenderItemText(e);
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            var rect = new Rectangle(Point.Empty, e.Item.Size);
            using (var brush = new SolidBrush(e.Item.Selected ? AppTheme.ButtonHover : AppTheme.Background))
                e.Graphics.FillRectangle(brush, rect);
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            using (var brush = new SolidBrush(AppTheme.Background))
                e.Graphics.FillRectangle(brush, e.AffectedBounds);
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            using (var pen = new Pen(AppTheme.Border))
                e.Graphics.DrawLine(pen, 0, e.Item.Height / 2, e.Item.Width, e.Item.Height / 2);
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            using (var pen = new Pen(AppTheme.Border))
                e.Graphics.DrawRectangle(pen, 0, 0, e.AffectedBounds.Width - 1, e.AffectedBounds.Height - 1);
        }
    }

    internal class DarkColorTable : ProfessionalColorTable
    {
        public override Color MenuItemSelected => AppTheme.ButtonHover;
        public override Color MenuItemBorder => AppTheme.Border;
        public override Color MenuBorder => AppTheme.Border;
        public override Color MenuStripGradientBegin => AppTheme.Background;
        public override Color MenuStripGradientEnd => AppTheme.Background;
        public override Color MenuItemSelectedGradientBegin => AppTheme.ButtonHover;
        public override Color MenuItemSelectedGradientEnd => AppTheme.ButtonHover;
        public override Color MenuItemPressedGradientBegin => AppTheme.Accent;
        public override Color MenuItemPressedGradientEnd => AppTheme.Accent;
        public override Color ImageMarginGradientBegin => AppTheme.Surface;
        public override Color ImageMarginGradientMiddle => AppTheme.Surface;
        public override Color ImageMarginGradientEnd => AppTheme.Surface;
        public override Color ToolStripDropDownBackground => AppTheme.Surface;
        public override Color SeparatorDark => AppTheme.Border;
        public override Color SeparatorLight => AppTheme.Border;
    }
}
