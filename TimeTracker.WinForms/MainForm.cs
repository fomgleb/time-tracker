using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TimeTracker.BusinessLogic.Controller;
using TimeTracker.BusinessLogic.Extensions;
using TimeTracker.BusinessLogic.Model;
using TimeTracker.WinForms.Properties;

namespace TimeTracker.WinForms
{
    public partial class MainForm : Form
    {
        private readonly HotKeysController _hotKeysController = new HotKeysController();
        private readonly TimeInvestmentController _timeInvestmentController = new TimeInvestmentController();

        private readonly Dictionary<Button, HotKeyType> _changeHotKeysButtons;

        private DateTime _selectedDate = DateTime.Today;

        public MainForm()
        {   
            InitializeComponent();

            _changeHotKeysButtons = new Dictionary<Button, HotKeyType>(HotKeysController.HotKeysCount)
            {
                {changeToggleAppDisplayHotKeyButton, HotKeyType.ToggleAppDisplay},
                {changeToggleStopwatchHotKeyButton, HotKeyType.ToggleStopwatch}
            };

            _hotKeysController.HotKeyChanged += OnHotKeyChanged;
            _hotKeysController.HotKeyPressed += OnHotKeyPressed;

            UpdateLabelsTexts();
            UpdateButtonText(_hotKeysController.HotKeys[0]);
            UpdateButtonText(_hotKeysController.HotKeys[1]);
            UpdateCalendar();
            UpdateTextBoxesTexts();
            monthCalendar.SelectionStart = DateTime.Today;
        }

        #region Events
        private void OnHotKeyPressed(HotKeyType hotKeyType)
        {
            switch (hotKeyType)
            {
                case HotKeyType.ToggleStopwatch:
                    ToggleSpendingTime();
                    break;
                case HotKeyType.ToggleAppDisplay:
                    ToggleAppDisplay();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(hotKeyType), hotKeyType, @"Not implemented enum.");
            }
        }
        #endregion

        #region Start or stop spending time 
        private void StartSpendingTime()
        {
            _timeInvestmentController.StartSpendingTime();
            notifyIcon.Icon = Resources.pause;
            startSpendingTimePictureBox.Image = Resources.pause1;
            switchToolStripMenuItem.Text = @"Stop";
            if (WindowState == FormWindowState.Normal)
                updateLabelsTextsTimer.Start();
        }

        private void StopSpendingTime()
        {
            _timeInvestmentController.StopSpendingTime();
            _timeInvestmentController.Save();
            notifyIcon.Icon = Resources.play;
            startSpendingTimePictureBox.Image = Resources.play1;
            switchToolStripMenuItem.Text = @"Start";
            updateLabelsTextsTimer.Stop();
        }

        private void ToggleSpendingTime()
        {
            if (_timeInvestmentController.StopwatchIsRunning)
                StopSpendingTime();
            else
                StartSpendingTime();
        }

        private void SwitchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleSpendingTime();
        }
        #endregion

        #region Show or hide application
        private void ShowApp()
        {

            if (_timeInvestmentController.StopwatchIsRunning)
                updateLabelsTextsTimer.Start();
            UpdateLabelsTexts();
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            UpdateCalendar();
            monthCalendar.SelectionStart = DateTime.Today;
            Show();
            Activate();
            Focus();
        }

        private void HideApp()
        {
            updateLabelsTextsTimer.Stop();
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
            Hide();
        }

        private void ToggleAppDisplay()
        {
            if (WindowState == FormWindowState.Normal)
                HideApp();
            else if (WindowState == FormWindowState.Minimized)
                ShowApp();
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                ToggleAppDisplay();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                HideApp();
        }
        #endregion

        #region Change hot keys
        private void ChangeHotKeyButtons_Click(object sender, EventArgs e)
        {
            if (!(sender is Button button))
                throw new ArgumentNullException(nameof(sender), @"Senders can only be buttons.");
            if (!_changeHotKeysButtons.ContainsKey(button))
                throw new ArgumentOutOfRangeException(nameof(sender), button,
                    $@"Senders can only be buttons that change hot keys. Or {nameof(_changeHotKeysButtons)} doesn't contain all buttons that changes hot keys.");

            var pair = _changeHotKeysButtons.SingleOrDefault(p => p.Key == button);
            _hotKeysController.StartChangingHotKey(pair.Value);
            pair.Key.Text = @"Enter shortcut...";
        }
        #endregion

        #region Updates
        private void OnHotKeyChanged(HotKey hotKey)
        {
            UpdateButtonText(hotKey);
        }

        private void UpdateLabelsTexts()
        {
            investedTimeForDayLabel.Text = _timeInvestmentController.GetInvestedTimeForDay(monthCalendar.SelectionStart).ToStringWithoutDays();
            investedTimeForWeekLabel.Text = _timeInvestmentController.GetInvestedTimeForWeek(monthCalendar.SelectionStart).ToStringWithoutDays();
            investedTimeInMonthLabel.Text = _timeInvestmentController.GetInvestedTimeForMonth(monthCalendar.SelectionStart).ToStringWithoutDays();
            investedTimeInRangeLabel.Text = _timeInvestmentController.GetInvestedTimeByDateRange(monthCalendar.SelectionStart, monthCalendar.SelectionEnd).ToStringWithoutDays();
        }

        private void UpdateButtonText(HotKey hotKey)
        {
            var changingButton = _changeHotKeysButtons.SingleOrDefault(b => b.Value == hotKey.HotKeyType).Key;
            changingButton.Text = _hotKeysController.HotKeys.SingleOrDefault(h => h.HotKeyType == hotKey.HotKeyType).ToString();
        }

        private void UpdateTextBoxesTexts()
        {
            descriptionTextBox.Text = _timeInvestmentController.GetTimeInvestmentByDate(monthCalendar.SelectionStart).Description;
        }

        private void UpdateLabelsTextsTimer_Tick(object sender, EventArgs e)
        {
            UpdateLabelsTexts();
        }

        private void UpdateCalendar()
        {
            monthCalendar.MaxDate = DateTime.Today;
            monthCalendar.MinDate = _timeInvestmentController.TimeInvestments.Count > 0
                ? _timeInvestmentController.TimeInvestments[0].Date
                : DateTime.Today.AddDays(-1);
        }

        private void MonthCalendar_DateChanged(object sender, DateRangeEventArgs e)
        {
            _timeInvestmentController.SetDescription(descriptionTextBox.Text, _selectedDate);
            UpdateLabelsTexts();
            UpdateTextBoxesTexts();
            _selectedDate = monthCalendar.SelectionStart;
        }
        #endregion

        #region Form closing
        private void ClosePreparations()
        {
            _timeInvestmentController.SetDescription(descriptionTextBox.Text, _selectedDate);
            if (_timeInvestmentController.StopwatchIsRunning)
                StopSpendingTime();
            _timeInvestmentController.Save();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ClosePreparations();
        }

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClosePreparations();
            Close();
        }
        #endregion
    }
}
