using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cpu.Generic;

namespace MEF
{
    class Link
    {
        public GeneratedCpu _inCpu;
        public int _inPortNo;
        public GeneratedCpu _outCpu;
        public int _outPortNo;
        public Link(GeneratedCpu inCpu, int inPortNo, GeneratedCpu outCpu, int outPortNo)
        {
            this._inCpu = inCpu;
            this._inPortNo = inPortNo;
            this._outCpu = outCpu;
            this._outPortNo = outPortNo;
        }
        public void send()
        {
            Port inP = _inCpu.getPort();
            Port outP = _outCpu.getPort();
            try
            {

                if (outP._buf.ContainsKey(_outPortNo))
                {

                    if (inP._buf.ContainsKey(_inPortNo))
                    {
                        inP._buf[_inPortNo] = outP._buf[_outPortNo];
                    }
                    else
                    {
                        inP._buf.Add(_inPortNo, outP._buf[_outPortNo]);
                    }
                }
            }
            catch
            {

            }

        }
    }
}
