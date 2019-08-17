using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace OneMachine
{
    class Program
    {
        private enum Mode
        {
            Run,Stop
        }
        private enum Commands
        {
            Invalid, Exit, Exec, Input, Import, Export, TogglePrint, HelpPrint
        }
        private static Mode _mode = Mode.Run;
        private static Commands _cmd = Commands.Invalid;
        private static Arnie _arnie;
        static void Main(string[] args)
        {
            string s = "";
            _arnie = new Arnie(s);
            if (0 < args.Length)
            {
                if (File.Exists(args[0]))
                {
                    _arnie.import(args[0]);
                }
                else
                {
                    _arnie.input(args[0]);
                }
            }

            while(_mode != Mode.Stop)
            {
                Console.Write("cmd> ");
                string input = Console.ReadLine();
                string[] cmdarg = input.Split(' ');
                commandAlias(cmdarg[0]);
                if(1 < cmdarg.Length)
                {
                    commandExecute(cmdarg[1]);
                }
                else
                {
                    commandExecute("");
                }
            }
            Console.WriteLine("bye.");
        }
        private static void commandAlias(string cmdstr)
        {
            switch (cmdstr)
            {
                case "exit":
                case "quit":
                case "q":
                    _cmd = Commands.Exit;
                    break;
                case "exec":
                case "e":
                case "do":
                    _cmd = Commands.Exec;
                    break;
                case "import":
                case "in":
                case "read":
                case "r":
                    _cmd = Commands.Import;
                    break;
                case "input":
                case "i":
                    _cmd = Commands.Input;
                    break;
                case "export":
                case "ex":
                case "write":
                case "w":
                    _cmd = Commands.Export;
                    break;
                case "print":
                case "p":
                case "toggleprint":
                case "t":
                    _cmd = Commands.TogglePrint;
                    break;
                case "help":
                case "h":
                    _cmd = Commands.HelpPrint;
                    break;
                default:
                    _cmd = Commands.Invalid;
                    break;
            }
        }
        private static void commandExecute(string cmdstr)
        {
            switch (_cmd)
            {
                case Commands.Invalid:
                    Console.WriteLine("invalid command. type help or h.");
                    break;
                case Commands.Exit:
                    _mode = Mode.Stop;
                    break;
                case Commands.Exec:
                    int cnt;
                    if(int.TryParse(cmdstr, out cnt))
                    {
                        _arnie.exec(cnt);
                    }
                    else
                    {
                        _arnie.exec(1);
                    }
                    break;
                case Commands.Input:
                    _arnie.input(cmdstr);
                    break;
                case Commands.Import:
                    if(File.Exists(cmdstr))
                    {
                        _arnie.import(cmdstr);
                    }
                    else
                    {
                        Console.WriteLine("file not found <" + cmdstr + ">");
                    }
                    break;
                case Commands.Export:
                    _arnie.export(cmdstr);
                    break;
                case Commands.TogglePrint:
                    _arnie.togglePrint();
                    break;
                case Commands.HelpPrint:
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
                    Console.WriteLine("\tCQΘC is x -> Θ(x)");
                    break;
                default:
                    break;
            }
        }
    }
}
