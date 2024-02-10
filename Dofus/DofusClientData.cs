using System;
using System.Windows.Forms;

namespace DofusSwap.Dofus
{
    [Serializable]
    public class DofusClientData
    {
        public string name;
        public string key;
        public bool shift;
        public bool control;

        public Keys KeyBind { get; set; }
    }
}
