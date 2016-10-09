using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DynamicAudioSource
{
    public const float DEFAULT_MIN_DISTANCE = 5.0f;
    public const float DEFAULT_MAX_DISTANCE = 20.0f;
    public AudioSource Source { get; private set; }
    public string Name { get; private set; }
    private Dictionary<string, AudioClip> clips;
    
    public DynamicAudioSource(NamedAudioSource namedSource, List<NamedAudioClip> namedClips)
    {
        if (namedClips == null)
            throw new System.ArgumentException();

        Source = namedSource.source;
        Name = namedSource.name;
        clips = new Dictionary<string, AudioClip>();
        foreach (NamedAudioClip namedClip in namedClips)
            clips.Add(namedClip.name, namedClip.clip);

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
    public AudioManager(List<DynamicAudioSource> dySources)
    {
        if (dySources == null)
            throw new System.ArgumentException();

        sources = new Dictionary<string, DynamicAudioSource>();
        foreach (DynamicAudioSource dySource in dySources)
            sources.Add(dySource.Name, dySource);
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
}
