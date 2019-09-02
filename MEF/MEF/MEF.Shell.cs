using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEF
{
    class Shell
    {
        public enum Response{
            SUCCESS, ERROR_EXEC, ERROR_CMD, INVALID, HALT
        }
        private OS _os;
        public Shell(){
            _os = new OS();
        }
        private enum Command{
            Invalid, Import,Generate,Delete,Download,Run,Stop,Link,Unlink,State,Probe,Trace,Help,Quit
        }
        public Response CommnandExecute(string str){
            string[] arg = str.Split(' ');
            Command c = Command.Invalid;
            switch(arg[0]){
                case "import":
                case "imp":
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
                case "remove":
                case "rem":
                case "rm":
                    c = Command.Delete;
                    break;
                case "download":
                case "down":
                case "dwn":
                case "dl":
                case "d":
                case "program":
                case "prog":
                case "prg":
                    c = Command.Download;
                    break;
                case "run":
                case "r":
                    c = Command.Run;
                    break;
                case "stop":
                case "stp":
                case "s":
                case "halt":
                case "hlt":
                    c = Command.Stop;
                    break;
                case "link":
                case "lnk":
                case "l":
                    c = Command.Link;
                    break;
                case "unlink":
                case "ulink":
                case "ulnk":
                case "unl":
                case "u":
                    c = Command.Unlink;
                    break;
                case "state":
                case "stat":
                case "sta":
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
            int iCpuId, gCpuId, inCpuId, inPortNo, outCpuId, outPortNo;
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
                    ret = _os.import(arg[1]);
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
                    ret = _os.generate(gCpuId);
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
                    ret = _os.delete(gCpuId);
                    rsp = ret ? Response.SUCCESS : Response.ERROR_EXEC;
                    break;
                case Command.Download: 
                    if(arg.Length < 2){
                        rsp = Response.ERROR_CMD;
                        break;
                    }
                    ret = _os.download(arg[1]);
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
                    ret = _os.run(gCpuId);
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
                    ret = _os.stop(gCpuId);
                    rsp = ret ? Response.SUCCESS : Response.ERROR_EXEC;
                    break;
                case Command.Link:
                    if (arg.Length < 4)
                    {
                        rsp = Response.ERROR_CMD;
                        break;
                    }
                    if(!Int32.TryParse(arg[1], out inCpuId))
                    {
                        rsp = Response.ERROR_CMD;
                        break;
                    }
                    if (!Int32.TryParse(arg[2], out inPortNo))
                    {
                        rsp = Response.ERROR_CMD;
                        break;
                    }
                    if (!Int32.TryParse(arg[3], out outCpuId))
                    {
                        rsp = Response.ERROR_CMD;
                        break;
                    }
                    if (!Int32.TryParse(arg[4], out outPortNo))
                    {
                        rsp = Response.ERROR_CMD;
                        break;
                    }
                    ret = _os.link(inCpuId, inPortNo, outCpuId, outPortNo);
                    rsp = ret ? Response.SUCCESS : Response.ERROR_EXEC;
                    break;
                case Command.Unlink: 
                    break;
                case Command.State:
                    ret = _os.state();
                    rsp = ret ? Response.SUCCESS : Response.ERROR_EXEC;
                    break;
                case Command.Probe: 
                    break;
                case Command.Trace: 
                    break;
                case Command.Help:
                    _os.help();
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
