using System.Windows.Forms;
using Newtonsoft.Json;

namespace DofusSwap.Dofus
{
    public class DofusClientData
    {
        public string name { get; set; }
        public string key { get; set; }
        public bool shift { get; set; }
        public bool control { get; set; }
        public bool alt { get; set; }

        [JsonIgnore]
        public Keys KeyBind { get; set; }
    }
}
