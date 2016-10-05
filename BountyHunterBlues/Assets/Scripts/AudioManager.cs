using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioSourceWrapper
{
    /*
    public string sourceName;
    public AudioSource source;

    public AudioSourceWrapper(string sourceName, AudioSource source)
    {
        this.sourceName = sourceName;
        this.source = source;
    }
}

public class AudioClipWrapper
{
    public string clipName;
    public AudioClip clip;

    public AudioClipWrapper(string clipName, AudioClip clip)
    {
        this.clipName = clipName;
        this.clip = clip;
    }
}

public class AudioManager : MonoBehaviour {

    Dictionary<AudioSourceWrapper, AudioClipWrapper> sources;

	void Start()
    {
        sources = new Dictionary<AudioSourceWrapper, AudioClipWrapper>();
    }

    public void addSource(string sourceName, 
            AudioClip clip = null, bool loop = false, bool playOnAwake = false, float volume = 1.0f)
    {
        if (!sources.ContainsKey(sourceName)) {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            List<AudioClip> clips = new List<AudioClip>();
            if (clip != null) {
                source.clip = clip;
                clips.Add(clip);
            }
            KeyValuePair<AudioSource, List<AudioClip>> pair = new KeyValuePair<AudioSource, List<AudioClip>>(source, clips);
            sources.Add(sourceName, pair);
            setSourceAttributes(sourceName, loop, playOnAwake, volume);
        }

    }

    public void addClipToSource(string source, AudioClip clip)
    {
        List<AudioClip> clips;
        if (sources.TryGetValue(source, out clips))
            clips.Add(clip);
        else
        {
            clips = new List<AudioClip>();
            clips.Add(clip);
            sources.Add(source, clips);
        } 
    }

    public void playSource(AudioSource source)
    {
        sources.
        foreach (AudioSource s in sources.Keys)
        {

        }
    }

    public void setCurrentClip(string sourceName, AudioClip clip)
    {
        KeyValuePair<AudioSource, List<AudioClip>> pair;
        if(sources.TryGetValue(sourceName, out pair))
        {
            foreach (AudioClip c in pair.Value)
                if(c. == clip)
            pair.Key.clip = clip;
        }
    }

    public void setSourceAttributes(string sourceName, bool loop = false, bool playOnAwake = false, float volume = 1.0f)
    {

        KeyValuePair<AudioSource, List<AudioClip>> pair;
        if(sources.TryGetValue(sourceName, out pair))
        {
            AudioSource source = pair.Key;
            source.loop = loop;
            source.playOnAwake = playOnAwake;
            source.volume = volume;
        }
        
    }
    */
}
