using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DofusSwap.Properties;

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
            //string filePath = Path.Combine(Application.StartupPath, "Icon", "Swords.png");
            //Bitmap bitmap = new Bitmap(filePath);
            //IntPtr handle = bitmap.GetHicon();
            //notifyIcon.Icon = Icon.FromHandle(handle);
            notifyIcon.Icon = Resources.SwordsIcon;
            notifyIcon.Visible = true;
            notifyIcon.Text = Application.ProductName;

            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Exit", null, (s, e) => { Application.Exit(); });
            notifyIcon.ContextMenuStrip = contextMenu;
        }

        public void VisibilityChanged(bool value)
        {
            _Visible = value;
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
