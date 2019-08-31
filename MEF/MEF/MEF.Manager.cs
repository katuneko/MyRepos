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
    class Manager
    {
        List<ImportedCpu> _icpu;
        List<GeneratedCpu> _gcpu;
        
        public Manager() {
            //ここでcpu以下のDLLをすべて読み込む
            _icpu = ImportCpu();
            var task = ExecuteCpuManager();
            _gcpu = new List<GeneratedCpu>();
            debugPrintTitle();
            debugPrintImportedCpu();
        }
        private void debugPrintTitle(){
            Console.WriteLine("-- MEF User Interface --");
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
            Console.WriteLine("[Import CPU List]");
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
            GeneratedCpu genCpu = new GeneratedCpu(_icpu[iCpuId]._cputype);
            _gcpu.Add(genCpu);
            return true;
        }
        public bool delete(int gCpuId) {
            return true;
        }
        public bool download(string imagePath) {
            return true;
        }
        public bool run(int gCpuId) {
            return _gcpu[gCpuId].startThread();
        }
        public bool stop(int gCpuId) {
            return _gcpu[gCpuId].stopThread(); 
        }
        public bool link(int gCpuId, int portNo) {
            return true;
        }
        public bool unlink(int gCpuId, int portNo) {
            return true;
        }
        public bool state() {
            return true;
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
                            if (t.IsClass && t.IsPublic && !t.IsAbstract && t.FullName != "Cpu.Generic.Port")

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
        private async Task<Dummy> ExecuteCpuManager()
        {
            Dummy errorInfo = new Dummy();
            var Loop = Task.Run(() => executeCpuMngLoop());
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

        private void executeCpuMngLoop()
        {
            Object _lockObj;
            _lockObj = new object();
            while (true)
            {
                lock (_lockObj)
                {

                }
                try
                {

                }
                catch
                {

                }
                finally
                {

                }
            }
        }

        private class Dummy
        {

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

        private class GeneratedCpu
        {
            private string _aliasName;
            private PortSpec _pspec;
            private ICpu _cpu;
            private InternalState _inState;
            private readonly SemaphoreSlim _stopSem = new SemaphoreSlim(1, 1);
            private readonly object _locker= new object();
            public enum State
            {
                Run, Stop, Halt, Invalid
            }
            private enum InternalState
            {
                Continue, Single, Stop, Halt
            }
            public GeneratedCpu(Type cpuType)
            {
                if (cpuType != null)
                {
                    var obj = Activator.CreateInstance(cpuType, "");
                    _cpu = (ICpu)obj;
                }
                var task = MakeThread();
            }
            ~GeneratedCpu()
            {
                removeThread();
            }
            public void execLoop()
            {
                while(true)
                {
                    if (_inState == InternalState.Continue)
                    {
                        _cpu.step();
                    }
                    else if (_inState == InternalState.Single)
                    {
                        _cpu.step();
                        _inState = InternalState.Stop;
                    }
                    else if (_inState == InternalState.Stop)
                    {
                        _stopSem.Wait();
                        _stopSem.Release();
                    }
                    else if (_inState == InternalState.Halt)
                    {
                        break;
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
        }
    }
}
