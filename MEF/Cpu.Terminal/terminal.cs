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

        private void RichTextBox1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if(e.KeyData == Keys.Enter)
            {
                int i = richTextBox1.Lines.Length;
                _stringBuffer += richTextBox1.Lines[i - 1] + '\n';
            }
        }
    }
    public class term : ICpu
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
            
            string s1 = "";
            try/* 受信処理 */
            {
                object o = _port.inP<object>(0);
                s1 = o.ToString();
                _term.sendMsg(s1);
            }
            catch { }

            /* 送信処理 */
            s1 = _term.recvMsg();
            _port.outP(1, s1);

            return true;
        }
    }
}
