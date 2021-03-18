using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using BarbarianCall.Extensions;
using BarbarianCall.Types;

namespace BarbarianCall.Callouts
{
    class MassStreetFighting : CalloutBase
    {
        private List<Entity> Participant;
        private List<Ped> Gang1;
        private List<Ped> Gang2;
        private Ped Boss1;
        private Ped Boss2;
        private bool CanEnd = false;
        private List<Model> Gang1Model;
        private List<Model> Gang2Model;
        int gangMemberCount;
        private static readonly uint[] meleeWeapon = { 0x92A27487, 0x958A4A8F, 0xF9E6AA4B, 0x84BD7BFD, 0x4E875F73, 0xF9DCBF2D, 0xD8DF3C3C, 0x99B507EA, 0xDD5DF8D9, 0xDFE37640, 0x19044EE0, 0xCD274149, 0x3813FC08, 
            0x24B17070/*MOLOTOV*/ };
        RelationshipGroup gang1Relationship = new RelationshipGroup("SIJI");
        RelationshipGroup gang2Relationship = new RelationshipGroup("LORO");
        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutRunning = false;
            PursuitCreated = false;
            Participant = new List<Entity>();
            Gang1 = new List<Ped>();
            Gang2 = new List<Ped>();
            Spawn = SpawnManager.GetVehicleSpawnPoint(PlayerPed, 425, 725, true);
            if (Spawn == Spawnpoint.Zero) Spawn = SpawnManager.GetVehicleSpawnPoint(PlayerPed, 350, 850);
            if (Spawn == Spawnpoint.Zero)
            {
                Spawn.Position = World.GetNextPositionOnStreet(PlayerPed.Position.Around2D(425, 725));
                Spawn.Heading = SpawnManager.GetRoadHeading(Spawn.Position);
            }
            SpawnPoint = Spawn;
            SpawnHeading = Spawn;
            Gang1Model = CommonVariables.GangPedModels.Values.ToList().GetRandomElement();
            Gang2Model = CommonVariables.GangPedModels.Values.ToList().GetRandomElement(m=> m != Gang1Model);
            Gang1Model.ForEach(m => m.LoadAndWait());
            Gang2Model.ForEach(m => m.LoadAndWait());
            CalloutPosition = Spawn;
            CalloutAdvisory = string.Format("Total number of suspects is {0}", gangMemberCount * 2 + 2);
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            gangMemberCount = Peralatan.Random.Next(3, 9);
            for (int i = 0; i < gangMemberCount; i++)
            {
                Ped gangMember = new Ped(Gang1Model.GetRandomElement(), SpawnPoint.Around2D(50f), Peralatan.Random.Next() % 2 == 0 ? SpawnHeading : SpawnPoint.GetHeadingTowards(PlayerPed));
                gangMember.MakeMissionPed();
                Participant.Add(gangMember);
                CalloutEntities.Add(gangMember);
                gangMember.Inventory.GiveNewWeapon(meleeWeapon.GetRandomElement(), -1, true);
                gangMember.RelationshipGroup = gang1Relationship;
                gangMember.Metadata.BAR_Entity = true;
            }
            for (int i = 0; i < gangMemberCount; i++)
            {
                Ped gangMember = new Ped(Gang2Model.GetRandomElement(), SpawnPoint.Around2D(50f), Peralatan.Random.Next() % 2 == 0 ? SpawnHeading : SpawnPoint.GetHeadingTowards(PlayerPed));
                gangMember.MakeMissionPed();
                Participant.Add(gangMember);
                CalloutEntities.Add(gangMember);
                gangMember.Inventory.GiveNewWeapon(meleeWeapon.GetRandomElement(), -1, true);
                gangMember.RelationshipGroup = gang2Relationship;
                gangMember.Metadata.BAR_Entity = true;
            }
            gang1Relationship.SetRelationshipWith(gang2Relationship, Relationship.Hate);
            gang1Relationship.SetRelationshipWith(RelationshipGroup.Cop, Relationship.Hate);
            gang1Relationship.SetRelationshipWith(RelationshipGroup.Player, Relationship.Hate);
            gang2Relationship.SetRelationshipWith(gang1Relationship, Relationship.Hate);
            gang2Relationship.SetRelationshipWith(RelationshipGroup.Cop, Relationship.Hate);
            gang2Relationship.SetRelationshipWith(RelationshipGroup.Player, Relationship.Hate);
            return base.OnCalloutAccepted();
        }
        public override void Process()
        {
            if (CalloutRunning)
            {
                if (!Participant.All(e => CalloutEntities.Contains(e)))
                {
                    Participant.ForEach(e =>
                    {
                        if (e)
                        {
                            if (!CalloutEntities.Contains(e)) CalloutEntities.Add(e);
                        }
                    });
                }
            }
            base.Process();
        }
        public override void End()
        {
            Participant.ForEach(p =>
            {
                if (p)
                {
                    if (p.IsAlive && p.IsEntityAPed()) (p as Ped).Inventory.GiveNewWeapon("WEAPON_UNARMED", -1, true);
                    if (p) p.Dismiss();
                }
            });
            base.End();
        }
        private void GetClose()
        {

        }
    }
}
