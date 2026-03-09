using DirectoryDash.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace DirectoryDash.Services
{
    internal class KeyService
    {
        internal HotKey DetectGesture(InputEventArgs e)
        {
            if (e is not System.Windows.Input.KeyEventArgs args) return null;

            Key key = args.Key == Key.System ? args.SystemKey : args.Key;
            ModifierKeys mods = Keyboard.Modifiers;

            // Allow modifier-only keys (Ctrl, Shift, Alt)
            if (key == Key.LeftCtrl || key == Key.RightCtrl)
                mods &= ~ModifierKeys.Control;
            if (key == Key.LeftShift || key == Key.RightShift)
                mods &= ~ModifierKeys.Shift;
            if (key == Key.LeftAlt || key == Key.RightAlt)
                mods &= ~ModifierKeys.Alt;

            return new HotKey() { Key = key, Modifier = mods };
        }

        internal bool MatchGesture(System.Windows.Input.KeyEventArgs args, HotKey hotKey)
        {
            if (args.Key == hotKey.Key &&
                (hotKey.Modifier == ModifierKeys.None || Keyboard.Modifiers == hotKey.Modifier))
                return true;

            return false;
        }
    }
}
