using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio {

    namespace Sound {

        public abstract class BaseSound : ScriptableObject {
            [SerializeField] protected string soundName;
            [SerializeField][Range(0f, 1f)] protected float volume = 0.7f;
            [SerializeField][Range(.1f, 3f)] protected float pitch = 1f;
            [SerializeField] protected bool ignoreListenerPause = false; // see - https://gamedevbeginner.com/10-unity-audio-tips-that-you-wont-find-in-the-tutorials/#audiolistener_pause
            [SerializeField] protected bool realtimeEditorInspect = false;
            [SerializeField] protected AudioMixerGroup mixerGroup = null;
            [SerializeField][Range(0f, 1f)] protected float spatialBlend = 1f;
            [SerializeField][Range(0f, 360f)] protected float spread = 180f;
            [SerializeField][Range(1f, 20f)] protected float minFalloffDistance = 20f;
            [SerializeField][Range(5f, 100f)] protected float maxFalloffDistance = 40f;
            [SerializeField][Range(0f, 5f)] protected float dopplerLevel = 1f;

            // getters
            public new string name => soundName;
            public abstract bool hasClip { get; }
            public abstract bool hasSource { get; }
            public abstract bool isPlaying { get; }
            public float Volume => volume;
            public float Pitch => pitch;

            // public methods

            public abstract void Init(MonoBehaviour script, AudioMixerGroup mix = null, AudioSource _source = null);

            public abstract void Play();

            public abstract void Stop();

            // PRIVATE

            protected abstract IEnumerator RealtimeEditorInspection();
        }
    }
}

