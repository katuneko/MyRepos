using System;
using System.Collections.Generic;

namespace Cpu.Generic
{
    public interface ICpu
    {
        bool download(string src);
        bool step();
        ref Port getPort();
    }

    public class Port
    {
        public Dictionary<int, dynamic> _buf;
        public Port(PortSpec[] spec)
        {
            _buf = new Dictionary<int, dynamic>();
        }
        public void outP<T>(int port, T t)
        {
            if (_buf.ContainsKey(port))
            {
                _buf[port] = t;
            }
            else
            {
                _buf.Add(port, t);
            }
        }
        public T inP<T>(int port)
        {
            dynamic d = null;
            if (_buf.ContainsKey(port))
            {
                d = _buf[port];
                return d;
            }
            return default(T);
        }
    }
    public struct PortSpec
    {
        public int portNo;
        public Type type;
        public string service;
        public PortSpec(int portNo, Type type, string service)
        {
            this.portNo = portNo;
            this.type = type;
            this.service = service;
        }
    }
}
