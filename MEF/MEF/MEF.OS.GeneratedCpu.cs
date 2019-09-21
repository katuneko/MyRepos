using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Cpu.Generic;

namespace MEF
{
    class GeneratedCpu
    {
        private string _cpuName;
        private string _aliasName;
        private PortSpec _pspec;
        private CpuInterface _cpu;
        private InternalState _inState;
        private readonly SemaphoreSlim _stopSem = new SemaphoreSlim(0, 1);
        private List<Link> _linkList;
        //private readonly SemaphoreSlim _linkSem = new SemaphoreSlim(1, 1);
        private readonly object _linkLock = new object();
        private Barrier _groupBarrier;
        private bool _isBarrierEnable;
        private object _debugLock;
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
                _cpu = (CpuInterface)obj;
            }
            _inState = InternalState.Stop;
            var task = MakeThread();
            _linkList = new List<Link>();
            _isBarrierEnable = false;
        }
        public void setDebugLockObj(object o)//todo: debug
        {
            _debugLock = o;
        }
        public bool dispose()
        {
            removeThread();
            return true;
        }
        public void execLoop()
        {
            while (true)
            {
                if ((_inState == InternalState.Continue) || (_inState == InternalState.Single))
                {
                    lock(_debugLock){
                        _cpu.step();
                        if (_isBarrierEnable)
                        {
                            _groupBarrier.SignalAndWait();
                        }
                        lock(_linkLock){
                            //todo: データ交換もう少しきれいにしたい
                            foreach (Link l in _linkList)
                            {
                                l.send();
                            }
                            List<int> popPortNo= new List<int>();
                            foreach (Link l in _linkList)
                            {
                                if (!popPortNo.Contains(l._outPortNo))
                                {
                                    l.next();
                                }
                                popPortNo.Add(l._outPortNo);
                            }
                        }
                    }
                }
                else if (_inState == InternalState.Stop)
                {
                    _stopSem.Wait();
                    _stopSem.Release();
                }
                else if (_inState == InternalState.Halt)
                {
                    _cpu.dispose();
                    break;
                }

                if (_inState == InternalState.Single)
                {
                    _inState = InternalState.Stop;
                }
            }
        }
        private async Task<bool> MakeThread()
        {
            var Loop = Task.Run(() => execLoop());
            try
            {
                await Loop;
            }
            catch
            {
                return false;
            }
            return true;
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
            foreach (Link l in _linkList)
            {
                if (l._inPortNo == inPortNo)
                {
                    return false;
                }
            }
            Link newL = new Link(this, inPortNo, outCpu, outPortNo);
            lock(_linkLock){
                _linkList.Add(newL);
            }
            return true;//#todo
        }
        public bool removeLink(int inPortNo)
        {
            foreach (Link l in _linkList)
            {
                if (l._inPortNo == inPortNo)
                {
                    lock(_linkLock){
                        _linkList.Remove(l);
                    }
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
            if (_stopSem.CurrentCount == 0)
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
            if (_stopSem.CurrentCount == 0)
            {
                _stopSem.Release();
            }
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
        public void enableBarrier(Barrier b)
        {
            _groupBarrier = b;
            _isBarrierEnable = true;
        }
        public void disableBarrier()
        {
            _isBarrierEnable = false;
        }
    }
}
