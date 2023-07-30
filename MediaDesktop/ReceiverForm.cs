using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MediaDesktop
{
    public partial class ReceiverForm : Form
    {
        public static ReceiverForm Instance { get; private set; } = new ReceiverForm();
        public event EventHandler ScreenSolutionChanged;

        private ReceiverForm()
        {
            Instance = this;
            InitializeComponent();
        }

        protected override void DefWndProc(ref Message m)
        {
            const int WM_SOLUTIONCHANGED = 0x007e;
            if (m.Msg == WM_SOLUTIONCHANGED)
                ScreenSolutionChanged?.Invoke(this, EventArgs.Empty);
            base.DefWndProc(ref m);
        }
    }
}
