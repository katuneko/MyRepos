using System;
using System.Collections.Generic;

namespace Cpu.Generic
{
    public interface ICpu
    {
        bool download(string src);
        bool step();
        void trace(int traceLevel);
        void printHelp();
    }

    public class Port
    {
        public Dictionary<int, dynamic> _iBuf;
        public Dictionary<int, dynamic> _oBuf;
        public Port(PortSpec[] spec)
        {
            _iBuf = new Dictionary<int, dynamic>();
            _oBuf = new Dictionary<int, dynamic>();
        }
        public void outP(int port, dynamic c)
        {
            if (_oBuf.ContainsKey(port))
            {
                _oBuf[port] = c;
            }
            else
            {
                _oBuf.Add(port, c);
            }
        }
        public dynamic inP(int port)
        {
            dynamic d = null;
            if (_iBuf.ContainsKey(port))
            {
                d = _iBuf[port];
            }
            return d;
        }
    }
    public struct PortSpec
    {
        public int portNo;
        public string service;
        public PortSpec(int portNo, string service)
        {
            this.portNo = portNo;
            this.service = service;
        }
    }
}
