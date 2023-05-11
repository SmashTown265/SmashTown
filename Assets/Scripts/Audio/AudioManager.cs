using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Linq;

public class AudioManager : MonoBehaviour {

    private Sound[] sounds;
    public Sound[] music;
    public Sound[] soundEffects;
    public static AudioManager instance;

    // Start is called before the first frame update
    void Awake() {

        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        foreach (Sound s in music) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
          
        }
        foreach (Sound s in soundEffects) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            
        }
        
    }

    void Start() {
        Play("Theme");
    }

    public void Play (string name) {
        Sound s = Array.Find(music, sound => sound.name == name);
        if (s == null)
            s = Array.Find(soundEffects, sound => sound.name == name); 
        if (s == null)
        {
            print("Nope");
            return;
        }
        s.source.Play();
    }
    public void OnMusicVolChange(float musicVol)
    {
        foreach (Sound s in music)
        {
            s.source.volume =  musicVol;
        }
    }
    public void OnSoundEffectVolChange(float sfxVol)
    {
        foreach (Sound s in soundEffects)
        {
            s.source.volume = sfxVol;
        }
    }
}
