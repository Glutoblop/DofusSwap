using System;
using System.Windows.Forms;

namespace DofusSwap.Dofus
{
    [Serializable]
    public class DofusClient
    {
        public string name;
        public string key;

        public Keys KeyBind { get; set; }
    }
}
