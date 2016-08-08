using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WeebIRCServerTray
{
    public partial class Debug : MaterialForm
    {
        public Debug()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Blue600, Primary.Blue900, Primary.Blue500, Accent.Blue200, TextShade.WHITE);
        }

        private void Debug_Load(object sender, EventArgs e)
        {

        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
        private const int WM_VSCROLL = 277;
        private const int SB_PAGEBOTTOM = 7;

        public static void ScrollToBottom(RichTextBox MyRichTextBox)
        {
            SendMessage(MyRichTextBox.Handle, WM_VSCROLL, (IntPtr)SB_PAGEBOTTOM, IntPtr.Zero);
        }

        public void AppendToDebug(string debugmessage)
        {

            if (this.DebugWindow.InvokeRequired)
            {
                this.DebugWindow.BeginInvoke(new MethodInvoker(() => AppendToDebug(debugmessage)));
            }
            else
            {
                try
                {
                    this.DebugWindow.AppendText(debugmessage + "\n");
                    ScrollToBottom(DebugWindow);
                } catch (Exception derp)
                {

                }
                
            }
        }

        private void SaveToLog_CheckedChanged(object sender, EventArgs e)
        {
            if (SaveToLog.Checked)
            {
                Home.saveDebug = true;
            } else
            {
                Home.saveDebug = false;
            }
        }

        private void Debug_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }
    }
}
