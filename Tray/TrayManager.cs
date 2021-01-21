using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DofusSwap.Tray
{
    public class TrayManager
    {
        static NotifyIcon notifyIcon = new NotifyIcon();

        public event Action<bool> OnVisbilityToggled;

        private bool _Visible;

        public void Init()
        {
            _Visible = false;
            OnVisbilityToggled?.Invoke(_Visible);

            notifyIcon.Click += ToggleVisibility;
            string filePath = Path.Combine(Application.StartupPath, "Icon", "Swords.png");
            Bitmap bitmap = new Bitmap(filePath);
            IntPtr handle = bitmap.GetHicon();
            notifyIcon.Icon = Icon.FromHandle(handle);
            notifyIcon.Visible = true;
            notifyIcon.Text = Application.ProductName;

            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Exit", null, (s, e) => { Application.Exit(); });
            notifyIcon.ContextMenuStrip = contextMenu;
        }

        private void ToggleVisibility(object sender, EventArgs e)
        {
            _Visible = !_Visible;
            OnVisbilityToggled?.Invoke(_Visible);
        }

        public void Stop()
        {
            notifyIcon.Visible = false;
        }
    }
}
