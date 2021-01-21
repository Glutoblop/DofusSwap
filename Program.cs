using System;
using System.Windows.Forms;

namespace DofusSwap
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            DofusForm dofusForm = new DofusForm();

            // Standard message loop to catch click-events on notify icon
            // Code after this method will be running only after Application.Exit()
            Application.Run(dofusForm);
        }
    }
}
