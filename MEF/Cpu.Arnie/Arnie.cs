using System;
using System.Text;
using System.IO;
using System.Reflection;
using Cpu.Generic;

namespace Cpu.Arnie
{
    public class Arnie:ICpu
    {
        private string _src;
        private bool _isPrint;
        public Arnie(string src)
        {
            _src = src;
            _isPrint = false;

            _pspec = new PortSpec[]
            {
                new PortSpec(0, typeof(string), "Probe"),
            };
            _port = new Port(_pspec);
        }
        public void trace(int traceLevel)
        {
            _isPrint = (traceLevel == 0) ? false : true;
        }
        public override bool step()
        {
            if (_isPrint)
            {
                Console.WriteLine("\tstart: " + _src);
            }
            _src = execOnce(_src);
            if (_isPrint)
            {
                Console.WriteLine("\tend" + _src);
            }
            return true;
        }
        public override bool download(string src)
        {
            _src = src;
            return true;
        }
        private string execOnce(string src)
        {
            if(src == "")
            {
                return src;
            }

            char c = src[0];
            src = src.Substring(1, src.Length - 1);
            switch (c)
            {
                case 'Q':
                    src = Q(src);
                    break;
                case 'C':
                    src = C(execOnce(src));
                    break;
                case 'R':
                    src = R(execOnce(src));
                    break;
                case 'V':
                    src = V(execOnce(src));
                    break;
                case 'P':
                    src = P(execOnce(src));
                    break;
                case 'M':
                    src = M(execOnce(src));
                    break;
                default:
                    src = c + src;
                    break;
            }
            return src;
        }
        private string Q(string src)
        {
            if (_isPrint)
            {
                Console.WriteLine("\tExec <Q> => " + src);
            }
            return src;
        }
        private string C(string src)
        {
            if (_isPrint)
            {
                Console.WriteLine("\tExec <C> => " + src + "Q" + src);
            }
            return src + "Q" + src;
        }
        private string R(string src)
        {
            if (_isPrint)
            {
                Console.WriteLine("\tExec <R> => " + src + src);
            }
            return src + src;
        }
        private string V(string src)
        {
            string ret = "";
            for(; src.Length != 0 ;)
            {
                ret = ret + src[src.Length - 1];
                src = src.Substring(0, src.Length - 1);
            }
            if (_isPrint)
            {
                Console.WriteLine("\tExec <V> => " + ret);
            }
            return ret;
        }
        private string P(string src)
        {
            if (_isPrint)
            {
                Console.WriteLine("\tExec <P> => " + src + V(src));
            }
            return src + V(src);
        }
        private string M(string src)
        {
            if (_isPrint)
            {
                Console.WriteLine("\tExec <M> => " + src.Substring(1, src.Length - 1) + src[0]);
            }
            return src.Substring(1, src.Length - 1) + src[0];
        }
        public void import(string filepath)
        {
            StreamReader sr = new StreamReader(@filepath, Encoding.GetEncoding("UTF-8"));
            string _src = sr.ReadToEnd();
            sr.Close();
            _src = _src.Trim('\n', ' ', '\t', '\r');
        }
        public void export(string filepath)
        {
            StreamWriter sw = new StreamWriter(@filepath, false, Encoding.GetEncoding("UTF-8"));
            sw.Write(_src);
            sw.Close();
        }
        public void printHelp()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            Version ver = asm.GetName().Version;
            Console.WriteLine("\t------------------------------");
            Console.WriteLine("\tarnie machine ver." + ver);
            Console.WriteLine("\t------------------------------");
            Console.WriteLine("\t[usage]");
            Console.WriteLine("\tOneMachine.exe <program string> or <file path>");
            Console.WriteLine("");

            Console.WriteLine("\t[program syntax]");
            Console.WriteLine("\tQx -> x");
            Console.WriteLine("\tCx -> yQy (x -> y)");
            Console.WriteLine("\tRx -> yy (x -> y)");
            Console.WriteLine("\tVx -> inverted y (x -> y)");
            Console.WriteLine("\tPx -> y inverted y (x -> y)");
            Console.WriteLine("\tMx -> roteted y (x -> y)");
            Console.WriteLine("");

            Console.WriteLine("\t[commands]");
            Console.WriteLine("\texec(e) (option)<execute count>: execute program");
            Console.WriteLine("\tinput(i) <program string>: input program(overwrite old program)");
            Console.WriteLine("\tread(r) <filepath>: read program from file(overwrite old program)");
            Console.WriteLine("\twrite(w) <filepath>: write program");
            Console.WriteLine("\tprint(p): toggle print level");
            Console.WriteLine("\tquit(q): quit machine");
            Console.WriteLine("\thelp(h): print this help");
            Console.WriteLine("");

            Console.WriteLine("\t[example]");
            Console.WriteLine("\tQC -> C");
            Console.WriteLine("\tCQC -> CQC");
            Console.WriteLine("\tCCQCC -> CCQCC Q CCQCC");
            Console.WriteLine("\tCCCQCCC -> CCCQCCC Q CCCQCCC Q CCCQCCC Q CCCQCCC");
            Console.WriteLine("\tCQΘC is x -> Θ(x) (Fixed point)");
        }
    }
}
