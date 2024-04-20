namespace ProjectTrotter.AmbientEvent
{
    using Rage;
    using System.Collections.Generic;
    using ProjectTrotter.Types;
    internal interface IAmbientEvent
    {
        bool EventRunning { get; set; }
        List<Entity> Entities { get; set; }
        List<Blip> Blips { get; set; }
        Spawnpoint SpawnPoint { get; set; }
        bool Create();
        void Run();
        void End();
        void Process();
        GameFiber EventProcessFiber { get; set; }
    }
}
