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
                new PortSpec(0, "test service A"),
                new PortSpec(1, "test service B"),
                new PortSpec(2, "test service C"),
            };
            _port = new Port(_pspec);
        }
        public bool download(string src)
        {
            return true;
        }
        public bool step()
        {
            string s;
            _port.outP(0, "test");
            s = _port.inP(1);
            _port.outP(2, s);
            s = _port.inP(2);
            _port.outP(1, s);
            return true;
        }
        public void trace(int traceLevel)
        {

        }
        public void printHelp()
        {

        }
    }
}
