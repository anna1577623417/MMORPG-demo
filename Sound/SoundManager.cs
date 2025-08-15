using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoSingleton<SoundManager> {

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource soundAudioSource;

    const string MusicPath = "Music/";
    const string SoundPath = "Sound/";

    protected override void OnStart() {
        this.MusicVolume = Config.MusicVolume;
        this.SoundVolume = Config.SoundVolume;
        this.MusicOn = Config.MusicOn;
        this.SoundOn = Config.SoundOn;
    }

    private bool musicOn;
    
    public bool MusicOn {
        get { return musicOn; }
        set {
            musicOn = value;
            this.MusicMute(!musicOn);
        }
    }

    private bool soundOn;
    public bool SoundOn {
        get { return soundOn; }
        set {
            soundOn = value;
            this.SoundcMute(!soundOn);
            if (soundOn) this.SetVolume("SoundVolume", soundVolume);
        }
    }

    private int musicVolume;
    public int MusicVolume {
        get { return musicVolume; }
        set {
            musicVolume = value;
            if (musicOn) this.SetVolume("MusicVolume", musicVolume);
        }
    }

    private int soundVolume;
    public int SoundVolume {
        get { return soundVolume; }
        set {
            soundVolume = value;
        }
    }


    private void SoundcMute(bool mute) {
        this.SetVolume("SoundVolume", mute ? 0 : soundVolume);
    }

    private void MusicMute(bool mute) {
        this.SetVolume("MusicVolume",mute?0:musicVolume);
    }

    private void SetVolume(string name, int value) {
        float volume = value * 0.5f - 50f;
        this.audioMixer.SetFloat(name, volume);
    }

    internal void PlayMusic(string name) {
        AudioClip clip = Resloader.Load<AudioClip>(MusicPath + name);
        if (clip == null) {
            Debug.LogWarningFormat("PlayMusic：{0} not existed", name);
            return;
        }
        if (musicAudioSource.isPlaying) {
            musicAudioSource.Stop();
        }
        musicAudioSource.clip = clip;
        musicAudioSource.Play();
    }

    internal void PlaySound(string name) {
        AudioClip clip = Resloader.Load<AudioClip>(SoundPath + name);
        if(clip == null) {
            Debug.LogWarningFormat("PlaySound：{0} not existed", name);
            return;
        }
        soundAudioSource.PlayOneShot(clip);

    }
    protected void PlayClipOnAudioSource(AudioSource source,AudioClip clip,bool isLoop) {

    }
}
