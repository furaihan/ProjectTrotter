using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public static void WaitAudioScannerCompletion()
        {
            while (true)
            {
                GameFiber.Yield();
                if (!LF.GetIsAudioEngineBusy()) break;
            }
        }
        public static void RequestBackup(Vector3 location, EBackupResponseType responseType)
        {
            int rand = Peralatan.Random.Next(1, 1000);
            if (rand < 500) LF.RequestBackup(location, responseType, EBackupUnitType.LocalUnit);
            else if (rand < 825) LF.RequestBackup(location, responseType, EBackupUnitType.StateUnit);
            else LF.RequestBackup(location, responseType, EBackupUnitType.SwatTeam);
        }
        public static void RequestAirUnit(Vector3 location, EBackupResponseType responseType)
        {
            int rand = Peralatan.Random.Next(1, 1000);
            if (rand < 800) LF.RequestBackup(location, responseType, EBackupUnitType.AirUnit);
            else LF.RequestBackup(location, responseType, EBackupUnitType.NooseAirUnit);
        }
    }
}
