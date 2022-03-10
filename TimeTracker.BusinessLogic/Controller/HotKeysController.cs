using HooksLibrary;
using System;
using System.Linq;
using System.Windows.Forms;
using TimeTracker.BusinessLogic.Model;

namespace TimeTracker.BusinessLogic.Controller
{
    public class HotKeysController : ControllerBase
    {
        public const int HotKeysCount = 2;
        private const string SAVE_FILE_NAME = "HotKeys.dat";

        private static readonly HotKey[] DefaultHotKeys = new HotKey[HotKeysCount]
        {
            new HotKey(new[] {Keys.LShiftKey, Keys.RShiftKey}, HotKeyType.ToggleStopwatch),
            new HotKey(new[] {Keys.LControlKey, Keys.F7}, HotKeyType.ToggleAppDisplay)
        };

        public event Action<HotKeyType> HotKeyPressed;
        public event Action<HotKey> HotKeyChanged;

        private readonly HotKey[] _hotKeys;

        public HotKey[] HotKeys
        {
            get
            {
                var returningHotKey = new HotKey[_hotKeys.Length];
                for (int i = 0; i < returningHotKey.Length; i++)
                    returningHotKey[i] = _hotKeys[i];
                return returningHotKey;
            }
        }

        private readonly HooksController _hooksController = new HooksController();

        /// <summary>
        /// -1 means that no hot key is changing.
        /// </summary>
        private int _changingHotKeyIndex = -1;

        /// <summary>
        /// Load hot keys and initialize hook keys controller (HOOK KEYS CONTROLLER MUST BE REMOVED AT THE END OF APPLICATION USING UninitializeHookKeys()).
        /// </summary>
        public HotKeysController()
        {
            _hotKeys = Load<HotKey[]>(SAVE_FILE_NAME) ?? DefaultHotKeys;

            _hooksController.PressedKeysRemoving += OnPressedKeysRemoving;
            _hooksController.PressedKeysChanged += OnPressedKeysChanged;
            _hooksController.Initialize();
        }

        /// <summary>
        /// Must be called at the end of the application.  
        /// </summary>
        public void UninitializeHookKeys()
        {
            _hooksController.Uninitialize();
        }

        #region When hot key is pressed
        private void OnPressedKeysChanged()
        {
            foreach (var hotKey in _hotKeys.Where(HotKeyIsPressed))
                HotKeyPressed?.Invoke(hotKey.HotKeyType);
        }

        private bool HotKeyIsPressed(HotKey hotKey)
        {
            var pressedKeys = new Keys[_hooksController.PressedKeys.Count];
            _hooksController.PressedKeys.CopyTo(pressedKeys);

            if (hotKey.Shortcut.Length != pressedKeys.Length) return false;

            var hotKeyIsPressed = true;
            for (var i = 0; i < pressedKeys.Length; i++)
                if (hotKey.Shortcut[i] != pressedKeys[i])
                {
                    hotKeyIsPressed = false;
                    break;
                }

            return hotKeyIsPressed;
        }
        #endregion

        #region Change hot key
        private void OnPressedKeysRemoving()
        {
            StopChangingHotKey();
        }

        public void StartChangingHotKey(HotKeyType hotKeyType)
        {
            for (var i = 0; i < HotKeysCount; i++)
                if (_hotKeys[i].HotKeyType == hotKeyType)
                    _changingHotKeyIndex = i;
        }

        private void StopChangingHotKey()
        {
            if (_changingHotKeyIndex == -1) return;

            var newShortcut = new Keys[_hooksController.PressedKeys.Count];
            for (var i = 0; i < newShortcut.Length; i++)
                newShortcut[i] = _hooksController.PressedKeys[i];

            _hotKeys[_changingHotKeyIndex] = _hotKeys[_changingHotKeyIndex].SetNewShortcut(newShortcut);

            HotKeyChanged?.Invoke(_hotKeys[_changingHotKeyIndex]);
            _changingHotKeyIndex = -1;

            Save(SAVE_FILE_NAME, _hotKeys);
        }   
        #endregion
    }
}
