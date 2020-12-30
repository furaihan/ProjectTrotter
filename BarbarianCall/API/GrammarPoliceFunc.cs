using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrammarPolice.API;
using GrammarPolice.Actions;
using Rage;

namespace BarbarianCall.API
{
    internal static class GrammarPoliceFunc
    {
        public static void SetStatus(EGrammarPoliceStatusType statusType) => SetStatus(statusType, false, false);
        public static void SetStatus(EGrammarPoliceStatusType statusType, bool displayNotification, bool playSound )
        {
            switch (statusType)
            {
                case EGrammarPoliceStatusType.Available:
                    GrammarPolice.API.Functions.Available(playSound, displayNotification);
                    break;
                case EGrammarPoliceStatusType.StartPatrol:
                    GrammarPolice.API.Functions.StartPatrol(playSound, displayNotification);
                    break;
                case EGrammarPoliceStatusType.Busy:
                    GrammarPolice.API.Functions.Busy(playSound, displayNotification);
                    break;
                case EGrammarPoliceStatusType.EnRoute:
                    Callout.Accept();
                    break;
                case EGrammarPoliceStatusType.OnScene:
                    GrammarPolice.API.Functions.Scene(playSound, displayNotification);
                    break;
                case EGrammarPoliceStatusType.Investigation:
                    GrammarPolice.API.Functions.Investigating(playSound, displayNotification);
                    break;
                case EGrammarPoliceStatusType.Emergency: case EGrammarPoliceStatusType.Panic:
                    GrammarPolice.API.Functions.SetPanicStatus(playSound, displayNotification);
                    break;
                case EGrammarPoliceStatusType.InPursuit:
                    GrammarPolice.API.Functions.InPursit(playSound, displayNotification);
                    break;
                case EGrammarPoliceStatusType.TrafficStop:
                    GrammarPolice.API.Functions.TrafficStop(playSound, displayNotification);
                    break;
                case EGrammarPoliceStatusType.ReturnToStation:
                    GrammarPolice.API.Functions.ReturnToStation(playSound, displayNotification);
                    break;
                default:
                    GrammarPolice.API.Functions.Busy(playSound, displayNotification);
                    break;
            }
        }
        public static string GetCallsign() => GrammarPolice.API.Functions.GetCallsign();
        public static string GetCallsignAudio() => string.Join(" ", GrammarPolice.API.Functions.GetCallsignAudioParts());
        public enum EGrammarPoliceStatusType
        {
            Available,
            StartPatrol,
            Busy,
            EnRoute,
            OnScene,
            Investigation,
            Panic,
            Emergency,
            InPursuit,
            TrafficStop,
            ReturnToStation
        }
    }
}
