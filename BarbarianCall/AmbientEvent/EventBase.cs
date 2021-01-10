using BarbarianCall.Types;
using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbarianCall.AmbientEvent
{
    internal class EventBase : IAmbientEvent
    {
        public bool EventRunning { get; set; }
        public List<Entity> Entities { get; set; }
        public List<Blip> Blips { get; set; }
        public SpawnPoint SpawnPoint { get; set; }
        public GameFiber EventProcessFiber { get; set; }

        public EventBase(SpawnPoint spawnPoint)
        {
            EventRunning = true;
            SpawnPoint = spawnPoint;
            Entities = new List<Entity>();
            Blips = new List<Blip>();
            EventProcessFiber = new GameFiber(delegate
            {
                while (EventRunning)
                {
                    GameFiber.Yield();
                    Process();
                }
            }, "[BarbarianCall] " + GetType().Name + " Process Fiber");
        }
        public virtual bool Create()
        {
            $"{GetType().Name} | Started".ToLog();
            foreach (Entity entity in Entities)
            {
                if (!entity)
                {
                    End();
                    return false;
                }
            }
            return true;
        }

        public void End()
        {
            throw new NotImplementedException();
        }

        public void Process()
        {
            throw new NotImplementedException();
        }

        public void Run()
        {
            $"{GetType().Name} | Event Run".ToLog();
        }
    }
}
