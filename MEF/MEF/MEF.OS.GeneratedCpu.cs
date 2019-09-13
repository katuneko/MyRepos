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
        private ICpu _cpu;
        private InternalState _inState;
        private readonly SemaphoreSlim _stopSem = new SemaphoreSlim(0, 1);
        private readonly object _locker = new object();
        private List<Link> _linkList;
        private readonly SemaphoreSlim _linkSem = new SemaphoreSlim(1, 1);
        private Barrier _groupBarrier;
        private bool _isBarrierEnable;
        private SemaphoreSlim _debugSem;
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
        public void setDebugSem(SemaphoreSlim s)//todo: debug
        {
            _debugSem = s;
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
                    _debugSem.Wait();
                    _cpu.step();
                    if (_isBarrierEnable)
                    {
                        _groupBarrier.SignalAndWait();
                    }
                    _linkSem.Wait();
                    foreach (Link l in _linkList)
                    {
                        l.send();
                    }
                    _linkSem.Release();
                    _debugSem.Release();
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
            _linkSem.Wait();
            _linkList.Add(newL);
            _linkSem.Release();
            return true;//#todo
        }
        public bool removeLink(int inPortNo)
        {
            foreach (Link l in _linkList)
            {
                if (l._inPortNo == inPortNo)
                {
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
