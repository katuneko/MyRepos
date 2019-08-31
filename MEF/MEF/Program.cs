using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEF
{
    class Program
    {
        static void Main(string[] args)
        {
            Terminal term = new Terminal();
            Terminal.Response rsp = Terminal.Response.INVALID;
            string str = "";
            while(rsp != Terminal.Response.HALT){
                str = Console.ReadLine();
                rsp = term.CommnandExecute(str);
                switch (rsp)
                {
                    case Terminal.Response.SUCCESS:
                        Console.WriteLine("Command Execution Success.");
                        break;
                    case Terminal.Response.ERROR_CMD:
                        Console.WriteLine("Command Execution Error(Invalid Command).");
                        break;
                    case Terminal.Response.ERROR_EXEC:
                        Console.WriteLine("Command Execution Error.(In Execute Error)");
                        break;
                    case Terminal.Response.INVALID:
                        Console.WriteLine("Unknown Error.");
                        break;
                    case Terminal.Response.HALT:
                        Console.WriteLine("Halt.");
                        break;
                    default: break;
                }
            }
        }
    }
}
