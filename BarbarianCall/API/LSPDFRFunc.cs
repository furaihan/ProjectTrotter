using System;
using Rage;
using LSPD_First_Response;
using LF = LSPD_First_Response.Mod.API.Functions;
using LSPD_First_Response.Engine.Scripting.Entities;

namespace BarbarianCall.API
{
    internal static class LSPDFRFunc
    {
        public static void PlayScannerAudio(string audio, bool waitForCompletion = false)
        {
            LF.PlayScannerAudio(audio);
            if (waitForCompletion)
            {
                WaitAudioScannerCompletion();
            }
        }
        public static void PlayScannerAudioUsingPosition(string sound, Vector3 location, bool waitForCompletion = false)
        {
            LF.PlayScannerAudioUsingPosition(sound, location);
            if (waitForCompletion)
            {
                WaitAudioScannerCompletion();
            }
        }
        public static void WaitAudioScannerCompletion(TimeSpan timeout)
        {
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            while (true)
            {
                GameFiber.Yield();
                if (!LF.GetIsAudioEngineBusy()) break;
                if (stopwatch.ElapsedMilliseconds > timeout.TotalMilliseconds && timeout != System.Threading.Timeout.InfiniteTimeSpan) break;
            }
        }
        public static void WaitAudioScannerCompletion() => WaitAudioScannerCompletion(System.Threading.Timeout.InfiniteTimeSpan);
        public static void WaitAudioScannerCompletion(int timeoutMilliseconds) => WaitAudioScannerCompletion(TimeSpan.FromMilliseconds(timeoutMilliseconds));
        public static void RequestBackup(Vector3 location, EBackupResponseType responseType)
        {
            int rand = MyRandom.Next(1, 1000);
            if (rand < 500) LF.RequestBackup(location, responseType, EBackupUnitType.LocalUnit);
            else if (rand < 825) LF.RequestBackup(location, responseType, EBackupUnitType.StateUnit);
            else LF.RequestBackup(location, responseType, EBackupUnitType.SwatTeam);
        }
        public static void RequestAirUnit(Vector3 location, EBackupResponseType responseType)
        {
            int rand = MyRandom.Next(1, 1000);
            if (rand < 800) LF.RequestBackup(location, responseType, EBackupUnitType.AirUnit);
            else LF.RequestBackup(location, responseType, EBackupUnitType.NooseAirUnit);
        }
        public static Persona GetPedPersona(Ped ped) => LF.GetPersonaForPed(ped);
    }
}
