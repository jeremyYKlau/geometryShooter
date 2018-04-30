using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {


    public enum AudioChannel { Master, Sfx, Music };

    public float masterVolumePercent {get; private set; }
    public float sfxVolumePercent { get; private set; }
    public float musicVolumePercent { get; private set; }

    AudioSource sfx2Dsource;
    AudioSource[] musicScores;
    int activeMusicIndex;
    //this makes it so everyone function can access the audio manager which is what we want as this method only plays sounds and musics, it accesses no data
    public static AudioManager instance;

    Transform audioListener;
    Transform playerPos;

    SoundLibrary library;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject); //destroys duplicate sound managers however to stop overlapping of same noise
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject); //this is to make sure audio manager keeps playing between scene changes

            library = GetComponent<SoundLibrary>();
            musicScores = new AudioSource[2];
            for (int i = 0; i < 2; i++)
            {
                GameObject newMusicSource = new GameObject("Music source " + (i + 1));
                musicScores[i] = newMusicSource.AddComponent<AudioSource>();
                newMusicSource.transform.parent = transform;
            }
            GameObject newSfx2DSource = new GameObject("2D sfx source ");
            sfx2Dsource = newSfx2DSource.AddComponent<AudioSource>();
            newSfx2DSource.transform.parent = transform;

            audioListener = FindObjectOfType<AudioListener>().transform;
            if(FindObjectOfType<Player>() != null)
            {
                playerPos = FindObjectOfType<Player>().transform;
            }

            masterVolumePercent = PlayerPrefs.GetFloat("master volume", 1);
            sfxVolumePercent = PlayerPrefs.GetFloat("sfx volume", 1);
            musicVolumePercent = PlayerPrefs.GetFloat("music volume", 1);
            PlayerPrefs.Save();
        }

    }

    void Update()
    {
        if (playerPos != null)
        {
            audioListener.position = playerPos.position;
        }
    }

    public void setVolume(float volumePercent, AudioChannel channel)
    {
        switch (channel)
        {
            case AudioChannel.Master:
                masterVolumePercent = volumePercent;
                break;
            case AudioChannel.Sfx:
                sfxVolumePercent = volumePercent;
                break;
            case AudioChannel.Music:
                musicVolumePercent = volumePercent;
                break;
        }

        musicScores[0].volume = musicVolumePercent * masterVolumePercent;
        musicScores[1].volume = musicVolumePercent * masterVolumePercent;

        PlayerPrefs.SetFloat("master volume", masterVolumePercent);
        PlayerPrefs.SetFloat("sfx volume", sfxVolumePercent);
        PlayerPrefs.SetFloat("music volume", musicVolumePercent);
    }

    public void playMusic(AudioClip clip, float fadeDuration = 1)
    {
        activeMusicIndex = 1 - activeMusicIndex;
        musicScores[activeMusicIndex].clip = clip;
        musicScores[activeMusicIndex].Play();

        StartCoroutine(MusicFade(fadeDuration));
    }

    public void playSound(AudioClip clip, Vector3 pos)
    {
        if (clip != null) {
            AudioSource.PlayClipAtPoint(clip, pos, sfxVolumePercent * masterVolumePercent);
        }
    }

    public void playSound(string soundName, Vector3 pos)
    {
        playSound(library.getSoundName(soundName), pos);
    }

    public void play2DSound(string soundName)
    {
        sfx2Dsource.PlayOneShot(library.getSoundName(soundName), sfxVolumePercent * masterVolumePercent);
    }

    IEnumerator MusicFade(float duration)
    {
        float percent = 0;
        while (percent < 1){
            percent += Time.deltaTime * 1 / duration;
            musicScores[activeMusicIndex].volume = Mathf.Lerp(0, musicVolumePercent * masterVolumePercent, percent);
            musicScores[1-activeMusicIndex].volume = Mathf.Lerp(musicVolumePercent * masterVolumePercent, 0, percent);
            yield return null;
        }
    }
}
