using System;
using System.Windows.Forms;

namespace DofusSwap.Dofus
{
    [Serializable]
    public class DofusClientData
    {
        public string name { get; set; }
        public string key { get; set; }
        public bool shift { get; set; }
        public bool control { get; set; }

        public Keys KeyBind { get; set; }
    }
}
