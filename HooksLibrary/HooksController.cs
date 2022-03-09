using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HooksLibrary
{
    public class HooksController
    {
        public event Action PressedKeysChanged;
        public event Action PressedKeysAdded;
        public event Action PressedKeysRemoving;

        public List<Keys> PressedKeys = new List<Keys>();
        public List<string> PressedKeysStringList = new List<string>();

        public void Initialize()
        {
            KBDHook.KeyDown += OnKeyDown;
            KBDHook.KeyUp += OnKeyUp;
            KBDHook.LocalHook = false;
            KBDHook.InstallHook();
        }

        public void Uninitialize()
        {
            if (KBDHook.IsHookInstalled)
                KBDHook.UninstallHook();
        }

        private void OnKeyDown(LLKHEventArgs e)
        {
            if (PressedKeys.Contains(e.Keys)) return;
            PressedKeys.Add(e.Keys);
            PressedKeysStringList.Add(e.Keys.ToString());
            PressedKeysChanged?.Invoke();
            PressedKeysAdded?.Invoke();
        }

        private void OnKeyUp(LLKHEventArgs e)
        {
            PressedKeysRemoving?.Invoke();
            PressedKeys.Remove(e.Keys);
            PressedKeysStringList.Remove(e.Keys.ToString());
            PressedKeysChanged?.Invoke();
        }
    }
}