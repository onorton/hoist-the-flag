using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private List<AudioSource> _audioSources;


    // Threshold in seconds for considering things happening at the same time
    private float _simultaneousThreshold = 0.05f;


    // Start is called before the first frame update
    void Start()
    {
        _audioSources = GetComponentsInChildren<AudioSource>().ToList();
    }


    public void Play(AudioClip clip)
    {
        var sourceNotPlaying = _audioSources.FirstOrDefault(s => !s.isPlaying);

        if (sourceNotPlaying != null && !_audioSources.Any(s => s.isPlaying && s.clip == clip && s.time < _simultaneousThreshold))
        {
            sourceNotPlaying.clip = clip;
            sourceNotPlaying.Play();
        }

    }

    public void ToggleMute(bool mute)
    {
        AudioListener.pause = mute;
    }
}