using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Threading;
using Cpu.Generic;

namespace MEF
{
    class OS
    {
        List<ImportedCpu> _icpu;
        List<GeneratedCpu> _gcpu;
        GroupManager _gmng;
        private static readonly SemaphoreSlim _debugSem = new SemaphoreSlim(1, 1);

        public OS() {
            //ここでcpu以下のDLLをすべて読み込む
            _icpu = ImportCpu();
            _gcpu = new List<GeneratedCpu>();
            _gmng = new GroupManager();
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
                genCpu.setDebugSem(_debugSem);
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
                _gcpu[gCpuId].dispose();
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
            try
            {
                return _gcpu[inCpuId].addLink(inPortNo, _gcpu[outCpuId], outPortNo);
            }
            catch
            {
                return false;
            }
        }
        public bool unlink(int gCpuId, int portNo) {
            try
            {
                return _gcpu[gCpuId].removeLink(portNo);
            }
            catch
            {
                return false;
            }
        }
        public bool group(List<int> groupList)
        {
            try
            {
                List<GeneratedCpu> g = new List<GeneratedCpu>();
                foreach(int i in groupList)
                {
                    g.Add(_gcpu[i]);
                }
                return _gmng.groupIndep(g);
            }
            catch
            {
                return false;
            }
        }
        public bool ungroup(List<int> groupList)
        {
            try
            {
                List<GeneratedCpu> g = new List<GeneratedCpu>();
                foreach (int i in groupList)
                {
                    g.Add(_gcpu[i]);
                }
                return _gmng.ungroup(g);
            }
            catch
            {
                return false;
            }
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
                Dictionary<int, Stack<dynamic>> buf = new Dictionary<int, Stack<dynamic>>(p._buf);
                Console.WriteLine("Port State");
                foreach (dynamic d in buf)
                {
                    dynamic c;
                    try
                    {
                        c = d.Value.Peek();
                    }
                    catch
                    {
                        c = null;
                    }
                    Console.WriteLine("[{0}], Value = {1}", d.Key, c);
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
    }
}
