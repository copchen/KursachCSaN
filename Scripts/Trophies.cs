

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trophies : MonoBehaviour
{
    
    public Image kills_1000, kills_5000, kills_10000, kills_100000;
    public Image scores_100000, scores_1000000;
    public Image wave_100, wave_1000;
    public Image unlock_Level4;


    void Start()
    {
        Update_Trophies();
    }

    
    public void Update_Trophies()
    {
        if (PlayerPrefs.GetInt("Total Kills") >= 1000) 
            kills_1000.color = Color.green;
        else
            kills_1000.color = Color.red;
        //------------------------------------------------
        if (PlayerPrefs.GetInt("Total Kills") >= 5000) 
            kills_5000.color = Color.green;
        else
            kills_5000.color = Color.red;
        //------------------------------------------------
        if (PlayerPrefs.GetInt("Total Kills") >= 10000)
            kills_10000.color = Color.green;
        else
            kills_10000.color = Color.red;
        //------------------------------------------------
        if (PlayerPrefs.GetInt("Total Kills") >= 100000) 
            kills_100000.color = Color.green;
        else
            kills_100000.color = Color.red;
        //------------------------------------------------
        if (PlayerPrefs.GetInt("Total Scores") >= 100000) 
            scores_100000.color = Color.green;
        else
            scores_100000.color = Color.red;
        //------------------------------------------------
        if (PlayerPrefs.GetInt("Total Scores") >= 1000000) 
            scores_1000000.color = Color.green;
        else
            scores_1000000.color = Color.red;
        //------------------------------------------------
        if (PlayerPrefs.GetInt("Level Unlocked3") == 1) 
            unlock_Level4.color = Color.green;
        else
            unlock_Level4.color = Color.red;
        //------------------------------------------------
        if (PlayerPrefs.GetInt("Total Waves Passed") >= 100) 
            wave_100.color = Color.green;
        else
            wave_100.color = Color.red;
        //------------------------------------------------
        if (PlayerPrefs.GetInt("Total Waves Passed") >= 1000) 
            wave_1000.color = Color.green;
        else
            wave_1000.color = Color.red;
        //------------------------------------------------
    }
}
