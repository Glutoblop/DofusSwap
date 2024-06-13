using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DofusSwap.KeyboardHook
{
    public class KeyboardManager
    {
        //https://gist.github.com/Stasonix/3181083
        //https://stackoverflow.com/questions/10740346/setforegroundwindow-only-working-while-visual-studio-is-open

        // ... { GLOBAL HOOK }
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        const int WH_KEYBOARD_LL = 13;
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;

        private IntPtr _WindowsHookEx = IntPtr.Zero;

        private LowLevelKeyboardProc _KeyboardProc;// = KeyboardHookProc;

        public bool ConsumeAlt { get; set; } = false;

        public void SetHook()
        {
            IntPtr hInstance = LoadLibrary("User32");
            _WindowsHookEx = SetWindowsHookEx(WH_KEYBOARD_LL, _KeyboardProc, hInstance, 0);
        }

        public void UnHook()
        {
            UnhookWindowsHookEx(_WindowsHookEx);
        }

        public IntPtr KeyboardHookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (ConsumeAlt && (wParam == (IntPtr)0x0000000000000104 || wParam == (IntPtr)0x0000000000000101))
            {
#if DEBUG
                Console.WriteLine("Simulated Alt Press Ignored and consumed.");
#endif
                return (IntPtr)1;
            }

            switch (code >= 0)
            {
                case true when wParam == (IntPtr)WM_KEYDOWN:
                    {
                        var key = (Keys)Marshal.ReadInt32(lParam);
                        if (OnKeyPressed?.Invoke(key) ?? false)
                        {
                            return (IntPtr)1;
                        }

                        break;
                    }
                case true when wParam == (IntPtr)WM_KEYUP:
                    {
                        var key = (Keys)Marshal.ReadInt32(lParam);
                        if (OnKeyReleased?.Invoke(key) ?? false)
                        {
                            return (IntPtr)1;
                        }

                        break;
                    }
            }

            return CallNextHookEx(_WindowsHookEx, code, (int)wParam, lParam);
        }

        public event Func<Keys, bool> OnKeyPressed;
        public event Func<Keys, bool> OnKeyReleased;

        public KeyboardManager()
        {
            //Setup triggered windows hook
            _KeyboardProc = KeyboardHookProc;
        }
    }
}
