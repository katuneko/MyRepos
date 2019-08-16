using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace OneMachine
{
    class Arnie
    {
        private string _src;
        private bool _isPrint;
        public Arnie(string src)
        {
            _src = src;
            _isPrint = false;
        }
        public void togglePrint()
        {
            _isPrint = !_isPrint;
        }
        public void exec(int cnt)
        {
            Console.WriteLine("start: " + _src);
            for (int i = 0; i < cnt; i++)
            {
                _src = execOnce(_src);
                Console.WriteLine("[" + i + "]: " + _src);
            }
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
                Console.WriteLine("Exec <Q> => " + src);
            }
            return src;
        }
        private string C(string src)
        {
            if (_isPrint)
            {
                Console.WriteLine("Exec <C> => " + src + "C" + src);
            }
            return src + "C" + src;
        }
        private string R(string src)
        {
            if (_isPrint)
            {
                Console.WriteLine("Exec <R> => " + src + src);
            }
            return src + src;
        }
        private string V(string src)
        {
            string ret = "";
            for(int i = 0; src.Length != 0 ;)
            {
                ret = ret + src[src.Length - 1];
                src = src.Substring(0, src.Length - 1);
            }
            if (_isPrint)
            {
                Console.WriteLine("Exec <V> => " + ret);
            }
            return ret;
        }
        private string P(string src)
        {
            if (_isPrint)
            {
                Console.WriteLine("Exec <P> => " + src + V(src));
            }
            return src + V(src);
        }
        private string M(string src)
        {
            if (_isPrint)
            {
                Console.WriteLine("Exec <M> => " + src.Substring(1, src.Length - 1) + src[0]);
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
        public void input(string src)
        {
            _src = src;
        }
    }
}
