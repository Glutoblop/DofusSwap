﻿using System;
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
            switch (code >= 0)
            {
                case true when wParam == (IntPtr)WM_KEYDOWN:
                {
                    var key = (Keys)Marshal.ReadInt32(lParam);
                    OnKeyPressed?.Invoke(key);
                    break;
                }
                case true when wParam == (IntPtr)WM_KEYUP:
                {
                    var key = (Keys)Marshal.ReadInt32(lParam);
                    OnKeyReleased?.Invoke(key);
                    break;
                }
            }

            return CallNextHookEx(_WindowsHookEx, code, (int)wParam, lParam);
        }

        public event Action<Keys> OnKeyPressed;
        public event Action<Keys> OnKeyReleased;
        
        public KeyboardManager()
        {
            //Setup triggered windows hook
            _KeyboardProc = KeyboardHookProc;
        }
    }
}
