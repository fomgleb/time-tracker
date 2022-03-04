using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HooksLibrary
{
    public class HooksController
    {
        public event Action PressedHookKeysListChanged;
        public event Action PressedHookKeysListRemoving;

        public List<Keys> PressedHookKeysList = new List<Keys>();
        public List<string> PressedHookKeysStringList = new List<string>();

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
            if (PressedHookKeysList.Contains(e.Keys)) return;
            PressedHookKeysList.Add(e.Keys);
            PressedHookKeysStringList.Add(e.Keys.ToString());
            PressedHookKeysListChanged?.Invoke();
        }

        private void OnKeyUp(LLKHEventArgs e)
        {
            PressedHookKeysListRemoving?.Invoke();
            PressedHookKeysList.Remove(e.Keys);
            PressedHookKeysStringList.Remove(e.Keys.ToString());
            PressedHookKeysListChanged?.Invoke();
        }
    }
}