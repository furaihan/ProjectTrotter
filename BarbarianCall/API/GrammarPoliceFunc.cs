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
        public static void SetStatus(EGrammarPoliceStatusType statusType, bool displayNotification = false, bool playSound = false)
        {
            switch (statusType)
            {
                case EGrammarPoliceStatusType.Available:
                    Functions.Available(playSound, displayNotification);
                    break;
                case EGrammarPoliceStatusType.StartPatrol:
                    Functions.StartPatrol(playSound, displayNotification);
                    break;
                case EGrammarPoliceStatusType.Busy:
                    Functions.Busy(playSound, displayNotification);
                    break;
                case EGrammarPoliceStatusType.EnRoute:
                    Callout.Accept();
                    break;
                case EGrammarPoliceStatusType.OnScene:
                    Functions.Scene(playSound, displayNotification);
                    break;
                case EGrammarPoliceStatusType.Investigation:
                    Functions.Investigating(playSound, displayNotification);
                    break;
                case EGrammarPoliceStatusType.Panic:
                    Functions.Panic();
                    break;
                case EGrammarPoliceStatusType.Emergency:
                    Backup.Panic();
                    break;
                case EGrammarPoliceStatusType.InPursuit:
                    Functions.InPursit(playSound, displayNotification);
                    break;
                case EGrammarPoliceStatusType.TrafficStop:
                    Functions.TrafficStop(playSound, displayNotification);
                    break;
                case EGrammarPoliceStatusType.ReturnToStation:
                    Functions.ReturnToStation(playSound, displayNotification);
                    break;
                default:
                    Functions.Busy(playSound, displayNotification);
                    break;
            }
        }
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
