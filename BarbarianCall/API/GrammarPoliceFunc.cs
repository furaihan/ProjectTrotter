using GrammarPolice.Actions;
using Func = GrammarPolice.API.Functions;

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
                    Func.Available(playSound, displayNotification);
                    break;
                case EGrammarPoliceStatusType.StartPatrol:
                    Func.StartPatrol(playSound, displayNotification);
                    break;
                case EGrammarPoliceStatusType.Busy:
                    Func.Busy(playSound, displayNotification);
                    break;
                case EGrammarPoliceStatusType.EnRoute:
                    Callout.Accept();
                    break;
                case EGrammarPoliceStatusType.OnScene:
                    Func.Scene(playSound, displayNotification);
                    break;
                case EGrammarPoliceStatusType.Investigation:
                    Func.Investigating(playSound, displayNotification);
                    break;
                case EGrammarPoliceStatusType.Emergency: case EGrammarPoliceStatusType.Panic:
                    Func.SetPanicStatus(playSound, displayNotification);
                    break;
                case EGrammarPoliceStatusType.InPursuit:
                    Func.InPursit(playSound, displayNotification);
                    break;
                case EGrammarPoliceStatusType.TrafficStop:
                    Func.TrafficStop(playSound, displayNotification);
                    break;
                case EGrammarPoliceStatusType.ReturnToStation:
                    Func.ReturnToStation(playSound, displayNotification);
                    break;
                default:
                    Func.Busy(playSound, displayNotification);
                    break;
            }
        }
        public static string GetCallsign() => Func.GetCallsign();
        public static string GetCallsignAudio() => string.Join(" ", Func.GetCallsignAudioParts());
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
