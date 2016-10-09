using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
public class AudioSourceWrapper
{
    
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
*/


public class DynamicAudioSource
{
    public const float DEFAULT_MIN_DISTANCE = 5.0f;
    public const float DEFAULT_MAX_DISTANCE = 20.0f;
    public AudioSource Source { get; private set; }
    public string Name { get; private set; }
    private Dictionary<string, AudioClip> clips;
    
    public DynamicAudioSource(NamedAudioSource namedSource, params NamedAudioClip[] pairs)
    {
        if (pairs.Length == 0)
            throw new System.ArgumentException();

        Source = namedSource.source;
        Name = namedSource.name;
        clips = new Dictionary<string, AudioClip>();
        foreach (NamedAudioClip pair in pairs)
            clips.Add(pair.name, pair.clip);

        Source.loop = false;
        Source.playOnAwake = false;
        Source.volume = 1.0f;
        Source.minDistance = DEFAULT_MIN_DISTANCE;
        Source.maxDistance = DEFAULT_MAX_DISTANCE;
    }

    
    public DynamicAudioSource swapToClip(string name)
    {
        Source.clip = clips[name];
        return this;
    }

    public void setLoop(bool isLooping) { Source.loop = isLooping; }
    public void setPlayOnAwake(bool playOnAwake) { Source.playOnAwake = playOnAwake; }
    public void setVolume(float volume) { Source.volume = volume; }
    public void setMinDistance(float minDistance) { Source.minDistance = minDistance; }
    public void setMaxDistance(float maxDistance) { Source.maxDistance = maxDistance; }
    public void Play() { Source.Play(); }
    public void Stop() { Source.Stop(); }
    public void Pause() { Source.Pause(); }

}
public class AudioManager
{
    private Dictionary<string, DynamicAudioSource> sources;
    public AudioManager(params NamedAudioSource[] pairs)
    {
        if (pairs.Length == 0)
            throw new System.ArgumentException();

        sources = new Dictionary<string, DynamicAudioSource>();
        foreach (NamedAudioSource pair in pairs)
            sources.Add(pair.name, );
    }

    public void Play(string sourceName, string clipName = null)
    {
        if (clipName == null)
            sources[sourceName].Play();
        else
            sources[sourceName].swapToClip(clipName).Play();
    }

    public void Stop(string sourceName)
    {
        sources[sourceName].Stop();
    }

    public void Pause(string sourceName)
    {
        sources[sourceName].Pause();
    }
    /*
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
