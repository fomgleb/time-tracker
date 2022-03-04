using System;
using HooksLibrary;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TimeTracker.BusinessLogic.Model;

namespace TimeTracker.BusinessLogic.Controller
{
    public class HotKeysController
    {
        private const uint HOT_KEYS_COUNT = 2;
        private const string SAVE_FILE_NAME = "HotKeys.dat";

        private List<HotKey> _hotKeys;

        private List<HotKey> HotKeys
        {
            get => _hotKeys;
            set => _hotKeys = value ?? throw new ArgumentNullException(nameof(value), "Hot keys list can't be null.");
        }

        private readonly HooksController _hooksController = new HooksController();

        private bool _hotKeyIsChanging = false;
        private HotKey _changingHotKey = null;

        public HotKeysController()
        {
            LoadHotKeys();

            _hooksController.PressedHookKeysListRemoving += OnPressedHookKeysListRemoving;

            _hooksController.Initialize();
        }

        private void OnPressedHookKeysListRemoving()
        {
            if (_hotKeyIsChanging)
                _changingHotKey.Shortcut = _hooksController.PressedHookKeysList;
        }

        private List<HotKey> LoadHotKeys()
        {
            var formatter = new BinaryFormatter();

            using (var fileStream = new FileStream(SAVE_FILE_NAME, FileMode.OpenOrCreate))
            {
                if (fileStream.Length > 0 && formatter.Deserialize(fileStream) is List<HotKey> hotKeys)
                    return hotKeys;
                return new List<HotKey>();
            }
        }

        public void Save()
        {
            var formatter = new BinaryFormatter();

            using (var fileStream = new FileStream(SAVE_FILE_NAME, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fileStream, HotKeys);
            }
        }

        public void ChangeHotKey(HotKeyEnum hotKey)
        {
            _hotKeyIsChanging = true;
            switch (hotKey)
            {
                case HotKeyEnum.SwitchStopwatch:
                    _changingHotKey = HotKeys[0];
                    break;
                case HotKeyEnum.ShowHideApplication:
                    _changingHotKey = HotKeys[1];
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(hotKey), hotKey, "There is undeclared enum.");
            }
        }

    }

    public enum HotKeyEnum
    {
        SwitchStopwatch,
        ShowHideApplication
    }
}
