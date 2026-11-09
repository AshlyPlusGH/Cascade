using System;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;

namespace Ash
{
    [CreateAssetMenu]
    public class Soundtrack : ScriptableObject
    {
        [SerializeField] private static bool debug;
        private static void Log(string contents){ if (debug){ Debug.Log(contents); }}

        private static EventInstance trackPlaying;

        private static readonly Dictionary<ENUM_SountrackTag, string> lookupTable = new()
        {
            { ENUM_SountrackTag.Track_0, "event:/Soundtrack/Track_0" },
            { ENUM_SountrackTag.Track_1, "event:/Soundtrack/Track_1" },
            { ENUM_SountrackTag.Track_2, "event:/Soundtrack/Track_2" }
        };

        public static VCA volumeController => RuntimeManager.GetVCA("vca:/Music");
        public static void SetVolume(float newVolume){ volumeController.setVolume(newVolume); }

        public static void PlayTrack(ENUM_SountrackTag track)
        { 
            trackPlaying.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            trackPlaying.release();

            trackPlaying = RuntimeManager.CreateInstance(lookupTable[track]);
            trackPlaying.start();
        }
        public static void PlayTrackByPosition(int pos)
        {
                Log("Playing Track " + pos);
            pos += 1; //Skip None tag
            var values = (ENUM_SountrackTag[])Enum.GetValues(typeof(ENUM_SountrackTag));
            
            PlayTrack(values[pos]);
        }

        public ENUM_SountrackTag argument;

        public void PlayTrackSetArgument(){ PlayTrack(argument); }
        public static void PlayTrackTriggerString(string sfx){ if (System.Enum.TryParse<ENUM_SountrackTag>(sfx, true, out var tag)){ PlayTrack(tag); }}
    }

    public enum ENUM_SountrackTag
    {
        None,
        Track_0,
        Track_1,
        Track_2,
        Track_3,
        Track_4,
        Track_5,
        Track_6,
        Etc
    }
}