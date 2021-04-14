using System.Collections.Generic;
using System.Linq;
using Rage;
using Rage.Native;

namespace BarbarianCall.Types
{
    internal class PedGroup : IHandleable, IDeletable
    {
        private static bool _valid = false;
        public PoolHandle Handle { get; protected set; }
        private static List<Ped> _groupMember = new();
        public List<Ped> GroupMember => _groupMember;
        public Ped GroupLeader { get; private set; }
        public int MemberCount 
        { 
            get
            {
                return _groupMember.Count;
            } 
        }
        public float SeparationRange 
        { 
            set
            { 
                uint handle = Handle;
                NativeFunction.Natives.SET_GROUP_SEPARATION_RANGE(handle, value);
            }
        }
        public static PedGroup CreateGroup(Ped leader, params Ped[] member)
        {
            uint handle = NativeFunction.Natives.CREATE_GROUP<uint>(0);
            if (handle == 0)
            {
                Peralatan.ToLog("Cannot create a new Ped Group");
                _valid = false;
                return null;
            }
            PedGroup pedGroup = new()
            {
                Handle = handle,
                GroupLeader = leader,
            };
            _groupMember = member.ToList();
            foreach (Ped mem in member)
            {
                NativeFunction.Natives.SET_PED_AS_GROUP_MEMBER(mem, handle);
            }
            NativeFunction.Natives.SET_PED_AS_GROUP_LEADER(leader, handle);
            _valid = true;
            return pedGroup;
        }
        public bool IsPedInThisGroup(Ped ped)
        {
            if (!IsValid()) throw new Rage.Exceptions.InvalidHandleableException(this);
            uint handle = Handle;
            return NativeFunction.Natives.IS_PED_GROUP_MEMBER<bool>(ped, handle);
        }
        public void RemovePedFromThisGroup(Ped ped)
        {
            if (!IsValid()) throw new Rage.Exceptions.InvalidHandleableException(this);
            if (!IsPedInThisGroup(ped)) return;
            NativeFunction.Natives.REMOVE_PED_FROM_GROUP(ped);
        }

        public bool Equals(IHandleable other)
        {
            return Handle == other.Handle;
        }

        public bool IsValid()
        {
            return _valid;
        }

        public void Delete()
        {
            if (!IsValid()) return;
            uint handle = Handle;
            NativeFunction.Natives.REMOVE_GROUP(handle);
            _valid = false;
        }
    }
}
