using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [System.Serializable]
    public class SFXSounds
    {
        public string tag;
        public AudioClip audio;
    }
    public List<SFXSounds> sfxSounds;
    public Dictionary<string, AudioClip> sfxDictionary;
    #region Singleton
    public static SFXManager instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion
    private void Start()
    {
        sfxDictionary = new Dictionary<string, AudioClip>();
        foreach (SFXSounds sfx in sfxSounds)
        {
           sfxDictionary.Add(sfx.tag, sfx.audio);
        }
    }
    public void PlaySFX(AudioSource audioSource, string tagAudio)
    {
        if (!sfxDictionary.ContainsKey(tagAudio))
        {
            Debug.LogWarning("AudioClip with tag " + tag + " doesn´t exist.");
        }
        else
        {
            AudioClip audioClip = sfxDictionary[tagAudio];
            audioSource.clip = audioClip;
            audioSource.Play();
        }
           
        
    }
    public void SetVolume(AudioSource audioSource,float volume)
    {
        audioSource.volume = volume;
    }
}
