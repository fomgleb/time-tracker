using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TimeTracker.BusinessLogic.Controller;

namespace TimeTracker.BusinessLogic.Model
{
    [Serializable]
    public class HotKey
    {
        private List<Keys> _shortcut;

        public List<Keys> Shortcut
        {
            get => _shortcut;
            set => _shortcut = value ?? throw new ArgumentNullException(nameof(value), "The shortcut can't be null.");
        }

        public HotKeyType HotKeyType { get; private set; }

        public HotKey(List<Keys> shortcut, HotKeyType hotKeyType)
        {
            Shortcut = shortcut;
            HotKeyType = hotKeyType;
        }

        public override string ToString()
        {
            var shortcut = "";
            for (var i = 0; i < Shortcut.Count; i++)
            {
                shortcut += Shortcut[i];
                if (Shortcut.Count != i + 1)
                    shortcut += @" + ";
            }
            return shortcut;
        }
    }
}