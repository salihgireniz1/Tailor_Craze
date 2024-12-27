using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public enum SFXType
{
    Knitted, DepositPop, DepositFill, TrayDisappear, TrayMove, UnlockGrid
}

[System.Serializable]
public struct SoundClip
{
    public SFXType sfxType;
    public AudioClip clip;
}

public class SoundManager : MonoSingleton<SoundManager>
{
    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public float maxPitch = 1.5f;

    [Header("Audio Clips")]
    public List<SoundClip> sfxClips;  // List of sound clips with SFXType and AudioClip pair
    public AudioClip backgroundMusic;

    private Dictionary<SFXType, AudioClip> sfxClipDictionary = new Dictionary<SFXType, AudioClip>();
    public bool isSfxMuted;
    public bool isBgmMuted;

    private const string SFXMutedKey = "SFXMuted";
    private const string BGMMutedKey = "BGMMuted";

    protected override void Awake()
    {
        base.Awake(); // Call base class awake for Singleton setup

        // Initialize the dictionary from the list
        foreach (var soundClip in sfxClips)
        {
            if (!sfxClipDictionary.ContainsKey(soundClip.sfxType))
            {
                sfxClipDictionary.Add(soundClip.sfxType, soundClip.clip);
            }
        }

        // Load saved sound settings
        LoadSoundSettings();
    }
    [Button]
    public void Reset()
    {
        ES3.Save(SFXMutedKey, false);
        ES3.Save(BGMMutedKey, true);
    }

    private void Start()
    {
        PlayBackgroundMusic();
    }

    private void LoadSoundSettings()
    {
        isSfxMuted = ES3.Load<bool>(SFXMutedKey, defaultValue: false);
        isBgmMuted = ES3.Load<bool>(BGMMutedKey, defaultValue: true);

        sfxSource.mute = isSfxMuted;
        bgmSource.mute = isBgmMuted;
    }

    public void PlaySFX(SFXType sfxType, float pitchIncrease = 0f)
    {
        // if (sfxSource.isPlaying) return;
        if (sfxClipDictionary.TryGetValue(sfxType, out AudioClip clip) && clip != null)
        {
            sfxSource.PlayOneShot(clip);
            sfxSource.pitch = Mathf.Min(maxPitch, sfxSource.pitch + pitchIncrease);
            sfxSource.pitch += pitchIncrease;
        }
        else
        {
            Debug.LogWarning($"SoundManager: No audio clip assigned for {sfxType}");
        }
    }
    public void ResetPitch()
    {
        sfxSource.pitch = 1f;
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null)
        {
            bgmSource.clip = backgroundMusic;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    public void ToggleMuteSFX()
    {
        isSfxMuted = !isSfxMuted;
        sfxSource.mute = isSfxMuted;
        ES3.Save(SFXMutedKey, isSfxMuted);
    }

    public void ToggleMuteBGM()
    {
        isBgmMuted = !isBgmMuted;
        bgmSource.mute = isBgmMuted;
        ES3.Save(BGMMutedKey, isBgmMuted);
    }
}
