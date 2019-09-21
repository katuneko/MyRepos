using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cpu.Generic;

namespace Cpu.Terminal
{
    internal partial class terminal : Form
    {
        string _stringBuffer;
        public terminal()
        {
            InitializeComponent();
        }
        public string recvMsg()
        {
            string ret = _stringBuffer;
            _stringBuffer = "";
            return ret;
        }
        public void sendMsg(string str)
        {
            this.Invoke((MethodInvoker)(() => richTextBox1.AppendText(str)));
        }
        public void removeThread()
        {
            this.Invoke((MethodInvoker)(() => this.Close()));
        }

        private void Terminal_Shown(object sender, EventArgs e)
        {
            this.ActiveControl = null;
        }

        private void Terminal_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void Terminal_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void RichTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            _stringBuffer += e.KeyData;
        }
    }
    public class term : Generic.ICpu
    {
        private string _src;
        internal terminal _term;
        public term(string src)
        {
            _src = src;
            _pspec = new PortSpec[]
            {
                new PortSpec(0, typeof(string), "[IN]Display"),
                new PortSpec(1, typeof(string), "[OUT]Key"),
            };
            _port = new Port(_pspec);
            _term = new terminal();
            var task = MakeThread();
        }
        public override bool dispose()
        {
            _term.removeThread();
            return true;
        }
        private async Task<bool> MakeThread()
        {
            var Loop = Task.Run(() => {
                Application.Run(_term);
            });
            try
            {
                await Loop;
                return true;
            }
            catch
            {
                return false;
            }
        }
        public override bool step()
        {
            string s1 = _port.inP<string>(0);
            if(s1 == null)
            {
                s1 = "";
            }
            _term.sendMsg(s1);
            s1 = _term.recvMsg();
            _port.outP(1, s1);

            return true;
        }
    }
}
