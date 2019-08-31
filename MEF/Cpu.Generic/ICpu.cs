using System;

namespace Cpu.Generic
{
    public interface ICpu
    {
        bool input(string src);
        bool step();
        void trace(int traceLevel);
        void printHelp();
    }

    public class Port
    {
        public dynamic _iBuf;
        public dynamic _oBuf;
        public void outP(int port, dynamic c)
        {
            _oBuf = c;
        }
        public ref dynamic inP(int port)
        {
            return ref _iBuf;
        }
    }
}
