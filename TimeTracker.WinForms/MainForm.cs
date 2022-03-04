using System;
using System.Windows.Forms;
using TimeTracker.BusinessLogic.Controller;

namespace TimeTracker.WinForms
{
    public partial class MainForm : Form
    {
        private HotKeysController _hotKeysController = new HotKeysController();
        private TimeInvestmentController _timeInvestmentController = new TimeInvestmentController();

        public MainForm()
        {
            InitializeComponent();
        }

        #region Main Form
        private void MainForm_Load(object sender, EventArgs e)
        {
            
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            
        }
        #endregion

        #region Buttons
        private void ChangeHotKeyButton_MouseClick(object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region ToolStipMenuItem
        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SwitchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tableLayoutPanel4_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
