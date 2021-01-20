using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DofusSwap.Tray
{
    public class TrayManager
    {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        public static void SetConsoleWindowVisibility(bool visible)
        {
            IntPtr hWnd = FindWindow(null, Console.Title);
            if (hWnd != IntPtr.Zero)
            {
                if (visible) ShowWindow(hWnd, 1); //1 = SW_SHOWNORMAL           
                else ShowWindow(hWnd, 0); //0 = SW_HIDE               
            }
        }

        static NotifyIcon notifyIcon = new NotifyIcon();

        static bool Visible = true;

        public void Init()
        {
            SetConsoleWindowVisibility((Visible = false));

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

        private static void ToggleVisibility(object sender, EventArgs e)
        {
            Visible = !Visible;
            SetConsoleWindowVisibility(Visible);
        }

        public void Stop()
        {
            notifyIcon.Visible = false;
        }
    }
}
