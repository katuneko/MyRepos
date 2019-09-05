using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cpu.Generic;

namespace MEF
{
    class OS
    {
        List<ImportedCpu> _icpu;
        List<GeneratedCpu> _gcpu;
        private static readonly SemaphoreSlim _debugSem = new SemaphoreSlim(1, 1);

        public OS() {
            //ここでcpu以下のDLLをすべて読み込む
            _icpu = ImportCpu();
            _gcpu = new List<GeneratedCpu>();
            debugPrintTitle();
            debugPrintImportedCpu();
        }
        private void debugPrintTitle(){
            Assembly asm = Assembly.GetExecutingAssembly();
            Version ver = asm.GetName().Version;
            Console.WriteLine("-- MEF OS ver." + ver + " --");
        }
        private void debugPrintHelp()
        {
            Console.WriteLine("<Commands>");
            Console.WriteLine("    CPU Manage");
            Console.WriteLine("        import <DLL or Folder Path>: ");
            Console.WriteLine("        generate <iCPU ID>: ");
            Console.WriteLine("        delete <gCPU ID>:");
            Console.WriteLine("        download <Image Path>:");
            Console.WriteLine("        run <gCPU ID> <count>");
            Console.WriteLine("        stop <gCPU ID>");
            Console.WriteLine("        copy <gCPU ID>");
            Console.WriteLine("        group <gCPU ID> ...");
            Console.WriteLine("        ungroup <gCPU ID> ...");
            Console.WriteLine("        capture <gGpu ID>: 各ポートの入出力の取得。取りこぼし注意。probeは内部状態の取得でキャプチャの一種。実装はCPU側。マネージャ指定ポートを指定する。");
            Console.WriteLine("        Sync:単位実行の同期。Group単位で同期？取りたくないケースに対処必要。");
            Console.WriteLine("        quit");
            Console.WriteLine("");
            Console.WriteLine("    Connection");
            Console.WriteLine("        link <upstream gCPU ID> <upstream gCPU Port> <downstream gCPU ID> <downstream gCPU Port>");
            Console.WriteLine("        unlink <gCPU ID> <kind> <Port>");
            Console.WriteLine("");
            Console.WriteLine("    DebugPrint");
            Console.WriteLine("        state -cpu -link -counter");
            Console.WriteLine("");
            Console.WriteLine("    Help");
            Console.WriteLine("        help");
        }
        private void debugPrintImportedCpu()
        {
            Console.WriteLine("Imported CPU List:");
            for (int i = 0;  i < _icpu.Count; i++)
            {
                ImportedCpu cpu = _icpu[i];
                Console.WriteLine("[" + i + "]" + cpu.getName());
            }
        }
        public bool import(string path) {
            return true;
        }
        public bool generate(int iCpuId) {
            bool ret = false;
            try
            {
                GeneratedCpu genCpu = new GeneratedCpu(_icpu[iCpuId]._cputype, _icpu[iCpuId].getName());
                _gcpu.Add(genCpu);
                ret = true;
            }
            catch
            {

            }
            return ret;
        }
        public bool delete(int gCpuId) {
            bool ret = false;
            try{
                _gcpu.RemoveAt(gCpuId);
                ret = true;
            }catch{

            }
            return ret;
        }
        public bool download(string imagePath) {
            return true;
        }
        public bool run(int gCpuId) {
            bool ret = false;
            try{
                _gcpu[gCpuId].startThread();
                ret = true;
            }catch{

            }
            return ret;
        }
        public bool stop(int gCpuId) {

            bool ret = false;
            try{
                _gcpu[gCpuId].stopThread();
                ret = true;
            }catch{

            }
            return ret; 
        }
        public bool link(int inCpuId, int inPortNo, int outCpuId, int outPortNo) {
            bool ret = false;
            try
            {
                ret = _gcpu[inCpuId].addLink(inPortNo, _gcpu[outCpuId], outPortNo);
            }
            catch
            {

            }
            return ret;
        }
        public bool unlink(int gCpuId, int portNo) {
            bool ret = false;
            try
            {
                ret = _gcpu[gCpuId].removeLink(portNo);
            }
            catch
            {

            }
            return ret;
        }
        public bool state() {
            debugPrintAllCpuState();
            return true;
        }
        private void debugPrintAllCpuState()
        {
            for (int i = 0; i < _gcpu.Count; i++)
            {
                Console.Write("[" + i + "] " );
                GeneratedCpu.State s = _gcpu[i].getState();
                switch (s)
                {
                    case GeneratedCpu.State.Run:
                        Console.Write("<RUN> ");
                        break;
                    case GeneratedCpu.State.Stop:
                        Console.Write("<STOP> ");
                        break;
                    case GeneratedCpu.State.Halt:
                    case GeneratedCpu.State.Invalid:
                    default:break;
                }
                Console.Write(_gcpu[i].getCpuName());
                Console.WriteLine("");

                _debugSem.Wait();
                Port p = _gcpu[i].getPort();
                Dictionary<int, dynamic> buf = new Dictionary<int, dynamic>(p._buf);
                Console.WriteLine("Port State");
                foreach (dynamic d in buf)
                {
                    Console.WriteLine("[{0}], Value = {1}", d.Key, d.Value);
                }
                _debugSem.Release();
            }
        }
        public bool probe() {
            return true;
        }
        public bool trace() {
            return true;
        }
        public bool help() {
            debugPrintHelp();
            return true;
        }
        public class ImportedCpu
        {
            public Type _cputype;
            public string getName(){
                return _cputype.Name;
            }
        }

        private static List<ImportedCpu> ImportCpu()
        {
            List<ImportedCpu> cpuList = new List<ImportedCpu>();
            //CPUフォルダ
            string rootCpuFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            rootCpuFolder += "\\cpu";
            if (!Directory.Exists(rootCpuFolder)) {
                throw new ApplicationException("cpuフォルダ\"" + rootCpuFolder + "\"が見つかりませんでした。");
            }

            //.dllファイルを探す
            string[] folders = Directory.GetDirectories(rootCpuFolder);
            foreach (string folder in folders) {
                string[] dlls = Directory.GetFiles(folder, "*.dll");
                foreach (string dll in dlls)
                {
                    try
                    {
                        //アセンブリとして読み込む
                        Assembly asm = Assembly.LoadFrom(dll);
                        bool isImport = true;
                        foreach(Type t in asm.GetTypes()){
                            //アセンブリ内のすべての型について、
                            //プラグインとして有効か調べる
                            if (t.IsClass && t.IsPublic && !t.IsAbstract && t.FullName != "Cpu.Generic.Port" && t.FullName != "Cpu.Generic.ICpu")
                            {
                                //Assemblyじゃなくてもよい？
                                ImportedCpu cpu = new ImportedCpu();
                                cpu._cputype = t;
                                cpuList.Add(cpu);
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
            return cpuList;
        }
        public class PortSpec
        {
            public struct Spec
            {
                int portNo;
                string service;
            }
            public Spec[] _spec;
            public void setPortSpec(Spec[] s)
            {
                s = _spec;
            }
            public Spec[] getPortSpec()
            {
                return _spec;
            }
        }
        class Dummy
        {

        }

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

        private class GeneratedCpu
        {
            private string _cpuName;
            private string _aliasName;
            private PortSpec _pspec;
            private ICpu _cpu;
            private InternalState _inState;
            private readonly SemaphoreSlim _stopSem = new SemaphoreSlim(0, 1);
            private readonly object _locker= new object();
            private List<Link> _linkList;
            private readonly SemaphoreSlim _linkSem = new SemaphoreSlim(1, 1);
            private Barrier _groupBarrier;
            private bool _isBarrierEnable;
            public enum State
            {
                Run, Stop, Halt, Invalid
            }
            private enum InternalState
            {
                Continue, Single, Stop, Halt
            }
            public GeneratedCpu(Type cpuType, string cpuName)
            {
                _cpuName = cpuName;
                if (cpuType != null)
                {
                    var obj = Activator.CreateInstance(cpuType, "");
                    _cpu = (ICpu)obj;
                }
                _inState = InternalState.Stop;
                var task = MakeThread();
                _linkList = new List<Link>();
                _isBarrierEnable = false;
            }
            ~GeneratedCpu()
            {
                removeThread();
            }
            public void execLoop()
            {
                while(true)
                {
                    if ((_inState == InternalState.Continue) || (_inState == InternalState.Single))
                    {
                        _debugSem.Wait();
                        _cpu.step();
                        if(_isBarrierEnable){
                            _groupBarrier.SignalAndWait();
                        }
                        _linkSem.Wait();
                        foreach (Link l in _linkList)
                        {
                            l.send();
                        }
                        _linkSem.Release();
                        _debugSem.Release();
                    }else if (_inState == InternalState.Stop)
                    {
                        _stopSem.Wait();
                        _stopSem.Release();
                    }
                    else if (_inState == InternalState.Halt)
                    {
                        break;
                    }

                    if(_inState == InternalState.Single){
                        _inState = InternalState.Stop;
                    }
                }
            }
            private async Task<Dummy> MakeThread()
            {
                Dummy errorInfo = new Dummy();
                var Loop = Task.Run(() => execLoop());
                try
                {
                    await Loop;
                }
                catch
                {
                    return errorInfo;
                }
                return errorInfo;
            }
            public string getCpuName()
            {
                return _cpuName;
            }
            public void setAlias(string aliasName)
            {
                _aliasName = aliasName;
            }
            public string getAlias()
            {
                return _aliasName;
            }
            public bool addLink(int inPortNo, GeneratedCpu outCpu, int outPortNo)
            {
                foreach(Link l in _linkList){
                    if(l._inPortNo == inPortNo){
                        return false;
                    }
                }
                Link newL = new Link(this, inPortNo, outCpu, outPortNo);
                _linkSem.Wait();
                _linkList.Add(newL);
                _linkSem.Release();
                return true;//#todo
            }
            public bool removeLink(int inPortNo)
            {
                foreach(Link l in _linkList){
                    if(l._inPortNo == inPortNo){
                        _linkSem.Wait();
                        _linkList.Remove(l);
                        _linkSem.Release();
                        return true;
                    }
                }
                return false;//#todo
            }
            public State getState()
            {
                State ret = State.Invalid;
                switch (_inState)
                {
                    case InternalState.Continue:
                    case InternalState.Single:
                        ret = State.Run;
                        break;
                    case InternalState.Stop:
                        ret = State.Stop;
                        break;
                    case InternalState.Halt:
                        ret = State.Halt;
                        break;
                    default:
                        break;
                }
                return ret;
            }
            public bool startThread()
            {
                bool ret = false;
                _inState = InternalState.Continue;
                if(_stopSem.CurrentCount == 0)
                {
                    _stopSem.Release();
                    ret = true;
                }
                return ret;
            }
            public bool stopThread()
            {
                bool ret = false;
                if (_stopSem.CurrentCount == 1)
                {
                    _stopSem.Wait();
                    ret = true;
                }
                _inState = InternalState.Stop;
                return ret;
            }
            public void removeThread()
            {
                _inState = InternalState.Halt;
            }
            public void stepThread()
            {
                _inState = InternalState.Single;
            }
            public Port getPort()
            {
                return _cpu.getPort();
            }
            public void enableBarrier(Barrier b){
                _groupBarrier = b;
                _isBarrierEnable = true;
            }
            public void disableBarrier(){
                _isBarrierEnable = false;
            }
        }
    }
}
