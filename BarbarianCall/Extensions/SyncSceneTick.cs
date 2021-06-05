using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using BarbarianCall.Types;
using BarbarianCall.Extensions;

namespace BarbarianCall.Extensions
{
    internal static class SyncSceneTick
    {
        private static Ped PlayerPed => Game.LocalPlayer.Character;
        private static int sofaStatus = 0;
        private static SynchronizedScene syncScene;
        private static Rage.Object bench;
        private static Vector3 benchInitialPos;
        private static Rotator benchInitialRot;
        private static Rage.Task closeTask;
        private static readonly List<string> benchIdles =  new() { "idle_a", "idle_b", "idle_c" };
        private static readonly List<Model> models = new()
        {
            2663909392,
            2977869181,
            3531108208,
            3232156621,
            3666247552,
            3079285877,
            2037887057,
            1805980844,
            4195466914,
            3891075673,
            437354449,
            1290593659
        };
        internal static void SofaOnTick()
        {         
            switch (sofaStatus)
            {
                case 0:
                    Rage.Object[] objs = World.GetEntities(PlayerPed.Position, 2f, GetEntitiesFlags.ConsiderAllObjects).Where(x => x.IsObject()).Select(x => (Rage.Object)x).ToArray();
                    bench = objs.Where(x => models.Contains(x.Model)).OrderBy(x => Vector3.DistanceSquared(x.Position, PlayerPed)).FirstOrDefault();
                    if (!bench) break;
                    Peralatan.DisplayHelpTextWithGXTEntriesThisFrame("MPTV_WALK");
                    if (Game.IsControlPressed(2, GameControl.Context)) sofaStatus = 1;
                    break;
                case 1:
                    benchInitialPos = SynchronizedScene.GetAnimationInitialOffsettPosition("anim@amb@office@seating@male@var_a@base@", "enter", bench.Position, bench.Rotation);
                    benchInitialRot = SynchronizedScene.GetAnimationInitialOffsettRotation("anim@amb@office@seating@male@var_a@base@", "enter", bench.Position, bench.Rotation);
                    closeTask = PlayerPed.Tasks.FollowNavigationMeshToPosition(benchInitialPos, benchInitialRot.ToHeading(), 1f, 10000);
                    sofaStatus = 2;
                    break;
                case 2:
                    if (closeTask.Status == Rage.TaskStatus.InProgress) break;
                    syncScene = new SynchronizedScene(bench.Position, bench.Rotation);
                    syncScene.TaskToPed(PlayerPed, "anim@amb@office@seating@male@var_a@base@", "enter", 13);
                    sofaStatus = 3;
                    break;
                case 3:
                    if (syncScene.Phase != 1f) break;
                    syncScene = new SynchronizedScene(bench.Position, bench.Rotation);
                    syncScene.TaskToPed(PlayerPed, "anim@amb@office@seating@male@var_a@base@", "base", 13, playbackRate: 1148846080);
                    sofaStatus = 4;
                    break;
                case 4:
                    Peralatan.DisplayHelpTextWithGXTEntriesThisFrame("HAIR_SOFA_STAND");
                    if (Game.IsControlPressed(2, GameControl.FrontendCancel))
                    {
                        sofaStatus = 5;
                    }
                    if (syncScene.Phase != 1f) break;
                    syncScene = new SynchronizedScene(bench.Position, bench.Rotation);
                    syncScene.TaskToPed(PlayerPed, "anim@amb@office@seating@male@var_a@base@", benchIdles.GetRandomElement(), 13, playbackRate: 1148846080);
                    sofaStatus = 3;
                    break;
                case 5:
                    syncScene = new SynchronizedScene(bench.Position, bench.Rotation);
                    syncScene.TaskToPed(PlayerPed, "anim@amb@office@seating@male@var_a@base@", "exit", 13, playbackRate: 1000f);
                    sofaStatus = 6;
                    break;
                case 6:
                    if (syncScene.Phase != 1f) break;
                    PlayerPed.Tasks.Clear();
                    SynchronizedScene.DeleteAllSynchronizedScene();
                    sofaStatus = 0;
                    break;
            }
        }
    }
}
