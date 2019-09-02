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
        ref Port getPort();
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
        public void outP<T>(int port, T t)
        {
            if (_oBuf.ContainsKey(port))
            {
                _oBuf[port] = t;
            }
            else
            {
                _oBuf.Add(port, t);
            }
        }
        public T inP<T>(int port)
        {
            dynamic d = null;
            if (_iBuf.ContainsKey(port))
            {
                d = _iBuf[port];
                return d;
            }
            return default(T);
        }
    }
    public struct PortSpec
    {
        public int portNo;
        public Type inType;
        public Type outType;
        public string inService;
        public string outService;
        public PortSpec(int portNo, Type inType, Type outType, string inService, string outService)
        {
            this.portNo = portNo;
            this.inType = inType;
            this.outType = outType;
            this.inService = inService;
            this.outService = outService;
        }
    }
}
