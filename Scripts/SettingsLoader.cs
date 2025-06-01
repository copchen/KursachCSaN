

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsLoader : MonoBehaviour
{

   
    [Space(5)]
    [Header("Post processing Volume")]
    public GameObject volume;

   
    [Space(5)]
    [Header("Load Game Settings")]
    public AudioSource musicSource;

    void Start()
    {
        if (GetComponent<AudioSource>())
            musicSource = GetComponent<AudioSource>();

        #region Volume
        if(volume)
        {
           
            if (PlayerPrefs.GetInt("Effect") == 1) // 1 = true , 0 = false        
                volume.SetActive(true);
            else
                volume.SetActive(false);
        }
        #endregion

        #region Music
        if (musicSource)
        {
            if (PlayerPrefs.GetInt("Music") == 1)     
                musicSource.Play();
            else
                musicSource.Stop();
        }
        #endregion
    }
}
