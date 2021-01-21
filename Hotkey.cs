using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DofusSwap
{
    //Using https://www.dreamincode.net/forums/topic/180436-global-hotkeys/
    public sealed class Hotkey
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private readonly int modifier;
        private readonly int key;
        private readonly IntPtr hWnd;
        private int id;

        public static class Constants
        {
            //modifiers
            public const int NOMOD = 0x0000;
            public const int ALT = 0x0001;
            public const int CTRL = 0x0002;
            public const int SHIFT = 0x0004;
            public const int WIN = 0x0008;

            //windows message id for hotkey
            public const int WM_HOTKEY_MSG_ID = 0x0312;
        }

        public Hotkey(int modifier, Keys key, Form form)
        {
            this.modifier = modifier;
            this.key = (int)key;
            hWnd = form.Handle;
            id = this.GetHashCode();
        }

        public bool Register()
        {
            return RegisterHotKey(hWnd, id, modifier, key);
        }

        public bool Unregister()
        {
            return UnregisterHotKey(hWnd, id);
        }

        #region Overrides of Object

        public override int GetHashCode()
        {
            return modifier ^ key ^ hWnd.ToInt32();
        }

        #endregion
    }
}