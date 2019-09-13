using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MEF
{
    class GroupManager
    {
        /* todo */
        //直列実行
        List<Group> _groupList;
        public GroupManager()
        {
            _groupList = new List<Group>();
        }
        public bool groupMarge(List<GeneratedCpu> g)
        {
            return true;//todo
        }
        public bool groupIndep(List<GeneratedCpu> gCpuList)
        {
            this.ungroup(gCpuList);
            Group newGrp = new Group();
            foreach (GeneratedCpu gCpu in gCpuList)
            {
                newGrp.addGroup(gCpu);
            }
            _groupList.Add(newGrp);
            return true;//todo

        }
        public bool ungroup(List<GeneratedCpu> gCpuList)
        {
            foreach (GeneratedCpu gCpu in gCpuList)
            {
                foreach (Group g in _groupList)
                {
                    if (g.Exists(gCpu))
                    {
                        g.removeGroup(gCpu);
                    }
                }
            }
            return true;//todo
        }
    }

    class Group
    {
        public List<GeneratedCpu> _groupCpu;
        public Barrier _b;
        public Group()
        {
            _groupCpu = new List<GeneratedCpu>();
            _b = new Barrier(0);
        }
        public bool addGroup(GeneratedCpu g)
        {
            try
            {
                _b.AddParticipant();
                _groupCpu.Add(g);
                g.enableBarrier(_b);
                return true;
            }
            catch
            {
                return false;
            }

        }
        public bool removeGroup(GeneratedCpu g)
        {
            try
            {
                g.disableBarrier();
                _groupCpu.Remove(g);
                _b.RemoveParticipant();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Exists(GeneratedCpu g)
        {
            return _groupCpu.Contains(g);
        }
    }
}
