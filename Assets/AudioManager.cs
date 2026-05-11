using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : ManagerLocatable
{
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private List<AudioEntry> sfxEntries;
    
    private Dictionary<string, AudioClip> _sfxLookup;

    protected override void OnInitialized()
    {
        _sfxLookup = new Dictionary<string, AudioClip>(sfxEntries.Count);
        foreach (var entry in sfxEntries)
            _sfxLookup[entry.Name] = entry.Clip;
    }

    public void PlaySfx(string sfxName)
    {
        if (_sfxLookup.TryGetValue(sfxName, out var clip))
            sfxSource.PlayOneShot(clip);
        else
            Debug.LogWarning($"AudioManager: SFX '{sfxName}' not found!");
    }

    [Serializable]
    public class AudioEntry
    {
        [SerializeField] private string name;
        [SerializeField] private AudioClip clip;
        
        public string Name => name;
        public AudioClip Clip => clip;
    }
}
