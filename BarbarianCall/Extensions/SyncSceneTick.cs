using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using Rage.Native;
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
        private static readonly List<uint> models = new()
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
            1290593659,
            3510013129,
            4020259920,
            0xfbbe41fb,
            98421364,
            3742736044,
            3043304331,
            4123023395
        };
        internal static void SofaOnTick()
        {         
            switch (sofaStatus)
            {
                case 0:
                    Rage.Object[] objs = World.GetEntities(PlayerPed.Position, 2f, GetEntitiesFlags.ConsiderAllObjects).Where(x => x.IsObject()).Select(x => (Rage.Object)x).ToArray();
                    bench = objs.Where(x => models.Contains(x.Model.Hash)).OrderBy(x => Vector3.DistanceSquared(x.Position, PlayerPed)).FirstOrDefault();
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
                    Peralatan.DisplayHelpTextWithGXTEntriesThisFrame("MPOFSEAT_PCEXIT");
                    if (Game.IsControlPressed(2, GameControl.ScriptRRight))
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
        internal class Seat
        {
            internal string PropName { get; set; }
            internal string ScenarioName { get; set; }
            internal Vector3 Offsett { get; set; }
            internal float Heading { get; set; }
            internal Seat(string prop, string scen, Vector3 off, float head)
            {
                PropName = prop;
                ScenarioName = scen;
                Offsett = off;
                Heading = head;
            }
        }
        private static List<Seat> Seats = new()
        {
            new Seat("prop_bench_01a", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0.5f), 180),
            new Seat("prop_bench_01b", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0.5f), 180),
            new Seat("prop_bench_01c", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0.5f), 180),
            new Seat("prop_bench_02", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0.5f), 180),
            new Seat("prop_bench_03", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0.5f), 180),
            new Seat("prop_bench_04", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0.5f), 180),
            new Seat("prop_bench_05", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0.5f), 180),
            new Seat("prop_bench_06", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0.5f), 180),
            new Seat("prop_bench_05", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0.5f), 180),
            new Seat("prop_bench_08", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0.5f), 180),
            new Seat("prop_bench_09", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0.5f), 180),
            new Seat("prop_bench_10", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0.5f), 180),
            new Seat("prop_bench_11", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0.5f), 180),
            new Seat("prop_fib_3b_bench", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0.5f), 180),
            new Seat("prop_ld_bench01", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0.5f), 180),
            new Seat("prop_wait_bench_01", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0.5f), 180),
            new Seat("hei_prop_heist_off_chair", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("hei_prop_hei_skid_chair", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_chair_01a", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_chair_01b", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_chair_02", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_chair_03", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_chair_04a", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_chair_04b", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_chair_05", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_chair_06", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_chair_05", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_chair_08", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_chair_09", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_chair_10", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_chateau_chair_01", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_clown_chair", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_cs_office_chair", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_direct_chair_01", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_direct_chair_02", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_gc_chair02", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_off_chair_01", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_off_chair_03", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_off_chair_04", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_off_chair_04b", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_off_chair_04_s", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_off_chair_05", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_old_deck_chair", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_old_wood_chair", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_rock_chair_01", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_skid_chair_01", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_skid_chair_02", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_skid_chair_03", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_sol_chair", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_wheelchair_01", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_wheelchair_01_s", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("p_armchair_01_s", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("p_clb_officechair_s", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("p_dinechair_01_s", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("p_ilev_p_easychair_s", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("p_soloffchair_s", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("p_yacht_chair_01_s", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("v_club_officechair", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("v_corp_bk_chair3", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("v_corp_cd_chair", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("v_corp_offchair", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("v_ilev_chair02_ped", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("v_ilev_hd_chair", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("v_ilev_p_easychair", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("v_ret_gc_chair03", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_ld_farm_chair01", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_table_04_chr", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_table_05_chr", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_table_06_chr", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("v_ilev_leath_chr", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_table_01_chr_a", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_table_01_chr_b", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_table_02_chr", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_table_03b_chr", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_table_03_chr", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_torture_ch_01", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("v_ilev_fh_dineeamesa", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("v_ilev_fh_kitchenstool", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("v_ilev_tort_stool", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("v_ilev_fh_kitchenstool", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("v_ilev_fh_kitchenstool", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("v_ilev_fh_kitchenstool", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("v_ilev_fh_kitchenstool", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("hei_prop_yah_seat_01", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("hei_prop_yah_seat_02", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("hei_prop_yah_seat_03", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_waiting_seat_01", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_yacht_seat_01", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_yacht_seat_02", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_yacht_seat_03", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_hobo_seat_01", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_rub_couch01", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("miss_rub_couch_01", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_ld_farm_couch01", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_ld_farm_couch02", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_rub_couch02", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_rub_couch03", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_rub_couch04", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("p_lev_sofa_s", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("p_res_sofa_l_s", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("p_v_med_p_sofa_s", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("p_yacht_sofa_01_s", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("v_ilev_m_sofa", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("v_res_tre_sofa_s", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("v_tre_sofa_mess_a_s", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("v_tre_sofa_mess_b_s", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("v_tre_sofa_mess_c_s", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_roller_car_01", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
            new Seat("prop_roller_car_02", "PROP_HUMAN_SEAT_BENCH", new Vector3(0f, 0f, 0f), -90),
        };
		public static void ChairSit()
        {
            List<uint> objs = Seats.Select(x => Game.GetHashKey(x.PropName)).ToList();
            Rage.Object obj = null;
            int status = 0;
            bool playing = false;
            Rage.Task scenarioTask;
            while(true)
            {
                GameFiber.Yield();                
                switch (status)
                {
                    case 0:
                        obj = World.GetEntities(PlayerPed.Position, 2f, GetEntitiesFlags.ConsiderAllObjects).Where(x => x && x.IsObject()).OrderBy(x => x.DistanceToSquared(PlayerPed)).Cast<Rage.Object>().FirstOrDefault();
                        if (!obj) break;
                        if (!objs.Contains(obj.Model.Hash)) break;
                        Peralatan.DisplayHelpTextWithGXTEntriesThisFrame("MPTV_WALK");
                        if (Game.IsControlPressed(2, GameControl.Context)) status = 1;
                        break;
                    case 1:
                        if (playing) break;
                        Seat seat = Seats[objs.IndexOf(obj.Model.Hash)];
                        PlayerPed.Tasks.FollowNavigationMeshToPosition(obj.Position + obj.ForwardVector * -1f, obj.Heading + seat.Heading, 1f, 10000);
                        scenarioTask =  PlayerPed.PlayScenarioAction(seat.ScenarioName, obj.Position + seat.Offsett, obj.Heading, -1, true, true);
                        GameFiber.Wait(2500);
                        playing = true;
                        status = 2;
                        break;
                    case 2:
                        Peralatan.DisplayHelpTextWithGXTEntriesThisFrame("MPOFSEAT_PCEXIT");
                        if (Game.IsControlPressed(2, GameControl.ScriptRRight)) status = 3;
                        break;
                    case 3:
                        PlayerPed.Tasks.Clear();
                        GameFiber.Wait(2500);
                        playing = false;
                        break;
                }
            }
        }
	}
}
