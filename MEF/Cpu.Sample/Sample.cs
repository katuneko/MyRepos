using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cpu.Generic;

namespace Cpu.Sample
{
    public class Sample:ICpu
    {
        public Port _port;
        public PortSpec[] _pspec;
        public Sample(string arg)
        {
            _pspec = new PortSpec[]
            {
                new PortSpec(0, typeof(string), "[IN]Probe"),
                new PortSpec(1, typeof(bool),   "ON"),
                new PortSpec(2, typeof(bool),   "OFF"),
                new PortSpec(3, typeof(bool),   "And(in1)"),
                new PortSpec(4, typeof(bool),   "And(in2)"),
                new PortSpec(5, typeof(bool),   "And(out)"),
                new PortSpec(6, typeof(bool),   "Not(in)"),
                new PortSpec(7, typeof(bool),   "Not(out)"),
            };
            _port = new Port(_pspec);
        }
        public bool download(string src)
        {
            return true;
        }
        public bool step()
        {
            _port.outP(0, "test");
            _port.outP(1, true);
            _port.outP(2, false);
            bool b1 = _port.inP<bool>(3);
            bool b2 = _port.inP<bool>(4);
            _port.outP(5, b1 & b2);
            bool b3 = _port.inP<bool>(6);
            _port.outP(7, !b3);
            return true;
        }
        public ref Port getPort()
        {
            return ref _port;
        }
    }
}
