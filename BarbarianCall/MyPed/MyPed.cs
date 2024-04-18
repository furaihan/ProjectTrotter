using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;

namespace BarbarianCall.MyPed
{
    internal class MyPed : Ped
    {
        private PedComponentCollection _pedComponentCollection;
        private PedCombatProperty _pedCombatProperty;
        internal MyPed(Model model, Vector3 position, float heading) : base(model, position, heading)
        {
        }
        internal MyPed(Vector3 position, float heading) : base(position, heading)
        {
        }
        internal MyPed(Vector3 position) : base(position)
        {
        }
        protected MyPed(PoolHandle handle) : base(handle)
        {
        }
        internal PedCombatProperty CombatProperty
        {
            get
            {
                return _pedCombatProperty ??= new PedCombatProperty(this);
            }
        }
        internal PedComponentCollection Wardrobe
        {
            get
            {
                return _pedComponentCollection ??= new PedComponentCollection(this);
            }
        }
        internal static MyPed FromRegularPed(Ped ped)
        {
            return new MyPed(ped.Handle);
        }
    }
}
