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
            Shell term = new Shell();
            Shell.Response rsp = Shell.Response.INVALID;
            string str = "";
            while(rsp != Shell.Response.HALT){
                str = Console.ReadLine();
                try
                {
                    rsp = term.CommnandExecute(str);
                    switch (rsp)
                    {
                        case Shell.Response.SUCCESS:
                            Console.WriteLine("Command Execution Success.");
                            break;
                        case Shell.Response.ERROR_CMD:
                            Console.WriteLine("Invalid Command.");
                            break;
                        case Shell.Response.ERROR_EXEC:
                            Console.WriteLine("Execution Error.");
                            break;
                        case Shell.Response.INVALID:
                            Console.WriteLine("Unknown Error.");
                            break;
                        case Shell.Response.HALT:
                            Console.WriteLine("Halt.");
                            break;
                        default: break;
                    }
                }
                catch
                {
                    Console.WriteLine("Unknown Error.");
                }
            }
        }
    }
}
