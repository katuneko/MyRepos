using System;
using System.Collections.Generic;

namespace Cpu.Generic
{
    public interface ICpu
    {
        bool download(string src);
        bool step();
        bool dispose();
        ref Port getPort();
    }

    public class Port
    {
        public Dictionary<int, Stack<dynamic>> _buf;
        public Port(PortSpec[] spec)
        {
            _buf = new Dictionary<int, Stack<dynamic>>();
            foreach(PortSpec p in spec)
            {
                _buf[p.portNo] = new Stack<dynamic>();
            }
        }
        public void outP<T>(int port, T t)
        {
            if (_buf[port].Count < 100)//todo: kari
            {
                _buf[port].Push(t);
            }
            else
            {
                _buf[port].Pop();
                _buf[port].Push(t);
            }
        }
        public T inP<T>(int port)
        {
            dynamic d = null;
            try
            {
                d = _buf[port].Pop();
                return d;
            }
            catch
            {
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
