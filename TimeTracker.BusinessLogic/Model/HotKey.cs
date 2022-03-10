using System;
using System.Windows.Forms;

namespace TimeTracker.BusinessLogic.Model
{
    [Serializable]
    public struct HotKey
    {
        private Keys[] _shortcut;

        public Keys[] Shortcut
        {
            get
            {
                Keys[] returningKeys = new Keys[_shortcut.Length];
                for (var i = 0; i < returningKeys.Length; i++)
                    returningKeys[i] = _shortcut[i];
                return returningKeys;
            }
        }

        public HotKeyType HotKeyType { get; }

        public HotKey(Keys[] shortcut, HotKeyType hotKeyType)
        {
            if (shortcut == null)
                throw new ArgumentNullException(nameof(shortcut), "The shortcut can't be null.");

            _shortcut = shortcut;
            HotKeyType = hotKeyType;
        }

        public HotKey SetNewShortcut(Keys[] shortcut)
        {
            return new HotKey(shortcut, HotKeyType);
        }

        public override string ToString()
        {
            var shortcut = "";
            for (var i = 0; i < Shortcut.Length; i++)
            {
                shortcut += Shortcut[i];
                if (Shortcut.Length != i + 1)
                    shortcut += @" + ";
            }
            return shortcut;
        }
    }

    public enum HotKeyType
    {
        ToggleStopwatch,
        ToggleAppDisplay
    }
}