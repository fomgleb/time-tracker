using HooksLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using TimeTracker.BusinessLogic.Model;

namespace TimeTracker.BusinessLogic.Controller
{
    public class HotKeysController
    {
        private const int HOT_KEYS_COUNT = 2;
        private const string SAVE_FILE_NAME = "HotKeys.dat";

        private static readonly List<HotKey> DefaultHotKeys = new List<HotKey>
        {
            new HotKey(new List<Keys> {Keys.LShiftKey, Keys.RShiftKey}, HotKeyType.ToggleStopwatch),
            new HotKey(new List<Keys> {Keys.LControlKey, Keys.F7}, HotKeyType.ToggleAppDisplay)
        };

        public event Action<HotKeyType> HotKeyPressed;
        public event Action<HotKeyType> HotKeyChanged;

        private List<HotKey> _hotKeys;

        private List<HotKey> HotKeys
        {
            get => _hotKeys;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value), "Hot keys list can't be null.");

                _hotKeys = value.Count == HOT_KEYS_COUNT ? value : DefaultHotKeys;
            }
        }

        private readonly HooksController _hooksController = new HooksController();

        /// <summary>
        /// -1 means that no hot key is changing.
        /// </summary>
        private int _changingHotKeyIndex = -1;

        public HotKeysController()
        {
            LoadHotKeys();
            
            _hooksController.PressedKeysRemoving += OnPressedKeysRemoving;
            _hooksController.PressedKeysChanged += OnPressedKeysChanged;

            _hooksController.Initialize();
        }

        public void UninitializeHookKeys()
        {
            _hooksController.Uninitialize();
        }

        private void OnPressedKeysRemoving()
        {
            if (_changingHotKeyIndex == -1) return;

            HotKeys[_changingHotKeyIndex].Shortcut.Clear();
            foreach (var pressedHookKey in _hooksController.PressedKeys)
                HotKeys[_changingHotKeyIndex].Shortcut.Add(pressedHookKey);

            HotKeyChanged?.Invoke(HotKeys[_changingHotKeyIndex].HotKeyType);
            _changingHotKeyIndex = -1;
            SaveHotKeys();
        }

        private void OnPressedKeysChanged()
        {
            var pressedKeys = new Keys[_hooksController.PressedKeys.Count];
            _hooksController.PressedKeys.CopyTo(pressedKeys);

            foreach (var hotKey in HotKeys)
            {
                if (hotKey.Shortcut.Count != pressedKeys.Length) continue;

                var currentHotKeyIsPressed = true;
                for (var i = 0; i < pressedKeys.Length; i++)
                    if (hotKey.Shortcut[i] != pressedKeys[i])
                    {
                        currentHotKeyIsPressed = false;
                        break;
                    }

                if (currentHotKeyIsPressed)
                    HotKeyPressed?.Invoke(hotKey.HotKeyType);
            }
        }

        /// <summary>
        /// Load hot keys or empty list.
        /// </summary>
        /// <returns> Loaded or default hot keys. </returns>
        private void LoadHotKeys()
        {
            var formatter = new BinaryFormatter();

            using (var fileStream = new FileStream(SAVE_FILE_NAME, FileMode.OpenOrCreate))
            {
                if (fileStream.Length > 0 && formatter.Deserialize(fileStream) is List<HotKey> hotKeys)
                    HotKeys = hotKeys;
                else
                    HotKeys = new List<HotKey>();
            }
        }

        public void SaveHotKeys()
        {
            var formatter = new BinaryFormatter();

            using (var fileStream = new FileStream(SAVE_FILE_NAME, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fileStream, HotKeys);
            }
        }

        public void StartChangingHotKey(HotKeyType hotKeyType)
        {
            _changingHotKeyIndex = HotKeys.FindIndex(h => h.HotKeyType == hotKeyType);
        }

        public string GetHotKeyString(HotKeyType hotKeyType)
        {
            return HotKeys.SingleOrDefault(h => h.HotKeyType == hotKeyType)?.ToString();
        }
    }

    public enum HotKeyType
    {
        ToggleStopwatch,
        ToggleAppDisplay
    }
}
