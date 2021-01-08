﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using LSPD_First_Response;
using LSPD_First_Response.Mod.API;
using BarbarianCall.Types;

namespace BarbarianCall
{
    //TODO: Add some AmbientEvent
    internal class EventHandler
    {
        internal static void AmbientDispatchCall()
        {
            string alphabet;
            if (Initialization.IsLSPDFRPluginRunning("GrammarPolice", new Version(1, 4, 2, 2)))
            {
                var userCS = API.GrammarPoliceFunc.GetCallsign().Split('-').Where(s => s.All(ss => !char.IsDigit(ss))).FirstOrDefault()[0];
                alphabet = string.Concat("ABCDEFGHIJKLMNOPQRSTUVWXYZ".Where(c => c != userCS));
            }
            else alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            DateTime ADTimer = DateTime.UtcNow + new TimeSpan(0, 0, 0, Peralatan.Random.Next(300, 850), Peralatan.Random.Next(100, 985));
            "Starting ambient dispatch call Loop".ToLog();
            while (true)
            {
                GameFiber.Yield();
                if (DateTime.UtcNow.CompareTo(ADTimer) > 0)
                {
                    try
                    {
                        GameFiber.Wait(500);
                        "Starting ambient dispatch call event".ToLog();
                        "Check if there is no audio scanner playing".ToLog();
                        if (Functions.GetIsAudioEngineBusy())
                        {
                            $"Audio engine is busy, cancelling event".ToLog();
                            ADTimer = DateTime.UtcNow + new TimeSpan(0, 0, 0, Peralatan.Random.Next(300, 850), Peralatan.Random.Next(100, 985));
                            $"Next ambient dispatch call should occured at {ADTimer.ToLocalTime().ToLongTimeString()}".ToLog();
                            continue;
                        }
                        "Audio engine is free, continuing process".ToLog();
                        Vector3 PlayerPos = Game.LocalPlayer.Character.Position;
                        string randomCallsign;
                        randomCallsign = Peralatan.Random.Next() % 4 == 0 ?
                            $"BAR_{alphabet.GetRandomElement(true)} BAR_{Peralatan.Random.Next(1, 28)}" :
                            $"BAR_{Peralatan.Random.Next(1, 10)} BAR_{alphabet.GetRandomElement(true)} BAR_{Peralatan.Random.Next(1, 28)}";
                        Vector3 randomLocation = SpawnManager.GetVehicleSpawnPoint(PlayerPos.Around(500, 650), Peralatan.Random.Next(1000, 2000), Peralatan.Random.Next(3000, 5000));
                        if (randomLocation == SpawnPoint.Zero) randomLocation = World.GetNextPositionOnStreet(PlayerPos.Around(Peralatan.Random.Next(1000, 2000), Peralatan.Random.Next(3000, 5000)));
                        string reporter = Peralatan.Random.Next() % 2 == 0 ? "WE_HAVE" : Peralatan.Random.Next() % 2 == 0 ? "CITIZENS_REPORT" : "OFFICERS_REPORT";
                        IDictionary<string, EBackupResponseType> ambientCrimes = new Dictionary<string, EBackupResponseType>()
                        {
                            {"CRIME_GRAND_THEFT_AUTO", EBackupResponseType.Code3 },
                            {"CRIME_BRANDISHING_WEAPON", EBackupResponseType.Code3 },
                            {"CRIME_ROBBERY" , EBackupResponseType.Code3 },
                            {"CRIME_HIT_AND_RUN" , EBackupResponseType.Code3 },
                            {"CRIME_GUNFIRE" , EBackupResponseType.Code3 },
                            {"CRIME_SUSPECT_ON_THE_RUN" , EBackupResponseType.Code3 },
                            {"BAR_CRIME_ASSAULT" , EBackupResponseType.Code3 },
                            {"BAR_CRIME_STABBED" , EBackupResponseType.Code3 },
                            {"BAR_CRIME_CIVILIAN_NEEDING_ASSISTANCE", EBackupResponseType.Code2 },
                            {"CRIME_CRIMINAL_ACTIVITY" , EBackupResponseType.Code3 },
                            {"CRIME_PED_STRUCT_BY_VEHICLE" , EBackupResponseType.Code2},
                            {"CRIME_CAR_ON_FIRE" , EBackupResponseType.Code3 },
                        };
#if DEBUG
                        $"DISPATCH_TO {randomCallsign} {reporter} {ambientCrimes.Values.GetRandomElement()} IN_OR_ON_POSITION {randomLocation.GetZoneName()}".Print();
                        randomLocation.ToString().Print();
#endif
                        string crime = ambientCrimes.Keys.GetRandomElement();
                        Functions.PlayScannerAudioUsingPosition($"DISPATCH_TO {randomCallsign} {reporter} {crime} IN_OR_ON_POSITION", randomLocation);
                        GameFiber.SleepUntil(() => !Functions.GetIsAudioEngineBusy(), -1);
                        GameFiber.Wait(2500);
                        GameFiber.SleepUntil(() => !Functions.GetIsAudioEngineBusy(), -1);
                        ambientCrimes.TryGetValue(crime, out var typ);
                        Functions.PlayScannerAudio("BAR_AI_RESPOND");
                        GameFiber.SleepUntil(() => !Functions.GetIsAudioEngineBusy(), -1);
                        GameFiber.Wait(1500);
                        GameFiber.SleepUntil(() => !Functions.GetIsAudioEngineBusy(), -1);
                        switch (typ)
                        {
                            case EBackupResponseType.Code2: Functions.PlayScannerAudio("UNITS_RESPOND_CODE_2"); break;
                            case EBackupResponseType.Code3: Functions.PlayScannerAudio("UNITS_RESPOND_CODE_3"); break;
                            default: GameFiber.Sleep(90); break;
                        }
                    }
                    catch (Exception e)
                    {
                        "Ambient Dispatch Call Error".ToLog();
                        e.ToString().ToLog();
                        NetExtension.SendError(e);
                    }
                    ADTimer = DateTime.UtcNow + new TimeSpan(0, 0, 0, Peralatan.Random.Next(300, 850), Peralatan.Random.Next(100, 985));
                    $"Next ambient dispatch call should occured at {ADTimer.ToLocalTime().ToLongTimeString()}".ToLog();
                }
            }
        }
    }
}
