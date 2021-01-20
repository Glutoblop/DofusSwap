using DofusSwap.Dofus;
using DofusSwap.Tray;
using System;
using System.Windows.Forms;

namespace DofusSwap
{
    public class DofusForm : Form
    {
        private TrayManager _TrayManager;
        private DofusClientManager _DofusClientManager;

        private Hotkey[] _FunctionHotKeys;

        public DofusForm()
        {
            _TrayManager = new TrayManager();
            _DofusClientManager = new DofusClientManager();

            _TrayManager.Init();
            _DofusClientManager.Init();

            _FunctionHotKeys = new Hotkey[_DofusClientManager.Clients.Count];
            for (int i = 0; i < _FunctionHotKeys.Length; i++)
            {
                _FunctionHotKeys[i] = new Hotkey(Hotkey.Constants.NOMOD, _DofusClientManager.Clients[i].KeyBind, this);
            }

            KeyPreview = true;

            Shown += delegate (object sender, EventArgs args)
            {
                foreach (Hotkey functionHotKey in _FunctionHotKeys)
                {
                    functionHotKey.Register();
                }

                Visible = false;
            };
        }

        private void HandleHotKey(IntPtr mHWnd, Keys keyPressed)
        {
            _DofusClientManager.OnKeyDown(mHWnd, keyPressed);
        }

        private Keys GetKey(IntPtr LParam)
        {
            return (Keys)((LParam.ToInt32()) >> 16); // not all of the parenthesis are needed, I just found it easier to see what's happening
        }


        #region Overrides of Form

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Hotkey.Constants.WM_HOTKEY_MSG_ID)
            {
                Keys keyPressed = GetKey(m.LParam);
                Console.WriteLine($"[Hot Key Detected] {keyPressed}");
                HandleHotKey(m.HWnd, keyPressed);
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        #endregion

        public void Stop()
        {
            _TrayManager.Stop();
        }
    }
}
