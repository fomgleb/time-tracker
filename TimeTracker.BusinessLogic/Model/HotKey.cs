using System;
using System.Collections.Generic;
using System.Windows.Forms;

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

        public HotKey(List<Keys> shortcut)
        {
            Shortcut = shortcut;
        }
    }
}