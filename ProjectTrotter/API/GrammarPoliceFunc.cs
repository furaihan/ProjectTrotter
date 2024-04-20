using GrammarPolice.Actions;
using Func = GrammarPolice.API.Functions;
using System.IO;
using System;

namespace ProjectTrotter.API
{
    internal static class GrammarPoliceFunc
    {
        private const string DllPath = @"Plugins\LSPDFR\GrammarPolice.dll";
        private static readonly bool IsValid = File.Exists(DllPath) && Initialization.IsLSPDFRPluginRunning("GrammarPolice");
        public static void SetStatus(EGrammarPoliceStatusType statusType) => SetStatus(statusType, false, false);
        public static void SetStatus(EGrammarPoliceStatusType statusType, bool displayNotification, bool playSound )
        {
            if (IsValid)
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
                    case EGrammarPoliceStatusType.Emergency:
                    case EGrammarPoliceStatusType.Panic:
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
        }
        public static string GetCallsign() => IsValid ? Func.GetCallsign() : string.Empty;
        public static string GetCallsignAudio() => IsValid ? string.Join(" ", Func.GetCallsignAudioParts()) : string.Empty;
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
        static GrammarPoliceFunc()
        {
            if (IsValid) AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        }
        private static System.Reflection.Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.ToLower().StartsWith("grammarpolice, ") && IsValid)
            {
                return System.Reflection.Assembly.Load(File.ReadAllBytes(DllPath));
            }
            return null;
        }
    }
}
