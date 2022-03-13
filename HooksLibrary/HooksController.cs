using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HooksLibrary
{
    public class HooksController
    {
        /// <summary>
        /// In seconds.
        /// </summary>
        private const int KEY_REMOVE_COUNTDOWN = 4;

        public event Action PressedKeysChanged;
        public event Action PressedKeysAdded;
        public event Action PressedKeysRemoving;

        private readonly List<Keys> _pressedKeys = new List<Keys>();
        private readonly List<DateTime> _pressTimes = new List<DateTime>();

        /// <summary>
        /// Keys that is pressed now.
        /// </summary>
        public Keys[] PressedKeys => _pressedKeys.ToArray();

        /// <summary>
        /// Create new exemplar and initialize hooks.
        /// </summary>
        public HooksController()
        {
            Initialize();
        }

        ~HooksController()
        {
            Uninitialize();
        }

        private void Initialize()
        {
            KBDHook.KeyDown += OnKeyDown;
            KBDHook.KeyUp += OnKeyUp;
            KBDHook.LocalHook = false;
            KBDHook.InstallHook();
        }

        private void Uninitialize()
        {
            if (KBDHook.IsHookInstalled)
                KBDHook.UninstallHook();
        }

        private void RemoveLongPressedKeys()
        {
            var pressedKeys = _pressedKeys.ToArray();
            var pressTimes = _pressTimes.ToArray();

            var keysToRemove = pressedKeys
                .Where((key, i) => pressTimes[i] <= DateTime.Now.AddSeconds(-KEY_REMOVE_COUNTDOWN))
                .Select(key => key);
            foreach (var key in keysToRemove)
            {
                PressedKeysRemoving?.Invoke();
                _pressTimes.RemoveAt(_pressedKeys.IndexOf(key));
                _pressedKeys.Remove(key);
            }
        }
        
        private void OnKeyDown(LLKHEventArgs e)
        {
            if (_pressedKeys.Contains(e.Keys)) return;

            RemoveLongPressedKeys();
            _pressedKeys.Add(e.Keys);
            _pressTimes.Add(DateTime.Now);
            PressedKeysChanged?.Invoke();
            PressedKeysAdded?.Invoke();
        }

        private void OnKeyUp(LLKHEventArgs e)
        {
            if (_pressedKeys.Count == 0 && _pressTimes.Count == 0) return;

            PressedKeysRemoving?.Invoke();
            _pressTimes.RemoveAt(_pressedKeys.IndexOf(e.Keys));
            _pressedKeys.Remove(e.Keys);
            PressedKeysChanged?.Invoke();
        }
    }
}