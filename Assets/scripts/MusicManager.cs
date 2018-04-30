using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{

    public AudioClip gameMusic;
    public AudioClip menuMusic;

    string sceneName;

    /* Deprecated code apparently
    void Start()
    {
        OnLevelWasLoaded(0);
    }

    void OnLevelWasLoaded(int sceneIndex)
    {
        string newSceneName = SceneManager.GetActiveScene().name;
        if(newSceneName != sceneName)
        {
            sceneName = newSceneName;
            Invoke("PlayMusic", .2f); //invoked with small delay to avoid playing music before destroyed audio manager causing overlaps
        }
    }*/
    void Awake()
    {
        SceneManager.sceneLoaded += loadScene;
    }

    void Start()
    {
        SceneManager.sceneLoaded += loadScene;
    }

    void loadScene(Scene scene, LoadSceneMode mode)
    {
        string newSceneName = SceneManager.GetActiveScene().name;
        if (newSceneName != sceneName)
        {
            sceneName = newSceneName;
            Invoke("PlayMusic", .2f); //invoked with small delay to avoid playing music before destroyed audio manager causing overlaps
        }
    }

    void PlayMusic()
    {
        AudioClip cliptoPlay = null;
        if (sceneName == "Menu")
        {
            cliptoPlay = menuMusic;
        }
        else if (sceneName == "Game")
        {
            cliptoPlay = gameMusic;
        }
        if (cliptoPlay != null)
        {
            AudioManager.instance.playMusic(cliptoPlay, 2);
            Invoke("PlayMusic", cliptoPlay.length);
        }
    }
}
