using System;
using Rage;
using N = Rage.Native.NativeFunction;

namespace BarbarianCall.Types
{
    public class Mugshot : IHandleable, IDeletable, IEquatable<Mugshot>
    {
        public Ped Ped { get; protected set; }
        public string Texture { get; protected set; } = "CHAR_BLANK_ENTRY";
        public PoolHandle Handle { get; protected set; }
        public bool IsTransparentBackground { get; protected set; }
        public bool IsReady => N.Natives.IS_PEDHEADSHOT_READY<bool>((uint)Handle);
        public Mugshot(Ped ped, bool isTransparent = false)
        {
            GameFiber.StartNew(() =>
            {
                Ped = ped;
                int _handle = isTransparent ? N.Natives.REGISTER_PEDHEADSHOT_TRANSPARENT<int>(ped) : N.Natives.REGISTER_PEDHEADSHOT<int>(ped);
                Handle = new PoolHandle((uint)_handle);
                IsTransparentBackground = isTransparent;
                GameFiber.SleepUntil(() => IsReady, 1000);
                Logger.Log($"Mugshot creation is success: {IsReady}");
                Texture = IsReady ? N.Natives.GET_PEDHEADSHOT_TXD_STRING<string>((uint)Handle) : "CHAR_BLANK_ENTRY";
                ped.Metadata.BAR_MugshotCreated = true;
            });          
        }
        public static Mugshot FromPed(Ped ped, bool transparent) => new(ped, transparent);
        public static Mugshot FromPed(Ped ped) => FromPed(ped, false);
        private static string[] StringToArray(string str)
        {
            int stringsNeeded = (str.Length % 99 == 0) ? (str.Length / 99) : ((str.Length / 99) + 1);

            string[] outputString = new string[stringsNeeded];
            for (int i = 0; i < stringsNeeded; i++)
            {
                outputString[i] = str.Substring(i * 99, MathHelper.Clamp(str.Substring(i * 99).Length, 0, 99));
            }
            return outputString;
        }
        public uint DisplayNotification(string title, string subtitle, string text,bool fadein = false, bool blink = false)
        {
            string[] vs = StringToArray(text);
            GameFiber.SleepUntil(() => IsReady, 1000);
            N.Natives.BEGIN_TEXT_COMMAND_THEFEED_POST("CELL_EMAIL_BCON");
            foreach (string st in vs) N.Natives.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME(st);
            N.Natives.END_TEXT_COMMAND_THEFEED_POST_MESSAGETEXT<uint>(Texture, Texture, fadein, 4, title, subtitle);
            return N.Natives.END_TEXT_COMMAND_THEFEED_POST_TICKER<uint>(blink, true);
        }
        public bool IsValid() => N.Natives.IS_PEDHEADSHOT_VALID((uint)Handle);

        public bool Equals(IHandleable other) => other is not null && other is Mugshot && Handle == other.Handle;

        public void Delete() => N.Natives.UNREGISTER_PEDHEADSHOT((uint)Handle);

        public bool Equals(Mugshot other)
        {
            return other is not null && other.Ped == Ped;
        }
    }
}
