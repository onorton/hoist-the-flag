using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private List<AudioSource> _audioSources;
    private Dictionary<int, int> _audioSourcePriorities;


    // Threshold in seconds for considering things happening at the same time
    private float _simultaneousThreshold = 0.05f;


    // Start is called before the first frame update
    void Start()
    {
        _audioSources = GetComponentsInChildren<AudioSource>().ToList();
        _audioSourcePriorities = new Dictionary<int, int>();
    }


    public void Play(AudioClip clip, int priority = 0)
    {
        var sourceNotPlaying = _audioSources.FirstOrDefault(s => !s.isPlaying);

        if (sourceNotPlaying != null)
        {
            if (!_audioSources.Any(s => s.isPlaying && s.clip == clip && s.time < _simultaneousThreshold))
            {
                sourceNotPlaying.clip = clip;
                sourceNotPlaying.Play();
                _audioSourcePriorities[_audioSources.IndexOf(sourceNotPlaying)] = priority;
            }
            return;
        }

        // Check for lowest priority
        var lowerPrioritySources = _audioSourcePriorities.Where(p => p.Value > priority).OrderByDescending(p => p.Value).Select(p => _audioSources[p.Key]).ToList();

        if (lowerPrioritySources.Count > 0)
        {
            var source = lowerPrioritySources.First();
            source.clip = clip;
            source.Play();
            _audioSourcePriorities[_audioSources.IndexOf(source)] = priority;

        }


    }

    public void ToggleMute(bool mute)
    {
        AudioListener.pause = mute;
    }
}