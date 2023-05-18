using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    GameDirector gameDirector;
    AreaManager areaManager;

    public Transform musicParent;
    public Transform sfxParent;

    List<AudioSource> musicSources = new List<AudioSource>();
    List<AudioSource> sfxSources = new List<AudioSource>();
    int activeMusicSource =  0;
    int emptyMusicSource = 1;

    public AudioClip songForest;
    //public AudioClip songNight;
    public AudioClip songInterior;
    AudioClip lastSong;

    Locations curLocation = Locations.None;
     

    void Start()
    {
        areaManager = GetComponent<AreaManager>();

        // GameObject managers = GameObject.FindGameObjectWithTag("Managers");
        // gameDirector = managers.GetComponent<GameDirector>();

        foreach(Transform child in musicParent) {
            musicSources.Add(child.gameObject.GetComponent<AudioSource>());
        }

        //  foreach(Transform child in sfxParent) {
        //     sfxSources.Add(child.gameObject.GetComponent<AudioSource>());
        // }

        // DayPhaseChanged(GameDirector.dayPhase);
        //AdressLocations activeLocation = managers.GetComponent<AreaManager>().ActiveAdressLocation;
        //SetMusicLocation(activeLocation);        
    }

    void OnEnable() {
        AreaManager.AreaLoaded += AreaLoaded;
    }

    void OnDisable() {
        AreaManager.AreaLoaded -= AreaLoaded;
    }

    // void DayPhaseChanged(GameDirector.DayPhase dayPhase) {
       
    //    //if(curLocation != AdressLocations.Home) {
    //         AudioClip oldSong = dayPhase == GameDirector.DayPhase.Day ? songNight : songDay;
    //         AudioClip newSong = dayPhase == GameDirector.DayPhase.Day ? songDay : songNight;
    //         StartCoroutine(FadeMusic(oldSong, newSong));
    //    //}
    // }

    //WORKING SETUP FOR INTERIOR MUSIC AND CHANGING MUSIC BASED ON LOCATION
    void AreaLoaded(Area areaScript) {                
        SetMusicLocation(areaScript);       
    }

    void SetMusicLocation(Area area) {

         //Change to interior music
        if(area.location != curLocation && area.location == Locations.Home) {
             StartCoroutine(FadeMusic(lastSong, songInterior));
        }

        //Exit interior to forest, change it based on phase of the day
        if(area.location != curLocation && area.location == Locations.Forest) {
            //AudioClip newSong = GameDirector.dayPhase == GameDirector.DayPhase.Day ? songDay : songNight;
            StartCoroutine(FadeMusic(lastSong, songForest));
        }

        curLocation = area.location;
    }

    //Have two audiosources for music, so that current song can fade out, while new song fades in
    //Active is the one currently playing and with new dayphase, will fade out. While Empty source is the one fading in with new song
    IEnumerator FadeMusic(AudioClip oldSong, AudioClip newSong) {
        float fadeIndex = 0f;
        float fadeSpeed = 1.0f;

        musicSources[activeMusicSource].clip = oldSong;
        musicSources[emptyMusicSource].clip = newSong;
        lastSong = newSong;

        while(fadeIndex < 1.0f) {
            fadeIndex += (Time.deltaTime * (1f/fadeSpeed));

            musicSources[activeMusicSource].volume = 1.0f - fadeIndex;
            musicSources[emptyMusicSource].volume = fadeIndex;

            yield return null;
        }

        musicSources[activeMusicSource].volume = 0f;
        musicSources[emptyMusicSource].volume = 1f;

        activeMusicSource++; if(activeMusicSource > 1) { activeMusicSource = 0; }
        emptyMusicSource++; if(emptyMusicSource > 1) { emptyMusicSource = 0; }

        if(musicSources[activeMusicSource].isPlaying == false) {
            musicSources[activeMusicSource].Play();
            musicSources[emptyMusicSource].Play();
        }
    }
}
