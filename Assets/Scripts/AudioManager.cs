using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private Sound[] sounds;

    private List<Source> _sources = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        foreach (var sound in sounds)
        {
            var src = new Source
            {
                name = sound.name,
                source = gameObject.AddComponent<AudioSource>()
            };

            _sources.Add(src);
            _sources[_sources.Count - 1].source.volume = sound.volume;
            _sources[_sources.Count - 1].source.pitch = sound.pitch;
            _sources[_sources.Count - 1].source.loop = sound.loop;
            _sources[_sources.Count - 1].source.clip = sound.clip;
            _sources[_sources.Count - 1].source.playOnAwake = sound.loop;
        }
    }

    public void PlaySound(string name)
    {
        foreach (var source in _sources.Where(source => source.name == name)) source.source.Play();
    }

    public AudioClip GetClip(string name)
    {
        return sounds.Where(sound => sound.name == name).Select(sound => sound.clip).FirstOrDefault();
    }
}

[System.Serializable]
public struct Source
{
    public string name;
    public AudioSource source;
}

[System.Serializable]
public struct Sound
{
    public string name;
    public float volume;
    public float pitch;
    public bool loop;
    public AudioClip clip;
}