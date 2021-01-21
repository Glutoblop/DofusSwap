using DofusSwap.Dofus;
using DofusSwap.Tray;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DofusSwap
{
    public class DofusForm : Form
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

        private TrayManager _TrayManager;
        private DofusClientManager _DofusClientManager;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel ToolBarLayout;
        private Button ShowConsoleButton;
        private Hotkey[] _FunctionHotKeys;

        private bool _ConsoleVisible;

        public DofusForm()
        {
            _TrayManager = new TrayManager();
            _TrayManager.OnVisbilityToggled += TrayManagerOnOnVisbilityToggled;

            _DofusClientManager = new DofusClientManager();

            _TrayManager.Init();
            _DofusClientManager.Init();

            _FunctionHotKeys = new Hotkey[_DofusClientManager.Clients.Count];
            for (int i = 0; i < _FunctionHotKeys.Length; i++)
            {
                _FunctionHotKeys[i] = new Hotkey(Hotkey.Constants.NOMOD, _DofusClientManager.Clients[i].KeyBind, this);
            }

            KeyPreview = true;
            SetConsoleWindowVisibility(_ConsoleVisible = false);

            Shown += OnShown;
            Closed += OnClosed;

            InitializeComponent();
        }

        private void OnClosed(object sender, EventArgs e)
        {
            _TrayManager?.Stop();
        }

        private void OnShown(object sender, EventArgs e)
        {
            foreach (Hotkey functionHotKey in _FunctionHotKeys)
            {
                functionHotKey.Register();
            }

            Visible = false;
        }

        private void TrayManagerOnOnVisbilityToggled(bool vis)
        {
            Visible = vis;
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

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DofusForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.ToolBarLayout = new System.Windows.Forms.TableLayoutPanel();
            this.ShowConsoleButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.ToolBarLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48.25986F));
            this.tableLayoutPanel1.Controls.Add(this.ToolBarLayout, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 355F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(740, 387);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // ToolBarLayout
            // 
            this.ToolBarLayout.ColumnCount = 2;
            this.ToolBarLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 86.6485F));
            this.ToolBarLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.3515F));
            this.ToolBarLayout.Controls.Add(this.ShowConsoleButton, 1, 0);
            this.ToolBarLayout.Location = new System.Drawing.Point(3, 3);
            this.ToolBarLayout.Name = "ToolBarLayout";
            this.ToolBarLayout.RowCount = 1;
            this.ToolBarLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.ToolBarLayout.Size = new System.Drawing.Size(734, 26);
            this.ToolBarLayout.TabIndex = 0;
            // 
            // ShowConsoleButton
            // 
            this.ShowConsoleButton.Location = new System.Drawing.Point(638, 3);
            this.ShowConsoleButton.Name = "ShowConsoleButton";
            this.ShowConsoleButton.Size = new System.Drawing.Size(93, 20);
            this.ShowConsoleButton.TabIndex = 0;
            this.ShowConsoleButton.Text = "Console";
            this.ShowConsoleButton.UseVisualStyleBackColor = true;
            this.ShowConsoleButton.Click += new System.EventHandler(this.ShowConsoleButton_Click);
            // 
            // DofusForm
            // 
            this.ClientSize = new System.Drawing.Size(764, 411);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DofusForm";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ToolBarLayout.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void ShowConsoleButton_Click(object sender, EventArgs e)
        {
            SetConsoleWindowVisibility((_ConsoleVisible = !_ConsoleVisible));
        }
    }
}
