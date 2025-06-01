
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    
    public Toggle Effect, Music;

    
    public Dropdown qualityLevel;
  

    void Start()
    {
       
        if (PlayerPrefs.GetInt("Effect") == 1)  
            Effect.isOn = true;
        else
            Effect.isOn = false;
       
        if (PlayerPrefs.GetInt("Music") == 1)      
            Music.isOn = true;
        else
            Music.isOn = false;
        
        qualityLevel.value = PlayerPrefs.GetInt("QualityLevel");
       
    }

    #region Effect
    
    public void Toggle_Effect()
    {
        StartCoroutine(Save_Effect());
    }
    
    IEnumerator Save_Effect()
    {
        yield return new WaitForSeconds(0.01f);
        if(Effect.isOn)
            PlayerPrefs.SetInt("Effect", 1);
        else
            PlayerPrefs.SetInt("Effect", 0);
    }
    
    #endregion

    #region Music
    
    public void Toggle_Music()
    {
        StartCoroutine(Save_Music());
    }
    
    IEnumerator Save_Music()
    {
        yield return new WaitForSeconds(0.01f);
        if (Music.isOn)
            PlayerPrefs.SetInt("Music", 1);
        else
            PlayerPrefs.SetInt("Music", 0);
    }
   
    #endregion

    #region Quality
    
    public void Toggle_Quality()
    {
        StartCoroutine(Save_Quality());
    }
    
    IEnumerator Save_Quality()
    {
        yield return new WaitForSeconds(0.01f);     
        
        PlayerPrefs.SetInt("QualityLevel", qualityLevel.value);

        QualitySettings.SetQualityLevel(qualityLevel.value);
    }
    
    #endregion

}
