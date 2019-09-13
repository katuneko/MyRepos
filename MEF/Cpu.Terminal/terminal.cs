﻿using System;
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
        private delegate void closeDelegate();
        public terminal()
        {
            InitializeComponent();
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
    }
    public class term : Generic.ICpu
    {
        private string _src;
        public Port _port;
        public PortSpec[] _pspec;
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
        public bool dispose()
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
        public bool download(string src)
        {
            return true;
        }
        public bool step()
        {
            //_term.sendMsg("C");
            return true;
        }
        public ref Port getPort()
        {
            return ref _port;
        }
    }
}
