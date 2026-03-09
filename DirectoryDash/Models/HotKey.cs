using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DirectoryDash.Models
{
    class HotKey
    {
        public Key Key { get; set; }
        public ModifierKeys Modifier { get; set; }

        internal string GetString()
        {
            return Modifier == null || Modifier == ModifierKeys.None ? Key.ToString() : Modifier.ToString() + " + " + Key.ToString();
        }
    }
}
