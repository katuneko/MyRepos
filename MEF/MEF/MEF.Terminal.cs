using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEF
{
    class Terminal
    {
        public enum Response{
            SUCCESS, ERROR_EXEC, ERROR_CMD, INVALID, HALT
        }
        private Manager mCpuMng;
        public Terminal(){
            mCpuMng = new Manager();
        }
        private enum Command{
            Invalid, Import,Generate,Delete,Download,Run,Stop,Link,Unlink,State,Probe,Trace,Help,Quit
        }
        public Response CommnandExecute(string str){
            string[] arg = str.Split(' ');
            Command c = Command.Invalid;
            switch(arg[0]){
                case "import":
                case "i":
                    c = Command.Import;
                    break;
                case "generate":
                case "gen":
                case "g":
                    c = Command.Generate;
                    break;
                case "delete":
                case "del":
                case "kill":
                case "k":
                    c = Command.Delete;
                    break;
                case "download":
                case "dl":
                case "d":
                    c = Command.Download;
                    break;
                case "run":
                case "r":
                    c = Command.Run;
                    break;
                case "stop":
                case "halt":
                case "program":
                case "s":
                    c = Command.Stop;
                    break;
                case "link":
                case "l":
                    c = Command.Link;
                    break;
                case "unlink":
                case "u":
                    c = Command.Unlink;
                    break;
                case "state":
                case "j":
                    c = Command.State;
                    break;
                case "probe":
                case "p":
                    c = Command.Probe;
                    break;
                case "trace":
                case "t":
                    c = Command.Trace;
                    break;
                case "help":
                case "h":
                    c = Command.Help;
                    break;
                case "quit":
                case "q":
                case "exit":
                case "e":
                    c = Command.Quit;
                    break;
                default:
                    break;
            }

            Response rsp = Response.INVALID;
            bool ret = false;
            int iCpuId, gCpuId;
            bool isSuccess;
            switch(c){
                case Command.Invalid: 
                    rsp = Response.INVALID;
                    break;
                case Command.Import:
                    if(arg.Length < 2){
                        rsp = Response.ERROR_CMD;
                        break;
                    }
                    ret = mCpuMng.import(arg[1]);
                    rsp = ret ? Response.SUCCESS : Response.ERROR_EXEC;
                    break;
                case Command.Generate:
                    if (arg.Length < 2)
                    {
                        rsp = Response.ERROR_CMD;
                        break;
                    }
                    isSuccess = Int32.TryParse(arg[1], out gCpuId);
                    if (!isSuccess)
                    {
                        rsp = Response.ERROR_CMD;
                        break;
                    }
                    ret = mCpuMng.generate(gCpuId);
                    rsp = ret ? Response.SUCCESS : Response.ERROR_EXEC;

                    break;
                case Command.Delete: 
                    if(arg.Length < 2){
                        rsp = Response.ERROR_CMD;
                        break;
                    }
                    isSuccess = Int32.TryParse(arg[1], out gCpuId);
                    if (!isSuccess)
                    {
                        rsp = Response.ERROR_CMD;
                        break;
                    }
                    ret = mCpuMng.delete(gCpuId);
                    rsp = ret ? Response.SUCCESS : Response.ERROR_EXEC;
                    break;
                case Command.Download: 
                    if(arg.Length < 2){
                        rsp = Response.ERROR_CMD;
                        break;
                    }
                    ret = mCpuMng.download(arg[1]);
                    rsp = ret ? Response.SUCCESS : Response.ERROR_EXEC;
                    break;
                case Command.Run:
                    if (arg.Length < 2)
                    {
                        rsp = Response.ERROR_CMD;
                        break;
                    }
                    isSuccess = Int32.TryParse(arg[1], out gCpuId);
                    if (!isSuccess)
                    {
                        rsp = Response.ERROR_CMD;
                        break;
                    }
                    ret = mCpuMng.run(gCpuId);
                    rsp = ret ? Response.SUCCESS : Response.ERROR_EXEC;
                    break;
                case Command.Stop:
                    if (arg.Length < 2)
                    {
                        rsp = Response.ERROR_CMD;
                        break;
                    }
                    isSuccess = Int32.TryParse(arg[1], out gCpuId);
                    if (!isSuccess)
                    {
                        rsp = Response.ERROR_CMD;
                        break;
                    }
                    ret = mCpuMng.stop(gCpuId);
                    rsp = ret ? Response.SUCCESS : Response.ERROR_EXEC;
                    break;
                case Command.Link: 
                    break;
                case Command.Unlink: 
                    break;
                case Command.State: 
                    break;
                case Command.Probe: 
                    break;
                case Command.Trace: 
                    break;
                case Command.Help: 
                    break;
                case Command.Quit:
                    rsp = Response.HALT;
                    break;
                default:
                    break;
            }
            return rsp;
        }
    }
}
