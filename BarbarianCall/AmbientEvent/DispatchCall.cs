using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Rage;
using LSPD_First_Response;
using LSPD_First_Response.Mod.API;
using BarbarianCall.Types;

namespace BarbarianCall.AmbientEvent
{
    internal class DispatchCall
    {
        internal static void AmbientDispatchCall()
        {
            string alphabet;
            if (Initialization.IsLSPDFRPluginRunning("GrammarPolice", new Version(1, 4, 2, 2)))
            {
                char userCS = API.GrammarPoliceFunc.GetCallsign().Split('-').Where(s => s.All(ss => !char.IsDigit(ss))).FirstOrDefault()[0];
                alphabet = string.Concat("ABCDEFGHIJKLMNOPQRSTUVWXYZ".Where(c => c != userCS));
            }
            else alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            //Consider using stopwatch instead
            Stopwatch sw = new Stopwatch();
            TimeSpan timer = new TimeSpan(0, 0, 0, Peralatan.Random.Next(250, 850), Peralatan.Random.Next(100, 985));
            "Starting ambient dispatch call Loop".ToLog();
            $"First ambient dispatch call should occuered at {(DateTime.Now + timer).ToLongTimeString()}".ToLog();
            sw.Start();
            while (true)
            {
                GameFiber.Yield();
                if (sw.Elapsed.TotalMilliseconds > timer.TotalMilliseconds)
                {
                    try
                    {
                        GameFiber.Wait(500);
                        "Starting ambient dispatch call event".ToLog();
                        "Check if there is no audio scanner playing".ToLog();
                        if (Functions.GetIsAudioEngineBusy())
                        {
                            $"Audio engine is busy, cancelling event".ToLog();
                            timer = new TimeSpan(0, 0, 0, Peralatan.Random.Next(250, 850), Peralatan.Random.Next(100, 985));
                            $"Next ambient dispatch call should occured at {(DateTime.Now + timer).ToLongTimeString()}".ToLog();
                            sw.Restart();
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
                            {"CRIME_PED_STRUCK_BY_VEHICLE" , EBackupResponseType.Code2},
                            {"CRIME_CAR_ON_FIRE" , EBackupResponseType.Code3 },
                        };
                        string crime = ambientCrimes.Keys.GetRandomElement();
                        Functions.PlayScannerAudioUsingPosition($"DISPATCH_TO {randomCallsign} {reporter} {crime} IN_OR_ON_POSITION", randomLocation);
#if DEBUG
                        $"DISPATCH_TO {randomCallsign} {reporter} {crime} IN_OR_ON_POSITION {randomLocation.GetZoneName()}".Print();
                        randomLocation.ToString().Print();
#endif
                        API.LSPDFRFunc.WaitAudioScannerCompletion();
                        GameFiber.Wait(2500);
                        API.LSPDFRFunc.WaitAudioScannerCompletion();
                        ambientCrimes.TryGetValue(crime, out EBackupResponseType typ);
                        Functions.PlayScannerAudio("BAR_AI_RESPOND");
                        API.LSPDFRFunc.WaitAudioScannerCompletion();
                        GameFiber.Wait(1500);
                        API.LSPDFRFunc.WaitAudioScannerCompletion();
                        switch (typ)
                        {
                            case EBackupResponseType.Code2: Functions.PlayScannerAudio("BAR_ACK BAR_DELAY_500MS UNITS_RESPOND_CODE_02"); break;
                            case EBackupResponseType.Code3: Functions.PlayScannerAudio("BAR_ACK BAR_DELAY_500MS UNITS_RESPOND_CODE_03"); break;
                            default: GameFiber.Sleep(90); break;
                        }
                    }
                    catch (Exception e)
                    {
                        "Ambient Dispatch Call Error".ToLog();
                        e.ToString().ToLog();
                        NetExtension.SendError(e);
                    }
                    finally
                    {
                        timer = new TimeSpan(0, 0, 0, Peralatan.Random.Next(250, 850), Peralatan.Random.Next(100, 985));
                        $"Next ambient dispatch call should occured at {(DateTime.Now + timer).ToLongTimeString()}".ToLog();
                        sw.Restart();
                    }                  
                }
            }
        }
    }
}
