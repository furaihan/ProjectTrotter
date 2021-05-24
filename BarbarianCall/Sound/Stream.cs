using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using static Rage.Native.NativeFunction;

namespace BarbarianCall.Sound
{
    public class Stream
    {
        public string Name { get; set; }
        public string SoundSet { get; set; }
        public Stream(string name, string soundSet)
        {
            Name = name;
            SoundSet = soundSet;
        }
        public Stream(string name)
        {
            Name = name;
            SoundSet = string.Empty;
        }
        public void Load()
        {
            GameFiber.StartNew(() =>
            {
                if (string.IsNullOrWhiteSpace(SoundSet))
                {
                    while (!Natives.LOAD_STREAM<bool>(Name, 0))
                    {
                        GameFiber.Yield();
                    }
                }      
                else
                {
                    while (!Natives.LOAD_STREAM<bool>(Name, SoundSet))
                    {
                        GameFiber.Yield();
                    }
                }
            });            
        }
        public void LoadAndWait()
        {
            if (string.IsNullOrWhiteSpace(SoundSet))
            {
                while (!Natives.LOAD_STREAM<bool>(Name, 0))
                {
                    GameFiber.Yield();
                }
            }
            else
            {
                while (!Natives.LOAD_STREAM<bool>(Name, SoundSet))
                {
                    GameFiber.Yield();
                }
            }
        }
        public void LoadWithStartOffset(int startOffset)
        {
            GameFiber.StartNew(() =>
            {
                if (string.IsNullOrWhiteSpace(SoundSet))
                {
                    while (!Natives.LOAD_STREAM_WITH_START_OFFSET<bool>(Name, startOffset, 0))
                    {
                        GameFiber.Yield();
                    }
                }
                else
                {
                    while (!Natives.LOAD_STREAM_WITH_START_OFFSET<bool>(Name, startOffset, SoundSet))
                    {
                        GameFiber.Yield();
                    }
                }
            });
        }
        public void LoadWithStartOffsetAndWait(int startOffset)
        {
            if (string.IsNullOrWhiteSpace(SoundSet))
            {
                while (!Natives.LOAD_STREAM_WITH_START_OFFSET<bool>(Name, startOffset, 0))
                {
                    GameFiber.Yield();
                }
            }
            else
            {
                while (!Natives.LOAD_STREAM_WITH_START_OFFSET<bool>(Name, startOffset, SoundSet))
                {
                    GameFiber.Yield();
                }
            }
        }
        public void PlayFrontEnd()
        {
            Natives.PLAY_STREAM_FRONTEND();
        }
        public void PlayFromPosition(Vector3 position)
        {
            Natives.PLAY_STREAM_FROM_POSITION(position.X, position.Y, position.Z);
        }
        public void PlayFromEntity(Entity entity)
        {
            if (Natives.IS_ENTITY_A_PED<bool>(entity))
            {
                Natives.PLAY_STREAM_FROM_PED(entity);
            }
            else if (Natives.IS_ENTITY_A_VEHICLE<bool>(entity))
            {
                Natives.PLAY_STREAM_FROM_VEHICLE(entity);
            }
            else if (Natives.IS_ENTITY_AN_OBJECT<bool>(entity))
            {
                Natives.PLAY_STREAM_FROM_OBJECT(entity);
            }
        }
        public override string ToString()
        {
            return $"Name: {Name} SoundSet: {SoundSet}";
        }
        public static void StopAnyStream()
        {
            Natives.STOP_STREAM();
        }
        public static bool IsPlaying => Natives.IS_STREAM_PLAYING<bool>();
        public static int PlayTime => Natives.GET_STREAM_PLAY_TIME<int>();

    }
}
